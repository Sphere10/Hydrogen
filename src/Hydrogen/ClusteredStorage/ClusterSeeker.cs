using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hydrogen;

public class ClusterChain {
	public long StartCluster;
	public long EndCluster;
	public long TotalClusters;
}

internal class ClusterPointer {
	public ClusterChain Chain;
	public long? CurrentIndex;
	public long? CurrentCluster;
	public ClusterTraits? CurrentTraits;

	public void CheckTraits() {
		Guard.Ensure(CurrentTraits.HasValue, "CurrentTraits is null");
		Guard.Ensure(CurrentIndex.HasValue, "CurrentIndex is null");
		Guard.Ensure(CurrentCluster.HasValue, "CurrentIndex is null");
		if (CurrentTraits.Value.HasFlag(ClusterTraits.Start))
			Guard.Ensure(CurrentIndex.Value == 0, $"Start cluster is START but has index {CurrentIndex.Value}");
		if (CurrentTraits.Value.HasFlag(ClusterTraits.End))
			Guard.Ensure(CurrentIndex.Value == Chain.TotalClusters - 1, $"End cluster {CurrentCluster} is END but index {CurrentIndex} does not match chain total clusters {Chain.TotalClusters}");
		if (CurrentIndex == 0 && !CurrentTraits.Value.HasFlag(ClusterTraits.Start))
			Guard.Ensure(CurrentTraits.Value.HasFlag(ClusterTraits.Start), $"Cluster {CurrentCluster} index is 0 but traits {CurrentTraits.Value} do not marked START");
		if (CurrentIndex == Chain.TotalClusters -1 && !CurrentTraits.Value.HasFlag(ClusterTraits.End))
			Guard.Ensure(CurrentTraits.Value.HasFlag(ClusterTraits.Start), $"Cluster index {CurrentTraits.Value} is end of chain but not marked END");

	}
}

/// <summary>
/// Used to track a (very large) logical sequence of clusters within a <see cref="ClusterContainer{TInner}"/>.
/// </summary>
internal class ClusterSeeker : IDisposable {
	// TODO: optimizations include:
	// - Intelligent memoization of LogicalCluster -> Cluster. Algorithm should strive for equidistant memoization along chain, eg. max 1024
	//   this would minimize seek-time on reused streams.

	private readonly IClusterContainer _clusters;

	public ClusterSeeker(IClusterContainer clusteredContainer, long terminalValue, Func<ClusterChain> chainLookup) {
		_clusters = clusteredContainer;
		// The seeker needs to be reset whenever a start/end cluster in the cluster chain is changed
		_clusters.ClusterChainCreated += ClusteredChainCreatedHandler;
		_clusters.ClusterChainStartChanged += ClusteredChainStartChangedHandler;
		_clusters.ClusterChainEndChanged += ClusterChainEndChangedHandler;
		_clusters.ClusterMoved += ClusterMovedHandler;
		_clusters.ClusterChainRemoved += ClusterChainRemovedHandler;
		TerminalValue = terminalValue;
		ClusterPointer = new Reloadable<ClusterPointer>(() => {
			var chain = chainLookup();
			//if (chain.StartCluster >= 0)
			//	Guard.Ensure(_clusters.FastReadClusterPrev(chain.StartCluster) == terminalValue, $"Start cluster {chain.StartCluster} did not point to expected terminal value");
			//if (chain.EndCluster >= 0)
			//	Guard.Ensure(_clusters.FastReadClusterNext(chain.EndCluster) == terminalValue, $"End cluster {chain.EndCluster} did not point to expected terminal value");
			return new ClusterPointer {
				Chain = chain,
				CurrentIndex = null,
				CurrentCluster = null,
				CurrentTraits = null
			};
		});
	}

	public Reloadable<ClusterPointer> ClusterPointer { get; }

	public long TerminalValue { get; private set; }

	public void SeekStart() {
		var ptr = ClusterPointer.Value;
		Guard.Ensure(ptr.Chain.StartCluster >= 0, $"Start cluster {ptr.Chain.StartCluster} is not defined for empty cluster chain");
		ptr.CurrentCluster = ptr.Chain.StartCluster;
		ptr.CurrentIndex = 0;
		ptr.CurrentTraits = _clusters.FastReadClusterTraits(ptr.CurrentCluster.Value);
		ptr.CheckTraits();
		Guard.Ensure(ptr.CurrentTraits.Value.HasFlag(ClusterTraits.Start), $"Start cluster {ptr.Chain.StartCluster} is not a start cluster");
	}

	public void SeekEnd() {
		var ptr = ClusterPointer.Value;
		Guard.Ensure(ptr.Chain.EndCluster >= 0, $"Last cluster {ptr.Chain.EndCluster} is not defined for empty cluster chain");
		ptr.CurrentCluster = ptr.Chain.EndCluster;
		ptr.CurrentIndex = ptr.Chain.TotalClusters - 1;
		ptr.CurrentTraits = _clusters.FastReadClusterTraits(ptr.CurrentCluster.Value);
		ptr.CheckTraits();
		Guard.Ensure(ptr.CurrentTraits.Value.HasFlag(ClusterTraits.End), $"End cluster {ptr.CurrentCluster.Value} was not marked end");
	}

