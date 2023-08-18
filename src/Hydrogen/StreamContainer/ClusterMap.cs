// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// A cluster map used to store disparate data streams into connected chains of clusters. <see cref="StreamContainer"/> uses
/// this to store multiple streams of data into a single stream.
/// </summary>
/// <remarks>Not thread-safe.</remarks>
public class ClusterMap {

	public event EventHandlerEx<object, ClusterMapChangedEventArgs> Changed;

	private readonly IExtendedList<Cluster> _clusters;

	public ClusterMap(IExtendedList<Cluster> clusters, int clusterSize) {
		Guard.ArgumentNotNull(clusters, nameof(clusters));
		Guard.ArgumentGTE(clusterSize, 0, nameof(clusterSize));
		_clusters = clusters.AsSynchronized();
		ClusterSize = clusterSize;
		ZeroClusterBytes = new byte[clusterSize];
	}

	public IReadOnlyExtendedList<Cluster> Clusters => _clusters;

	public int ClusterSize { get; }

	public byte[] ZeroClusterBytes { get; }

	public bool SuppressEvents { get; set; }

	public virtual (long, long) NewClusterChain(long quantity, long terminalValue) {
		Guard.ArgumentGT(quantity, 0, nameof(quantity));
		var @event = new ClusterMapChangedEventArgs() { ChainTerminal = terminalValue };
		var startClusterIndex = _clusters.Count;
		var startCluster = CreateCluster();
		@event.ClusterCountDelta++;
		@event.ChainNewStartCluster = startClusterIndex;
		@event.ChainNewEndCluster = startClusterIndex;
		@event.AddedClusters.Add(startClusterIndex);
		@event.AllChanges.Add((ClusterMapChangedEventArgs.MutationType.Added, startClusterIndex));

		// Make start cluster FIRST and LAST
		startCluster.Traits = ClusterTraits.Start | ClusterTraits.End;
		startCluster.Prev = terminalValue;
		startCluster.Next = terminalValue;
		_clusters.Add(startCluster);
		if (quantity > 1) {
			AppendClustersToEnd(startClusterIndex, quantity - 1, @event);
			@event.ChainOriginalStartCluster = null; // since we created it in this method (append thinks was pre-existing)
			@event.ChainOriginalEndCluster = null; // same
		}
		NotifyClustersChanged(@event);
		return (@event.ChainNewStartCluster.Value, @event.ChainNewEndCluster.Value);
	}

	public virtual long AppendClustersToEnd(long fromEnd, long quantity) {
		var @event = new ClusterMapChangedEventArgs();
		var result = AppendClustersToEnd(fromEnd, quantity, @event);
		NotifyClustersChanged(@event);
		return result;
	}

	public virtual long AppendClustersToEnd(long fromEnd, long quantity, ClusterMapChangedEventArgs @event) {
		// Appending clusters
		// A -> [B] -> [C] -> [D] -> [E] 
		// (1) A.Traits = !Last
		// (2) E.Next = A.Prev   (terminals)
		// (3) A.Next = B 
		// (4) B.Prev = A, C.Prev = B, D.Prev = C, E.Prev = D
		// (5) B.Next = C, C.Next = D, D.Next = E
		// (6) E.Traits = Last

		Guard.ArgumentInRange(fromEnd, 0, _clusters.Count - 1, nameof(fromEnd));
		Guard.ArgumentGTE(quantity, 0, nameof(quantity));
		Guard.Argument(ReadClusterTraits(fromEnd).HasFlag(ClusterTraits.End), nameof(fromEnd), "Can only append from last cluster in chain");

		// Appending nothing case
		var newEndCluster = fromEnd;
		if (quantity == 0)
			return newEndCluster;

		// A.Traits = !Last
		MaskClusterTraits(fromEnd, ClusterTraits.End, false, @event);
		var endTerminalValue = ReadClusterNext(fromEnd);  // remember what the end terminal value was
		@event.ChainTerminal = endTerminalValue;
		@event.ChainOriginalEndCluster = fromEnd;

		// Simply add new clusters, connecting them along the way
		var previous = fromEnd;
		for (var i = 0L; i < quantity; i++) {
			var newCluster = CreateCluster();
			var newClusterIX = _clusters.Count;
			@event.ClusterCountDelta++;
			@event.AddedClusters.Add(newClusterIX);
			@event.AllChanges.Add((ClusterMapChangedEventArgs.MutationType.Added, newClusterIX));

			// make new cluster connect to previous
			newCluster.Prev = previous;

			if (i == 0) {
				// creating first next cluster (B)
				WriteClusterNext(previous, newClusterIX, @event); // (3) A.Next = B 
			}

			if (i < quantity - 1) {
				// creating first/middle cluster
				// (5) B.Next = C, C.Next = D, D.Next = E, ...
				newCluster.Next = newClusterIX + 1;
			} else {
				// creating last cluster (E)
				newCluster.Traits |= ClusterTraits.End; // (6) E.Traits = Last
				newCluster.Next = endTerminalValue; // (2) E.Next = A.Next
				newEndCluster = newClusterIX;
				@event.ChainNewEndCluster = newEndCluster;
			}

			// finally save cluster to list
			_clusters.Add(newCluster);
			previous = newClusterIX;

		}
		return newEndCluster;

	}

