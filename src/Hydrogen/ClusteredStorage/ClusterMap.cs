// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Hydrogen;

/// <summary>
/// Used for managing a collection of clusters as connected chains. 
/// </summary>
internal class ClusterMap<TInner> : IClusterMap where TInner : IExtendedList<Cluster> {
	public event ClustersCountChangedEventHandler ClusterCountChanged;
	public event ClusterChainCreatedEventHandler ClusterChainCreated;
	public event ClusterChainRemovedEventHandler ClusterChainRemoved;
	public event ClusterChainBoundaryMovedEventHandler ClusterChainStartChanged;
	public event ClusterChainBoundaryMovedEventHandler ClusterChainEndChanged;
	public event ClusterMovedEventHandler ClusterMoved;

	protected readonly SynchronizedExtendedList<Cluster, TInner> _clusters;

	public ClusterMap(TInner clusters, int clusterSize) {
		Guard.ArgumentNotNull(clusters, nameof(clusters));
		Guard.ArgumentGTE(clusterSize, 0, nameof(clusterSize));
		_clusters = clusters.AsSynchronized<Cluster, TInner>();
		ClusterSize = clusterSize;
		ZeroClusterBytes = new byte[clusterSize];
	}

	public ISynchronizedObject ParentSyncObject { get => _clusters.ParentSyncObject; set => _clusters.ParentSyncObject = value; }

	public ReaderWriterLockSlim ThreadLock => _clusters.ThreadLock;

	public IReadOnlyExtendedList<Cluster> Clusters => _clusters;

	public int ClusterSize { get; }

	public byte[] ZeroClusterBytes { get; }

	public bool PreventClusterNavigation { get; set; }

	public bool SuppressEvents { get; set; }

	public IDisposable EnterReadScope() => _clusters.EnterReadScope();

	public IDisposable EnterWriteScope() => _clusters.EnterWriteScope();

	public virtual (long, long) NewClusterChain(long quantity, long terminalValue) {
		Guard.ArgumentGT(quantity, 0, nameof(quantity));
		using (EnterWriteScope()) {
			var startClusterIndex = _clusters.Count;
			var endCluster = startClusterIndex;
			var startCluster = CreateCluster();

			// Make start cluster FIRST and LAST
			startCluster.Traits = ClusterTraits.Start | ClusterTraits.End;
			startCluster.Prev = terminalValue;
			startCluster.Next = terminalValue;
			_clusters.Add(startCluster);
			NotifyClusterCountChanged(1, terminalValue);
			NotifyClusterChainCreated(startClusterIndex, startClusterIndex, 1, terminalValue);
			if (quantity > 1) {
				endCluster = AppendClustersToEnd(startClusterIndex, quantity - 1);
			}

			return (startClusterIndex, endCluster);
		}

	}

	public virtual long AppendClustersToEnd(long fromEnd, long quantity) {
		// Appending clusters
		// A -> [B] -> [C] -> [D] -> [E] 
		// (1) A.Traits = !Last
		// (2) E.Next = A.Next
		// (3) A.Next = B 
		// (4) B.Prev = A, C.Prev = B, D.Prev = C, E.Prev = D
		// (5) B.Next = C, C.Next = D, D.Next = E
		// (6) E.Traits = Last


		using (EnterWriteScope()) {
			Guard.ArgumentInRange(fromEnd, 0, _clusters.Count - 1, nameof(fromEnd));
			Guard.ArgumentGTE(quantity, 0, nameof(quantity));
			Guard.Argument(FastReadClusterTraits(fromEnd).HasFlag(ClusterTraits.End), nameof(fromEnd), "Can only append from last cluster in chain");

			// Appending nothing case
			var newEndCluster = fromEnd;
			if (quantity == 0)
				return newEndCluster;

			// (1) A.Traits = !Last
			FastMaskClusterTraits(fromEnd, ClusterTraits.End, false);
			var endTerminalValue = FastReadClusterNext(fromEnd);  // remember what the end terminal value was

			// (4) Simply add new clusters, connecting them along the way
			var previous = fromEnd;
			for (var i = 0L; i < quantity; i++) {
				var newCluster = CreateCluster();
				var newClusterIX = _clusters.Count;

				// (4) make new cluster connect to previous
				newCluster.Prev = previous;

				if (i == 0) {
					// creating first next cluster (B)
					FastWriteClusterNext(previous, newClusterIX); // (3) A.Next = B 
				}

				if (i < quantity - 1) {
					// creating middle cluster
					// (5) B.Next = C, C.Next = D, D.Next = E, ...
					newCluster.Next = newClusterIX + 1;
				} else {
					// creating last cluster (E)
					newCluster.Traits |= ClusterTraits.End; // (6) E.Traits = Last
					newCluster.Next = endTerminalValue; // (2) E.Next = A.Next
					newEndCluster = newClusterIX;
				}

				// finally save cluster to list
				_clusters.Add(newCluster);
				previous = newClusterIX;

				// notify when E is new last cluster
				if (i == quantity - 1)
					NotifyClusterChainEndChanged(newClusterIX, endTerminalValue, quantity);
			}
			NotifyClusterCountChanged(quantity, endTerminalValue);
			return newEndCluster;
		}
	}

