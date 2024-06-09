// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// A cluster map used to store disparate data streams into connected chains of clusters. <see cref="ClusteredStreams"/> uses
/// this to store multiple streams of data into a single stream.
/// </summary>
/// <remarks>Not thread-safe.</remarks>
public abstract class ClusterMap {

	public event EventHandlerEx<object, ClusterMapChangedEventArgs> Changed;

	public abstract IReadOnlyExtendedList<Cluster> Clusters { get; }

	public abstract int ClusterSize { get; }

	public abstract byte[] ZeroClusterBytes { get; }

	public bool SuppressEvents { get; set; }

	internal abstract long ClusterCount { get; }

	public virtual (long, long) NewClusterChain(long quantity, long terminalValue) {
		Guard.ArgumentGT(quantity, 0, nameof(quantity));
		var @event = new ClusterMapChangedEventArgs() { ChainTerminal = terminalValue };
		var startClusterIndex = ClusterCount;
		var startCluster = CreateCluster();
		@event.ClusterCountDelta++;
		@event.ChainNewStartCluster = startClusterIndex;
		@event.ChainNewEndCluster = startClusterIndex;
		@event.AddedClusters.Add(startClusterIndex);

		// Make start cluster FIRST and LAST
		startCluster.Traits = ClusterTraits.Start | ClusterTraits.End;
		startCluster.Prev = terminalValue;
		startCluster.Next = terminalValue;
		AddCluster(startCluster);
		
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

		Guard.ArgumentInRange(fromEnd, 0, ClusterCount - 1, nameof(fromEnd));
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
			var newClusterIX = ClusterCount;
			@event.ClusterCountDelta++;
			@event.AddedClusters.Add(newClusterIX);

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
			AddCluster(newCluster);
			previous = newClusterIX;

		}
		return newEndCluster;

	}

	public virtual long RemoveNextClusters(long fromCluster, long quantity = long.MaxValue) {
		// Walk back cluster chain end and then delete forward	
		Guard.ArgumentInRange(fromCluster, 0, ClusterCount - 1, nameof(fromCluster));

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

		Guard.ArgumentInRange(fromCluster, 0, ClusterCount - 1, nameof(fromCluster));
		if (quantity == 0)
			return 0;
		var fromTraits = ReadClusterTraits(fromCluster);
		var fromWasEnd = fromTraits.HasFlag(ClusterTraits.End);
		var fromWasStart = fromTraits.HasFlag(ClusterTraits.Start);
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
			var logicalClusterCount = ClusterCount - clustersRemoved;
			var tipCluster = logicalClusterCount - 1;
			var removeClusterPrevIsTip = removeClusterPrev == tipCluster;

			// Move the tip cluster into the removed cluster's spot
			MigrateTipClusterTo(tipCluster, removeCluster, deferredEvent);

			// if the fromNext was migrated, we need to track that
			if (!fromWasEnd && tipCluster == fromNext)
				fromNext = removeCluster;

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
		RemoveEndClusters(clustersRemoved);

		NotifyClustersChanged(deferredEvent);
		return clustersRemoved;
	}

	public long CalculateClusterChainLength(long byteLength) => (long)Math.Ceiling(byteLength / (double)ClusterSize);

	public virtual void Clear() => ClearClusters();

	private void MigrateTipClusterTo(long tipClusterIndex, long to, ClusterMapChangedEventArgs pendingEvent) {
		Guard.ArgumentInRange(tipClusterIndex, 0, ClusterCount - 1, nameof(tipClusterIndex));
		Guard.ArgumentInRange(to, 0, ClusterCount - 1, nameof(to));
		Guard.Argument(to <= tipClusterIndex, nameof(to), $"Cannot be greater than {nameof(tipClusterIndex)}");

		// If migrating to self, do nothing
		if (to == tipClusterIndex) {
			pendingEvent.RemovedClusters.Add(tipClusterIndex);
			return;
		}

		// Fetch clusters in question
		var tipCluster = GetCluster(tipClusterIndex);
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

		UpdateCluster(to, tipCluster);
		pendingEvent.ModifiedClusters.Add(to);
		pendingEvent.InformMovedCluster(tipClusterIndex, to);
		pendingEvent.RemovedClusters.Add(tipClusterIndex);
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

	protected void NotifyClustersChanged(ClusterMapChangedEventArgs @event) {
		if (SuppressEvents)
			return;
		Changed?.Invoke(this, @event);
	}


	#region Cluster Header

	internal abstract void AddCluster(Cluster cluster);

	internal abstract Cluster GetCluster(long index);

	internal abstract void UpdateCluster(long index, Cluster cluster);

	internal abstract void RemoveEndClusters(long quantity);

	internal abstract void ClearClusters();

	#endregion

	#region ReadClusterTraits

	internal abstract ClusterTraits ReadClusterTraits(long cluster);

	#endregion

	#region WriteClusterTraits

	internal void WriteClusterTraits(long cluster, ClusterTraits traits) {
		var @event = new ClusterMapChangedEventArgs();
		WriteClusterTraits(cluster, traits, @event);
		NotifyClustersChanged(@event);
	}

	internal void WriteClusterTraits(long cluster, ClusterTraits traits, ClusterMapChangedEventArgs pendingEvent) {
		WriteClusterTraitsInternal(cluster, traits);
		pendingEvent.ModifiedClusters.Add(cluster);
	}

	internal abstract void WriteClusterTraitsInternal(long cluster, ClusterTraits traits);

	#endregion

	#region MaskClusterTraits

	internal void MaskClusterTraits(long cluster, ClusterTraits traits, bool on) {
		var @event = new ClusterMapChangedEventArgs();
		MaskClusterTraits(cluster, traits, on, @event);
		NotifyClustersChanged(@event);
	}

	internal void MaskClusterTraits(long cluster, ClusterTraits traits, bool on, ClusterMapChangedEventArgs pendingEvent) {
		MaskClusterTraitsInternal(cluster, traits, on);
		pendingEvent.ModifiedClusters.Add(cluster);
	}

	internal abstract void MaskClusterTraitsInternal(long cluster, ClusterTraits traits, bool on);

	#endregion

	#region ReadClusterPrev

	internal abstract long ReadClusterPrev(long cluster);

	#endregion

	#region WriteClusterPrev

	internal void WriteClusterPrev(long cluster, long prev) {
		var @event = new ClusterMapChangedEventArgs();
		WriteClusterPrev(cluster, prev, @event);
		NotifyClustersChanged(@event);
	}

	internal void WriteClusterPrev(long cluster, long prev, ClusterMapChangedEventArgs pendingEvent) {
		WriteClusterPrevInternal(cluster, prev);
		pendingEvent.ModifiedClusters.Add(cluster);
	}

	internal abstract void WriteClusterPrevInternal(long cluster, long prev);

	#endregion

	#region ReadClusterNext

	internal abstract long ReadClusterNext(long cluster);

	#endregion

	#region WriteClusterNext

	internal void WriteClusterNext(long cluster, long next) {
		var @event = new ClusterMapChangedEventArgs();
		WriteClusterNext(cluster, next, @event);
		NotifyClustersChanged(@event);
	}

	internal void WriteClusterNext(long cluster, long next, ClusterMapChangedEventArgs pendingEvent) {
		WriteClusterNextInternal(cluster, next);
		pendingEvent.ModifiedClusters.Add(cluster);
	}

	internal abstract void WriteClusterNextInternal(long cluster, long next);
	
	#endregion

	#region ReadClusterData

	internal abstract byte[] ReadClusterData(long cluster, long offset, long size);

	#endregion

	#region WriteClusterData

	internal void WriteClusterData(long cluster, long offset, ReadOnlySpan<byte> data) {
		var @event = new ClusterMapChangedEventArgs();
		WriteClusterData(cluster, offset, data, @event);
		NotifyClustersChanged(@event);
	}

	internal void WriteClusterData(long cluster, long offset, ReadOnlySpan<byte> data, ClusterMapChangedEventArgs pendingEvent) {
		WriteClusterDataInternal(cluster, offset, data);
		pendingEvent.ModifiedClusters.Add(cluster);
	}

	internal abstract void WriteClusterDataInternal(long cluster, long offset, ReadOnlySpan<byte> data);

	#endregion


}