	public virtual long RemoveNextClusters(long fromCluster, long quantity = long.MaxValue) {
		// Walk back cluster chain end and then delete forward	
		Guard.ArgumentInRange(fromCluster, 0, _clusters.Count - 1, nameof(fromCluster));

		if (quantity == 0)
			return 0;

		var toRemove = 1L;
		while (!ReadClusterTraits(fromCluster).HasFlag(ClusterTraits.End) && toRemove < quantity) {
			fromCluster = ReadClusterNext(fromCluster);
			toRemove++;
		}
		return RemoveBackwards(fromCluster, toRemove);
	}

	public virtual long RemoveBackwards(long fromCluster, long quantity) {
		var deferredEvent = new ClusterMapChangedEventArgs();
		var clustersRemoved = 0L;

		Guard.ArgumentInRange(fromCluster, 0, _clusters.Count - 1, nameof(fromCluster));
		if (quantity == 0)
			return 0;
		var fromTraits = ReadClusterTraits(fromCluster);
		var fromWasEnd = fromTraits.HasFlag(ClusterTraits.End);
		var fromNext = ReadClusterNext(fromCluster);

		// Iterate through and remove clusters by tip-substitution
		var removeCluster = fromCluster;
		var removeClusterPrev = 0L;
		var removeClusterIsStart = false;

		// remove clusters backwards from end until we've removed the requested quantity (or we've removed the start cluster of chain)
		while (clustersRemoved < quantity && !removeClusterIsStart) {
			// Read remove cluster info
			var removeClusterTraits = ReadClusterTraits(removeCluster);
			removeClusterIsStart = removeClusterTraits.HasFlag(ClusterTraits.Start);
			removeClusterPrev = ReadClusterPrev(removeCluster);

			// Determine tip cluster
			var logicalClusterCount = _clusters.Count - clustersRemoved;
			var tipCluster = logicalClusterCount - 1;
			var removeClusterPrevIsTip = removeClusterPrev == tipCluster;

			// Move the tip cluster into the removed cluster's spot
			MigrateTipClusterTo(tipCluster, removeCluster, deferredEvent);

			// if the removed cluster's previous was the tip cluster (just moved into the removed clusters spot)
			// then it follows that the deleted cluster's previous is now where the deleted was cluster was
			if (removeClusterPrevIsTip && !removeClusterIsStart)
				removeClusterPrev = removeCluster;

			clustersRemoved++;
			deferredEvent.ClusterCountDelta--;

			if (!removeClusterIsStart) {
				removeCluster = removeClusterPrev;
			}
		}

		if (removeClusterIsStart && fromWasEnd) {
			// removed entire chain, nothing to do
			// [A] -> [B] -> [C] -> [D] -> [E] 
			Guard.Ensure(removeClusterPrev == fromNext, "Cluster chain had mismatching terminals");
			deferredEvent.ChainTerminal = removeClusterPrev;
			deferredEvent.ChainOriginalEndCluster = fromCluster;
			deferredEvent.ChainOriginalStartCluster = removeCluster;
			deferredEvent.ChainNewEndCluster = null;
			deferredEvent.ChainNewStartCluster = null;
		} else if (removeClusterIsStart && !fromWasEnd) {
			// removed from middle to beginning
			// [A] -> [B] -> [C] -> D -> E 
			// - set D.Traits.Start = true
			// - set D.Previous = A.Previous (terminal value propagates)
			MaskClusterTraits(fromNext, ClusterTraits.Start, true, deferredEvent);
			deferredEvent.ChainTerminal = removeClusterPrev;
			WriteClusterPrev(fromNext, deferredEvent.ChainTerminal.Value, deferredEvent);
			deferredEvent.ChainOriginalStartCluster = removeCluster;
			deferredEvent.ChainNewStartCluster = fromNext;
		} else if (!removeClusterIsStart && fromWasEnd) {
			// removed from end to middle
			// A -> B -> [C] -> [D] -> [E] 
			// - set B.Traits.End = true
			// - set B.Next = E.Next (terminal value propagates)
			deferredEvent.ChainTerminal = fromNext;
			MaskClusterTraits(removeClusterPrev, ClusterTraits.End, true, deferredEvent);
			WriteClusterNext(removeClusterPrev, deferredEvent.ChainTerminal.Value, deferredEvent);
			deferredEvent.ChainOriginalEndCluster = fromCluster;
			deferredEvent.ChainNewEndCluster = removeClusterPrev;
		} else {
			// removed from middle to middle
			// A -> [B] -> [C] -> [D] -> E 
			// - set A.Next = E
			// - set E.Previous = A
			Guard.Ensure(!removeClusterIsStart && !fromWasEnd, "Internal error: E8F085C8-6025-424D-8F42-C402637DF1A0");
			//@event.ChainTerminal = null;
			WriteClusterNext(removeClusterPrev, fromNext, deferredEvent);
			WriteClusterPrev(fromNext, removeClusterPrev, deferredEvent);
		}

		// finally remove the old clusters from collection
		_clusters.RemoveRange(_clusters.Count - clustersRemoved, clustersRemoved);  // Migrate included removals in @event

		NotifyClustersChanged(deferredEvent);
		return clustersRemoved;
	}