	public virtual long RemoveNextClusters(long fromCluster, long quantity = long.MaxValue) {
		// Walk back cluster chain end and then delete forward	
		using (EnterWriteScope()) {
			Guard.ArgumentInRange(fromCluster, 0, _clusters.Count - 1, nameof(fromCluster));

			if (quantity == 0)
				return 0;

			var toRemove = 1L;
			while (!FastReadClusterTraits(fromCluster).HasFlag(ClusterTraits.End) && toRemove < quantity) {
				fromCluster = FastReadClusterNext(fromCluster);
				toRemove++;
			}
			return RemoveBackwards(fromCluster, toRemove);
		}
	}

	public virtual long RemoveBackwards(long fromCluster, long quantity) {


		using (EnterWriteScope()) {
			Guard.ArgumentInRange(fromCluster, 0, _clusters.Count - 1, nameof(fromCluster));
			if (quantity == 0)
				return 0;
			var fromTraits = FastReadClusterTraits(fromCluster);
			var fromWasEnd = fromTraits.HasFlag(ClusterTraits.End);
			var fromNext = FastReadClusterNext(fromCluster);

			// Iterate through and remove clusters by tip-substitution
			var removedStartCluster = false;
			var clustersRemoved = 0L;
			var removeCluster = fromCluster;
			var lastRemovedPrev = 0L;
			var alreadyNotifiedEndClusterMoved = false;
			PreventClusterNavigation = true;
			try {
				while (clustersRemoved < quantity && !removedStartCluster) {
					var removeClusterTraits = FastReadClusterTraits(removeCluster);
					var removeClusterIsEndOfRecordStream = removeClusterTraits.HasFlag(ClusterTraits.End) && FastReadClusterNext(removeCluster) == -1;
					removedStartCluster = removeClusterTraits.HasFlag(ClusterTraits.Start);
					lastRemovedPrev = FastReadClusterPrev(removeCluster);
					var logicalClusterCount = _clusters.Count - clustersRemoved;
					var removedBlockPreviousIsTip = lastRemovedPrev == logicalClusterCount - 1;
					var tipClusterIndex = logicalClusterCount - 1;
					if (removeClusterIsEndOfRecordStream && !removedStartCluster) {
						// since we're moving the end block of the record stream, we need notify this removal now
						// since the record stream needs to be usable during this loop since event handlers
						// may require access to records. This notification ensures the record seeker is updated.
						// To ensure the below doesn't re-do this notification we rely on alreadyNotifiedEndClusterMoved.
						FastMaskClusterTraits(lastRemovedPrev, ClusterTraits.End, true);
						FastWriteClusterNext(lastRemovedPrev, -1);
						NotifyClusterChainEndChanged(lastRemovedPrev, -1, -1);
						alreadyNotifiedEndClusterMoved = true;
					}
					MigrateTipClusterTo(tipClusterIndex, removeCluster);
					// if the deleted cluster's previous was the tip cluster moved into the deleted clusters spot
					// then it follows that the deleted cluster's previous is now where the deleted was cluster was
					if (removedBlockPreviousIsTip && !removedStartCluster)
						lastRemovedPrev = removeCluster;

					clustersRemoved++;
					if (!removedStartCluster) {
						removeCluster = lastRemovedPrev;
						//removeCluster = lastRemovedPrev == logicalClusterCount - 1  ? removeCluster : lastRemovedPrev;
						// TODO: use this once tests passing: removeCluster = lastRemovedPrev;
					}
				}
			} finally {
				PreventClusterNavigation = false;
			}
			long? terminalValue = null;
			if (removedStartCluster && fromWasEnd) {
				// removed entire chain, nothing to do
				// [A] -> [B] -> [C] -> [D] -> [E] 
				Guard.Ensure(lastRemovedPrev == fromNext, "Cluster chain had mismatching terminals");
				terminalValue = lastRemovedPrev;
				NotifyClusterChainRemoved(terminalValue.Value);
			} else if (removedStartCluster && !fromWasEnd) {
				// removed from beginning to middle
				// [A] -> [B] -> [C] -> D -> E 
				// - set D.Traits.First = true
				// - set D.Previous = A.Previous (terminal value propagates)
				FastMaskClusterTraits(fromNext, ClusterTraits.Start, true);
				terminalValue = lastRemovedPrev;
				FastWriteClusterPrev(fromNext, lastRemovedPrev);
				NotifyClusterChainStartChanged(fromNext, terminalValue.Value, -clustersRemoved);
			} else if (!removedStartCluster && fromWasEnd) {
				// removed from middle to end
				// A -> B -> [C] -> [D] -> [E] 
				// - set B.Traits.Last = true
				// - set B.Next = E.Next (terminal value propagates)

				terminalValue = fromNext;
				FastWriteClusterNext(lastRemovedPrev, fromNext);
				if (!alreadyNotifiedEndClusterMoved) {
					FastMaskClusterTraits(lastRemovedPrev, ClusterTraits.End, true);
					NotifyClusterChainEndChanged(lastRemovedPrev, terminalValue.Value, -clustersRemoved); // we already notified in above loop
				}
			} else {
				// removed from middle to middle
				// A -> [B] -> [C] -> [D] -> E 
				// - set A.Next = E
				// - set E.Previous = A
				Guard.Ensure(!removedStartCluster && !fromWasEnd, "Internal error: E8F085C8-6025-424D-8F42-C402637DF1A0");
				terminalValue = null;
				FastWriteClusterNext(lastRemovedPrev, fromNext);
				FastWriteClusterPrev(fromNext, lastRemovedPrev);
			}

			// finally remove the old clusters from collection
			_clusters.RemoveRange(_clusters.Count - clustersRemoved, clustersRemoved);
			NotifyClusterCountChanged(-clustersRemoved, terminalValue);
			//deferredNotifications.ForEach(n => n.Invoke());
			return clustersRemoved;
		}

	}