	public void SeekTo(long logicalCluster) {
		var ptr = ClusterPointer.Value;
		Guard.Ensure(ptr.Chain.StartCluster >= 0, "Cannot seek as there are no clusters to seek");
		FindShortestPathToLogicalCluster(logicalCluster, out var startCluster, out var steps, out var direction);
		if (startCluster == ptr.Chain.StartCluster) {
			SeekStart();
		} else if (startCluster == ptr.Chain.EndCluster) {
			SeekEnd();
		} else if (startCluster == ptr.CurrentCluster) {
			// already at start point
		} else {
			throw new InvalidOperationException($"Unknown index to start cluster seek from: {startCluster}");
		}

		switch (direction) {
			case IterateDirection.LeftToRight:
				SeekForward(steps);
				break;
			case IterateDirection.RightToLeft:
				SeekBackwards(steps);
				break;
			default:
				throw new NotSupportedException($"{direction}");
		}
	}

	public void SeekForward(long steps) {
		var walkedClusters = new HashSet<long>();
		var ptr = ClusterPointer.Value;
		if (ptr.CurrentCluster == null) {
			ptr.CurrentCluster = ptr.Chain.StartCluster;
			ptr.CurrentIndex = 0;
			ptr.CurrentTraits = _clusters.FastReadClusterTraits(ptr.CurrentCluster.Value);
			ptr.CheckTraits();
		}
		walkedClusters.Add(ptr.CurrentCluster.Value);
		for (var i = 0; i < steps; i++) {
			Guard.Against(ptr.CurrentTraits.Value.HasFlag(ClusterTraits.End), $"Cannot walk past the last cluster on cluster chain ({steps - i} steps to go)");
			var nextCluster = _clusters.FastReadClusterNext(ptr.CurrentCluster.Value);
			if (walkedClusters.Contains(nextCluster))
				throw new CorruptDataException($"Cyclic dependency detected (cluster {ptr.CurrentCluster.Value} has cyclic next {nextCluster})");
			ptr.CurrentCluster = nextCluster;
			ptr.CurrentTraits = _clusters.FastReadClusterTraits(nextCluster);
			ptr.CurrentIndex++;
			ptr.CheckTraits();
			walkedClusters.Add(ptr.CurrentCluster.Value);
		}
	}

	public void SeekBackwards(long steps) {
		var walkedClusters = new HashSet<long>();
		var ptr = ClusterPointer.Value;
		if (ptr.CurrentCluster == null) {
			ptr.CurrentCluster = ptr.Chain.EndCluster;
			ptr.CurrentIndex = ptr.Chain.TotalClusters - 1;
			ptr.CurrentTraits = _clusters.FastReadClusterTraits(ptr.CurrentCluster.Value);
			ptr.CheckTraits();
		}
		walkedClusters.Add(ptr.CurrentCluster.Value);
		for (var i = 0; i < steps; i++) {
			Guard.Against(ptr.CurrentTraits.Value.HasFlag(ClusterTraits.Start), "Cannot walk before the first cluster on cluster chain");
			var prevCluster = _clusters.FastReadClusterPrev(ptr.CurrentCluster.Value);
			if (walkedClusters.Contains(prevCluster))
				throw new CorruptDataException($"Cyclic dependency detected (cluster {ptr.CurrentCluster.Value} has cyclic prev {prevCluster})");
			ptr.CurrentCluster = prevCluster;
			ptr.CurrentTraits = _clusters.FastReadClusterTraits(prevCluster);
			ptr.CurrentIndex--;
			ptr.CheckTraits();
			walkedClusters.Add(ptr.CurrentCluster.Value);
		}
	}

	protected void FindShortestPathToLogicalCluster(long logicalCluster, out long closestKnownCluster, out long steps, out IterateDirection seekDirection) {
		CheckNotEmpty();
		var paths = new List<(long FromKnownCluster, long Steps, IterateDirection Dir)>();

		// Consider path from start
		CalculatePathToFragment(0, logicalCluster, ClusterPointer.Value.Chain.TotalClusters, out var steps_, out var dir);
		paths.Add((ClusterPointer.Value.Chain.StartCluster, steps_, dir));

		// Consider path from end
		CalculatePathToFragment(ClusterPointer.Value.Chain.TotalClusters - 1, logicalCluster, ClusterPointer.Value.Chain.TotalClusters, out steps_, out dir);
		paths.Add((ClusterPointer.Value.Chain.EndCluster, steps_, dir));

		// Consider path from current position
		if (ClusterPointer.Value.CurrentIndex.HasValue) {
			CalculatePathToFragment(ClusterPointer.Value.CurrentIndex.Value, logicalCluster, ClusterPointer.Value.Chain.TotalClusters, out steps_, out dir);
			paths.Add((ClusterPointer.Value.CurrentCluster.Value, steps_, dir));
		}

		// TODO: future optimizations can intermittenly remember cluster positions as walks (say 1024 positions, equidistant)

		var result = paths.OrderBy(x => x.Steps).First();
		closestKnownCluster = result.FromKnownCluster;
		steps = result.Steps;
		seekDirection = result.Dir;
	}