	public long CalculateClusterChainLength(long byteLength) => (long)Math.Ceiling(byteLength / (double)ClusterSize);

	public virtual void Clear() => _clusters.Clear();

	private void MigrateTipClusterTo(long tipClusterIndex, long to, ClusterMapChangedEventArgs pendingEvent) {
		Guard.ArgumentInRange(tipClusterIndex, 0, _clusters.Count - 1, nameof(tipClusterIndex));
		Guard.ArgumentInRange(to, 0, _clusters.Count - 1, nameof(to));
		Guard.Argument(to <= tipClusterIndex, nameof(to), $"Cannot be greater than {nameof(tipClusterIndex)}");

		// If migrating to self, do nothing
		if (to == tipClusterIndex) {
			pendingEvent.RemovedClusters.Add(tipClusterIndex);
			pendingEvent.AllChanges.Add((ClusterMapChangedEventArgs.MutationType.Removed, tipClusterIndex));  // actual remove done by caller
			return;
		}

		// Fetch clusters in question
		var tipCluster = _clusters[tipClusterIndex];
		var tipWasStartCluster = tipCluster.Traits.HasFlag(ClusterTraits.Start);
		var tipWasEndCluster = tipCluster.Traits.HasFlag(ClusterTraits.End);

		// Case: (tip D moved to index B)     ;/ the tip D is the last cluster in collection but it is not the last cluster in the chain
		//  A -> [B] -> C -> [D] -> E
		//  (1) C.Next = B   (iff D is not Start)
		//  (2) E.Previous = B  (iff D is not End)
		if (tipWasStartCluster) {
			pendingEvent.InformChainNewStart(tipCluster.Prev, to);
		} else {
			WriteClusterNext(tipCluster.Prev, to, pendingEvent); // (1)
		}

		if (tipWasEndCluster) {
			pendingEvent.InformChainNewEnd(tipCluster.Next, to);
		} else {
			WriteClusterPrev(tipCluster.Next, to, pendingEvent); // (2)
		}

		_clusters.Update(to, tipCluster);
		pendingEvent.ModifiedClusters.Add(to);
		pendingEvent.InformMovedCluster(tipClusterIndex, to);
		pendingEvent.RemovedClusters.Add(tipClusterIndex);
		pendingEvent.AllChanges.Add((ClusterMapChangedEventArgs.MutationType.Modified, to));
		pendingEvent.AllChanges.Add((ClusterMapChangedEventArgs.MutationType.Removed, tipClusterIndex));  // actual remove done by caller
	}

	protected Cluster CreateCluster() {
		var cluster = new Cluster() {
			Prev = Cluster.Null,
			Next = Cluster.Null,
			Traits = ClusterTraits.None,
			Data = new byte[ClusterSize],
		};
		return cluster;
	}


	#region ReadClusterTraits

	public virtual ClusterTraits ReadClusterTraits(long cluster) => _clusters[cluster].Traits;

	#endregion

	#region WriteClusterTraits

	public void WriteClusterTraits(long cluster, ClusterTraits traits) {
		var @event = new ClusterMapChangedEventArgs();
		WriteClusterTraits(cluster, traits, @event);
		NotifyClustersChanged(@event);
	}

	protected void WriteClusterTraits(long cluster, ClusterTraits traits, ClusterMapChangedEventArgs pendingEvent) {
		WriteClusterTraitsInternal(cluster, traits);
		pendingEvent.ModifiedClusters.Add(cluster);
		pendingEvent.AllChanges.Add((ClusterMapChangedEventArgs.MutationType.Modified, cluster));
	}