	public virtual void Clear() => _clusters.Clear();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void MigrateTipClusterTo(long to) => MigrateTipClusterTo(_clusters.Count, to);

	// TODO: once all working, retry the deferred notifications approach from below
	private void MigrateTipClusterTo(long tipClusterIndex, long to) {
		Guard.ArgumentInRange(tipClusterIndex, 0, _clusters.Count - 1, nameof(tipClusterIndex));
		Guard.ArgumentInRange(to, 0, _clusters.Count - 1, nameof(to));
		Guard.Argument(to <= tipClusterIndex, nameof(to), $"Cannot be greater than {nameof(tipClusterIndex)}");

		// If migrating to self, do nothing
		if (to == tipClusterIndex)
			return;

		// Fetch clusters in question
		var tipCluster = _clusters[tipClusterIndex];
		var tipWasFirstCluster = tipCluster.Traits.HasFlag(ClusterTraits.Start);
		var tipWasLastCluster = tipCluster.Traits.HasFlag(ClusterTraits.End);

		// Case: (tip D moved to index B)     ;/ the tip D is the last cluster in collection but it is not the last cluster in the chain
		//  A -> [B] -> C -> [D] -> E
		//  (1) C.Next = B   (iff D is not Start)
		//  (2) E.Previous = B  (iff D is not End)
		long? terminalValue = null;
		if (!tipWasFirstCluster) {
			FastWriteClusterNext(tipCluster.Prev, to); // (1)
		} else {
			terminalValue = tipCluster.Prev;
		}

		if (!tipWasLastCluster) {
			FastWriteClusterPrev(tipCluster.Next, to); // (2)
		} else {
			terminalValue = tipCluster.Next;
		}

		_clusters.Update(to, tipCluster);

		NotifyClusterMoved(tipClusterIndex, to, tipCluster.Traits, terminalValue);

	}

