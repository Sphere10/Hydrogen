using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Used to track a (very large) logical sequence of clusters within a <see cref="ClusterContainer{TInner}"/>.
/// </summary>
internal class ClusterSeeker : IDisposable {
	// TODO: optimizations include:
	// - Intelligent memoization of LogicalCluster -> Cluster. Algorithm should strive for equidistant memoization along chain, eg. max 1024
	//   this would minimize seek-time on reused streams.

	private readonly IClusterContainer _clusters;

	public ClusterSeeker(IClusterContainer clusteredContainer, long startCluster, long endCluster, long totalClusters, long terminalValue) {
		_clusters = clusteredContainer;
		// The seeker needs to be reset whenever a start/end cluster in the cluster chain is changed
		_clusters.ClusterChainCreated += ClusteredChainCreatedHandler;
		_clusters.ClusterChainStartChanged += ClustereChainStartChangedHandler;
		_clusters.ClusterChainEndChanged += ClusterChainEndChangedHandler;
		_clusters.ClusterMoved += ClusterMovedHandler;
		_clusters.ClusterChainRemoved += ClusterChainRemovedHandler;
		Reset(startCluster, endCluster, totalClusters, terminalValue, true);
	}

	public void Reset(long startCluster, long endCluster, long totalClusters, long terminalValue, bool resetCurrentPointer) {
		#if ENABLE_CLUSTER_DIAGNOSTICS
		#warning Below can lead to false positive errors
		// NOTE: This can lead to false-positive errors since in the middle of removing clusters from a chain,
		// the clusters are in an invalid state an a migration may raise an event which leads to seeker reset
		// and this catches the temporarily invalid state. ClusterDiagnostics.
		VerifySeekResetStreamOptimized(_clusters, startCluster, endCluster, totalClusters, terminalValue, resetCurrentPointer ? null : CurrentCluster, resetCurrentPointer ? null : CurrentLogicalCluster, resetCurrentPointer ? null : CurrentTraits, new HashSet<long>());
		#endif

		StartCluster = startCluster;
		EndCluster = endCluster;
		TotalClusters = totalClusters;
		TerminalValue = terminalValue;

		if (resetCurrentPointer) {
			CurrentCluster = null;
			CurrentLogicalCluster = null;
			CurrentTraits = null;
		}

	}

	public long StartCluster { get; private set; }

	public long EndCluster { get; private set; }

	public long TerminalValue { get; private set; }

	public long TotalClusters { get; private set; }

	public long? CurrentCluster { get; private set; }

	public long? CurrentLogicalCluster { get; private set; }

	public ClusterTraits? CurrentTraits { get; private set; }

	public void SeekStart() {
		Guard.Ensure(StartCluster >= 0, $"Start cluster {StartCluster} is not defined for empty cluster chain");
		CurrentCluster = StartCluster;
		CurrentLogicalCluster = 0;
		CurrentTraits = _clusters.FastReadClusterTraits(CurrentCluster.Value);
		Guard.Ensure(CurrentTraits.Value.HasFlag(ClusterTraits.Start), $"Start cluster {StartCluster} is not a start cluster");
	}

	public void SeekEnd() {
		Guard.Ensure(EndCluster >= 0, $"Last cluster {EndCluster} is not defined for empty cluster chain");
		CurrentCluster = EndCluster;
		CurrentLogicalCluster = TotalClusters - 1;
		CurrentTraits = _clusters.FastReadClusterTraits(CurrentCluster.Value);
		Guard.Ensure(CurrentTraits.Value.HasFlag(ClusterTraits.End), $"End cluster {CurrentCluster.Value} was not marked end");
	}