	protected virtual void WriteClusterTraitsInternal(long cluster, ClusterTraits traits) {
		var clusterRecord = _clusters[cluster];
		clusterRecord.Traits = traits;
		_clusters[cluster] = clusterRecord;
	}

	#endregion

	#region MaskClusterTraits

	public void MaskClusterTraits(long cluster, ClusterTraits traits, bool on) {
		var @event = new ClusterMapChangedEventArgs();
		MaskClusterTraits(cluster, traits, on, @event);
		NotifyClustersChanged(@event);
	}

	protected virtual void MaskClusterTraits(long cluster, ClusterTraits traits, bool on, ClusterMapChangedEventArgs pendingEvent) {
		MaskClusterTraitsInternal(cluster, traits, on);
		pendingEvent.ModifiedClusters.Add(cluster);
		pendingEvent.AllChanges.Add((ClusterMapChangedEventArgs.MutationType.Modified, cluster));
	}

	protected virtual void MaskClusterTraitsInternal(long cluster, ClusterTraits traits, bool on) {
		var clusterRecord = _clusters[cluster];
		clusterRecord.Traits = clusterRecord.Traits.CopyAndSetFlags(traits, on);
		_clusters[cluster] = clusterRecord;
	}

	#endregion

	#region ReadClusterPrev

	public virtual long ReadClusterPrev(long cluster) => _clusters[cluster].Prev;

	#endregion

	#region WriteClusterPrev

	public void WriteClusterPrev(long cluster, long prev) {
		var @event = new ClusterMapChangedEventArgs();
		WriteClusterPrev(cluster, prev, @event);
		NotifyClustersChanged(@event);
	}

	protected virtual void WriteClusterPrev(long cluster, long prev, ClusterMapChangedEventArgs pendingEvent) {
		WriteClusterPrevInternal(cluster, prev);
		pendingEvent.ModifiedClusters.Add(cluster);
		pendingEvent.AllChanges.Add((ClusterMapChangedEventArgs.MutationType.Modified, cluster));
	}

	protected virtual void WriteClusterPrevInternal(long cluster, long prev) {
		var clusterRecord = _clusters[cluster];
		clusterRecord.Prev = prev;
		_clusters[cluster] = clusterRecord;
	}

	#endregion

	#region ReadClusterNext

	public virtual long ReadClusterNext(long cluster) => _clusters[cluster].Next;

	#endregion

	#region WriteClusterNext

	public virtual void WriteClusterNext(long cluster, long next) {
		var @event = new ClusterMapChangedEventArgs();
		WriteClusterNext(cluster, next, @event);
		NotifyClustersChanged(@event);
	}

	public virtual void WriteClusterNext(long cluster, long next, ClusterMapChangedEventArgs pendingEvent) {
		WriteClusterNextInternal(cluster, next);
		pendingEvent.ModifiedClusters.Add(cluster);
		pendingEvent.AllChanges.Add((ClusterMapChangedEventArgs.MutationType.Modified, cluster));
	}

	protected virtual void WriteClusterNextInternal(long cluster, long next) {
		var clusterRecord = _clusters[cluster];
		clusterRecord.Next = next;
		_clusters[cluster] = clusterRecord;
	}
	#endregion

	#region ReadClusterData

	public virtual byte[] ReadClusterData(long cluster, long offset, long size) {
		return _clusters[cluster].Data.AsSpan(checked((int)offset), checked((int)size)).ToArray();
	}

	#endregion

	#region WriteClusterData

	public virtual void WriteClusterData(long cluster, long offset, ReadOnlySpan<byte> data) {
		var @event = new ClusterMapChangedEventArgs();
		WriteClusterData(cluster, offset, data, @event);
		NotifyClustersChanged(@event);
	}

	protected virtual void WriteClusterData(long cluster, long offset, ReadOnlySpan<byte> data, ClusterMapChangedEventArgs pendingEvent) {
		WriteClusterDataInternal(cluster, offset, data);
		pendingEvent.ModifiedClusters.Add(cluster);
		pendingEvent.AllChanges.Add((ClusterMapChangedEventArgs.MutationType.Modified, cluster));
	}

	protected virtual void WriteClusterDataInternal(long cluster, long offset, ReadOnlySpan<byte> data) {
		var clusterRecord = _clusters[cluster];
		clusterRecord.Data = data.ToArray();
		_clusters[cluster] = clusterRecord;
	}

	#endregion

	private void NotifyClustersChanged(ClusterMapChangedEventArgs @event) {
		if (SuppressEvents)
			return;
		Changed?.Invoke(this, @event);
	}

}