	protected virtual Cluster CreateCluster() => new() {
		Prev = -1,
		Next = -1,
		Traits = ClusterTraits.None,
		Data = new byte[ClusterSize],
	};


	#region Fast Mutators

	public virtual ClusterTraits FastReadClusterTraits(long clusterIndex)
		=> _clusters[clusterIndex].Traits;

	public virtual void FastWriteClusterTraits(long clusterIndex, ClusterTraits traits) {
		var cluster = _clusters[clusterIndex];
		cluster.Traits = traits;
		_clusters[clusterIndex] = cluster;
	}

	public virtual void FastMaskClusterTraits(long clusterIndex, ClusterTraits traits, bool on) {
		var cluster = _clusters[clusterIndex];
		cluster.Traits = cluster.Traits.CopyAndSetFlags(traits, on);
		_clusters[clusterIndex] = cluster;
	}

	public virtual long FastReadClusterPrev(long clusterIndex)
		=> _clusters[clusterIndex].Prev;

	public virtual void FastWriteClusterPrev(long clusterIndex, long prev) {
		var cluster = _clusters[clusterIndex];
		cluster.Prev = prev;
		_clusters[clusterIndex] = cluster;
	}

	public virtual long FastReadClusterNext(long clusterIndex)
		=> _clusters[clusterIndex].Next;

	public virtual void FastWriteClusterNext(long clusterIndex, long next) {
		var cluster = _clusters[clusterIndex];
		cluster.Next = next;
		_clusters[clusterIndex] = cluster;
	}

	public virtual byte[] FastReadClusterData(long clusterIndex, long offset, long size) {
		return _clusters[clusterIndex].Data.AsSpan(checked((int)offset), checked((int)size)).ToArray();
	}

	public virtual void FastWriteClusterData(long clusterIndex, long offset, ReadOnlySpan<byte> data) {
		var cluster = _clusters[clusterIndex];
		cluster.Data = data.ToArray();
		_clusters[clusterIndex] = cluster;
	}

	public long CalculateClusterChainLength(long byteLength) => (long)Math.Ceiling(byteLength / (double)ClusterSize);

	public string ToStringFullContents() {
		var stringBuilder = new StringBuilder();
		for (var i = 0; i < Clusters.Count; i++) {
			var cluster = Clusters[i];
			stringBuilder.AppendLine($"\t{i}: {cluster}");
		}
		return stringBuilder.ToString();
	}

	#endregion

	#region Event Notifiers

	private void NotifyClusterCountChanged(long countDelta, long? terminalValue) {
		if (SuppressEvents)
			return;

		ClusterCountChanged?.Invoke(this, countDelta, terminalValue);
	}

	private void NotifyClusterChainCreated(long startCluster, long endCluster, long clusterCount, long terminalValue) {
		if (SuppressEvents)
			return;

		ClusterChainCreated?.Invoke(this, startCluster, endCluster, clusterCount, terminalValue);
	}

	private void NotifyClusterChainStartChanged(long cluster, long terminalValue, long clusterCountDelta) {
		if (SuppressEvents)
			return;

		ClusterChainStartChanged?.Invoke(this, cluster, terminalValue, clusterCountDelta);
	}

	private void NotifyClusterChainEndChanged(long cluster, long terminalValue, long clusterCountDelta) {
		if (SuppressEvents)
			return;

		ClusterChainEndChanged?.Invoke(this, cluster, terminalValue, clusterCountDelta);
	}

	private void NotifyClusterMoved(long fromCluster, long toCluster, ClusterTraits traits, long? terminalValue) {
		if (SuppressEvents)
			return;

		ClusterMoved?.Invoke(this, fromCluster, toCluster, traits, terminalValue);
	}

	private void NotifyClusterChainRemoved(long terminalValue) {
		if (SuppressEvents)
			return;

		ClusterChainRemoved?.Invoke(this, terminalValue);
	}

	#endregion

	#region Alternative Impl