	public static void CalculatePathToFragment(long fromLogicalCluster, long toLogicalCluster, long totalClusters, out long steps, out IterateDirection seekDirection) {
		var lastClusterIX = totalClusters - 1;
		Guard.ArgumentInRange(fromLogicalCluster, 0, lastClusterIX, nameof(fromLogicalCluster));
		Guard.ArgumentInRange(toLogicalCluster, 0, lastClusterIX, nameof(toLogicalCluster));

		if (fromLogicalCluster == toLogicalCluster) {
			steps = 0;
			seekDirection = IterateDirection.LeftToRight;
		} else if (fromLogicalCluster < toLogicalCluster) {
			steps = toLogicalCluster - fromLogicalCluster;
			seekDirection = IterateDirection.LeftToRight;
		} else {
			steps = fromLogicalCluster - toLogicalCluster;
			seekDirection = IterateDirection.RightToLeft;
		}
	}

	public void Dispose() {
		_clusters.ClusterChainCreated -= ClusteredChainCreatedHandler;
		_clusters.ClusterChainStartChanged -= ClusteredChainStartChangedHandler;
		_clusters.ClusterChainEndChanged -= ClusterChainEndChangedHandler;
		_clusters.ClusterMoved -= ClusterMovedHandler;
		_clusters.ClusterChainRemoved -= ClusterChainRemovedHandler;
	}

	private void CheckNotEmpty() => Guard.Ensure(ClusterPointer.Value.Chain.StartCluster >= 0 && ClusterPointer.Value.Chain.EndCluster >= 0, "Cluster chain is empty");

	private void ClusteredChainCreatedHandler(object sender, long startCluster, long endCluster, long totalClusters, long terminalValue) {
		if (terminalValue == TerminalValue) {
			ClusterPointer.Invalidate();
		}
	}
	private void ClusterChainEndChangedHandler(object sender, long cluster, long terminalValue, long clusterCountDelta) {

		if (terminalValue == TerminalValue) {
			if (ClusterPointer.Loaded && clusterCountDelta > 0) {
				// IMPORTANT: At this point in the handler, the clusters may not be updated completely yet. This is needed when record stream is being written, it may
				// allocate new clusters along the way (extending end) but we can't know the total clusters until Header.RecordCount is updated. This keeps the seeker UTD.
				// 
				// Also for general optimization. If appending to the end of the chain, we avoid invalidating pointer to prevent re-seeking to current position
				// This allows appending to a stream without reseeking after every append
				ClusterPointer.Value.Chain.TotalClusters += clusterCountDelta;
				ClusterPointer.Value.Chain.EndCluster = cluster;
				//ClusterPointer.Value.CurrentTraits = _clusters.FastReadClusterTraits(ClusterPointer.Value.CurrentIndex.Value);
				
				if (ClusterPointer.Value.CurrentTraits.HasValue) {
					// Note: the END state is inferred even though it may not be written that way yet. The cluster should 
					// be END after all handlers during migration of block are executed.
					ClusterPointer.Value.CurrentTraits = ClusterPointer.Value.CurrentTraits.Value.CopyAndSetFlags(ClusterTraits.End, false);  // remove any end trait if applicable
				}

				//var pathLen = ClusterDiagnostics.VerifyPath(_clusters.Clusters.ToArray(), ClusterPointer.Value.Chain.StartCluster,  ClusterPointer.Value.Chain.EndCluster, TerminalValue, new HashSet<long>());

			} else {
				// every other case, just invalidate
				ClusterPointer.Invalidate();
			}
		}
	}

	private void ClusterMovedHandler(object sender, long fromCluster, long toCluster, ClusterTraits traits, long? terminalValue) {
		// Rules here are simple, if moved cluster affects any part of the pointer then just invalidate it
		if (ClusterPointer.Loaded) {
			if (fromCluster == ClusterPointer.Value.Chain.StartCluster || fromCluster == ClusterPointer.Value.Chain.EndCluster || fromCluster == ClusterPointer.Value.CurrentCluster ||
			    toCluster == ClusterPointer.Value.Chain.StartCluster || toCluster == ClusterPointer.Value.Chain.EndCluster || toCluster == ClusterPointer.Value.CurrentCluster) {
				ClusterPointer.Invalidate();
			}
		}
	}

	private void ClusteredChainStartChangedHandler(object sender, long cluster, long terminalValue, long clusterCountDelta) {
		throw new NotSupportedException("Cluster chain start moved");
	}

	private void ClusterChainRemovedHandler(object sender, long terminalValue) {
		if (terminalValue == TerminalValue) {
			ClusterPointer.Invalidate();
		}
	}


}