	public void SeekTo(long logicalCluster) {
		Guard.Ensure(StartCluster != -1, "Seeker parameters have not been set");
		FindShortestPathToLogicalCluster(logicalCluster, out var startCluster, out var steps, out var direction);
		if (startCluster == StartCluster) {
			SeekStart();
		} else if (startCluster == EndCluster) {
			SeekEnd();
		} else if (startCluster == CurrentCluster) {
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
		for (var i = 0; i < steps; i++) {
			if (CurrentCluster == null) {
				CurrentCluster = StartCluster;
				CurrentLogicalCluster = 0;
				CurrentTraits = _clusters.FastReadClusterTraits(CurrentCluster.Value);
			} else {
				Guard.Against(CurrentTraits.Value.HasFlag(ClusterTraits.End), "Cannot walk past the last cluster on cluster chain");
				var nextCluster = _clusters.FastReadClusterNext(CurrentCluster.Value);
				if (walkedClusters.Contains(nextCluster))
					throw new CorruptDataException($"Cyclic dependency detected (cluster {CurrentCluster.Value} has cyclic next {nextCluster})");
				CurrentCluster = nextCluster;
				CurrentTraits = _clusters.FastReadClusterTraits(nextCluster);
				CurrentLogicalCluster++;
			}
			walkedClusters.Add(CurrentCluster.Value);
		}
	}

	public void SeekBackwards(long steps) {
		var walkedClusters = new HashSet<long>();
		for (var i = 0; i < steps; i++) {
			if (CurrentCluster == null) {
				CurrentCluster = EndCluster;
				CurrentLogicalCluster = TotalClusters - 1;
				CurrentTraits = _clusters.FastReadClusterTraits(CurrentCluster.Value);
			} else {
				Guard.Against(CurrentTraits.Value.HasFlag(ClusterTraits.Start), "Cannot walk before the first cluster on cluster chain");
				var prevCluster = _clusters.FastReadClusterPrev(CurrentCluster.Value);
				if (walkedClusters.Contains(prevCluster))
					throw new CorruptDataException($"Cyclic dependency detected (cluster {CurrentCluster.Value} has cyclic prev {prevCluster})");
				CurrentCluster = prevCluster;
				CurrentTraits = _clusters.FastReadClusterTraits(prevCluster);
				CurrentLogicalCluster--;
			}
			walkedClusters.Add(CurrentCluster.Value);
		}
	}


	protected void FindShortestPathToLogicalCluster(long logicalCluster, out long closestKnownCluster, out long steps, out IterateDirection seekDirection) {
		CheckNotEmpty();
		var paths = new List<(long FromKnownCluster, long Steps, IterateDirection Dir)>();

		// Consider path from start
		CalculatePathToFragment(0, logicalCluster, TotalClusters, out var steps_, out var dir);
		paths.Add((StartCluster, steps_, dir));

		// Consider path from end
		CalculatePathToFragment(TotalClusters - 1, logicalCluster, TotalClusters, out steps_, out dir);
		paths.Add((EndCluster, steps_, dir));

		// Consider path from current position
		if (CurrentLogicalCluster.HasValue) {
			CalculatePathToFragment(CurrentLogicalCluster.Value, logicalCluster, TotalClusters, out steps_, out dir);
			paths.Add((CurrentCluster.Value, steps_, dir));
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
		_clusters.ClusterChainStartChanged -= ClustereChainStartChangedHandler;
		_clusters.ClusterChainEndChanged -= ClusterChainEndChangedHandler;
		_clusters.ClusterMoved -= ClusterMovedHandler;
		_clusters.ClusterChainRemoved -= ClusterChainRemovedHandler;
	}

	private void CheckNotEmpty() => Guard.Ensure(StartCluster >= 0 && EndCluster >= 0, "Cluster chain is empty");

	private void ClusteredChainCreatedHandler(object sender, long startCluster, long endCluster, long totalClusters, long terminalValue) {
		if (terminalValue == TerminalValue) {
			Reset(startCluster, endCluster, totalClusters, terminalValue, true);
		}
	}

	private void ClusterChainEndChangedHandler(object sender, long cluster,  long terminalValue, long clusterCountDelta) {
		if (terminalValue == TerminalValue) {
			Reset(StartCluster, cluster, TotalClusters + clusterCountDelta, terminalValue, true);
		}
	}

	private void ClusterMovedHandler(object sender, long fromCluster, long toCluster, ClusterTraits traits, long? terminalValue) {
		if (fromCluster == StartCluster) {
			Reset(toCluster, EndCluster, TotalClusters, TerminalValue, true);
		} else if (fromCluster == EndCluster) {
			Reset(StartCluster, toCluster, TotalClusters, TerminalValue, true);
		} else if (fromCluster == CurrentCluster) {
			Reset(StartCluster, EndCluster, TotalClusters, TerminalValue, true);
		}
	}

	private void ClustereChainStartChangedHandler(object sender, long cluster,  long terminalValue, long clusterCountDelta) {
		if (terminalValue == TerminalValue) {
			Reset(cluster, EndCluster, TotalClusters + clusterCountDelta, terminalValue, true);
		}
	}

	private void ClusterChainRemovedHandler(object sender, long terminalValue) {
		if (terminalValue == TerminalValue) {
			Reset(-1, -1, 0, terminalValue, true);
		}
	}
}