	/*
 
// The below are fully tested implementations of algorithms but where deletes are forward. since this results in high fragmentation, they are not used.


public virtual long RemoveNextClusters(long fromCluster, long quantity = long.MaxValue) {
	using (EnterWriteScope()) {
		Guard.ArgumentInRange(fromCluster, 0, _clusters.Count - 1, nameof(fromCluster));
		
		if (quantity == 0)
			return 0;
		var fromTraits = FastReadClusterTraits(fromCluster);
		var fromWasStart = fromTraits.HasFlag(ClusterTraits.First);
		var fromPrev = FastReadClusterPrev(fromCluster);  

		// Iterate through and remove clusters by tip-substitution
		var encounteredEndCluster = false;
		var clustersRemoved = 0L;
		var removeCluster = fromCluster;
		var lastRemovedNext = 0L;
		while (clustersRemoved < quantity && !encounteredEndCluster) {
			encounteredEndCluster = FastReadClusterTraits(removeCluster).HasFlag(ClusterTraits.Last);
			lastRemovedNext = FastReadClusterNext(removeCluster);
			var logicalClusterCount = _clusters.Count - clustersRemoved;
			MigrateTipClusterTo(logicalClusterCount, removeCluster);
			clustersRemoved++;
			if (!encounteredEndCluster)
				removeCluster = lastRemovedNext == logicalClusterCount - 1  ? removeCluster : lastRemovedNext;
 			}

		if (fromWasStart && encounteredEndCluster) {
			// removed entire chain, nothing to do
			// [A] -> [B] -> [C] -> [D] -> [E] 
			NotifyClusterChainRemoved(fromCluster, fromPrev, removeCluster, lastRemovedNext);
		} else if (fromWasStart && !encounteredEndCluster) {
			// removed from beginning to middle
			// [A] -> [B] -> [C] -> D -> E 
			// - set D.Traits.First = true
			// - set D.Previous = A.Previous (terminal value propagates)
			FastMaskClusterTraits(lastRemovedNext, ClusterTraits.First, true);
			FastWriteClusterPrev(lastRemovedNext, fromPrev);
			NotifyMovedStartCluster(lastRemovedNext, fromPrev);
		} else if (!fromWasStart && encounteredEndCluster) {
			// removed from middle to end
			// A -> B -> [C] -> [D] -> [E] 
			// - set B.Traits.Last = true
			// - set B.Next = E.Next (terminal value propagates)
			FastMaskClusterTraits(fromPrev, ClusterTraits.Last, true);
			FastWriteClusterNext(fromPrev, lastRemovedNext);
			NotifyMovedEndCluster(fromPrev, lastRemovedNext);
		} else if (!fromWasStart && !encounteredEndCluster) {
			// removed from middle to middle
			// A -> [B] -> [C] -> [D] -> E 
			// - set A.Next = E
			// - set E.Previous = A
			FastWriteClusterNext(fromPrev, lastRemovedNext);
			FastWriteClusterPrev(lastRemovedNext, fromPrev);
		}

		// finally remove the old clusters from collection
		_clusters.RemoveRange(_clusters.Count - clustersRemoved, clustersRemoved);
		return clustersRemoved;
	}
}

public virtual long RemoveClustersFromEnd(long endCluster, long quantity) {
	// Walk back cluster chain end and then delete forward	
	using (EnterWriteScope()) {
		Guard.ArgumentInRange(endCluster, 0, _clusters.Count - 1, nameof(endCluster));
		Guard.Argument(FastReadClusterTraits(endCluster).HasFlag(ClusterTraits.Last), nameof(endCluster), "Must be the end cluster of a cluster chain");

		if (quantity == 0)
			return 0;

		var fromCluster = endCluster;
		var toRemove = 1L;
		while(!FastReadClusterTraits(fromCluster).HasFlag(ClusterTraits.First) && toRemove < quantity ) {
			fromCluster = FastReadClusterPrev(fromCluster);
			toRemove++;
		}
		return RemoveNextClusters(fromCluster, toRemove);
	}

}

*/
	#endregion

}

internal class ClusterMap : ClusterMap<IExtendedList<Cluster>> {
	public ClusterMap(IExtendedList<Cluster> clusters, int clusterSize)
		: base(clusters, clusterSize) {
	}

}
