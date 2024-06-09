// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Used to track a (very large) logical sequence of clusters within a <see cref="ClusterMap"/>.
/// </summary>
internal class ClusterSeeker  {
	// TODO: optimizations include:
	// - Intelligent memoization of LogicalCluster -> Cluster. Algorithm should strive for equidistant memoization along chain, eg. max 1024
	//   this would minimize seek-time on reused streams.

	private readonly ClusterMap _clusters;
	private readonly bool _integrityChecks;

	public ClusterSeeker(ClusterMap clusteredMap, long terminalValue, long startCluster, long endCluster, long totalClusters, bool integrityChecks) {
		_clusters = clusteredMap;
		_integrityChecks = integrityChecks;
		TerminalValue = terminalValue;
		Pointer = new ClusterPointer {
				Chain = new ClusterChain {
					StartCluster = startCluster,
					EndCluster = endCluster,
					TotalClusters = totalClusters
				},
				CurrentCluster = startCluster,
				CurrentIndex = startCluster != Cluster.Null ? 0 : Cluster.Null,
		};
		Pointer.CurrentTraits = CalculateCurrentTraits(); // method uses Pointer state, don't call within object initializer 
	}

	public ClusterPointer Pointer { get; }

	public long TerminalValue { get; private set; }

	public void SeekStart() {
		if (_integrityChecks) {
			Guard.Ensure(Pointer.Chain.StartCluster >= 0, $"Start cluster {Pointer.Chain.StartCluster} is not defined for empty cluster chain");
		}
		Pointer.CurrentCluster = Pointer.Chain.StartCluster;
		Pointer.CurrentIndex = 0;
		Pointer.CurrentTraits = CalculateCurrentTraits();
		if (_integrityChecks) {
			Pointer.CheckTraits();
			Guard.Ensure(Pointer.CurrentTraits.HasFlag(ClusterTraits.Start), $"Start cluster {Pointer.Chain.StartCluster} is not a start cluster");
		}
	}

	public void SeekEnd() {
		if (_integrityChecks) {
			Guard.Ensure(Pointer.Chain.EndCluster >= 0, $"Last cluster {Pointer.Chain.EndCluster} is not defined for empty cluster chain");
		}
		Pointer.CurrentCluster = Pointer.Chain.EndCluster;
		Pointer.CurrentIndex = Pointer.Chain.TotalClusters - 1;
		Pointer.CurrentTraits = CalculateCurrentTraits();
		if (_integrityChecks) {
			Pointer.CheckTraits();
			Guard.Ensure(Pointer.CurrentTraits.HasFlag(ClusterTraits.End), $"End cluster {Pointer.CurrentCluster} was not marked end");
		}
	}

	public void SeekTo(long logicalCluster) {
		if (_integrityChecks) {
			Guard.Ensure(Pointer.Chain.StartCluster >= 0, "Cannot seek as there are no clusters to seek");
		}
		FindShortestPathToLogicalCluster(logicalCluster, out var startCluster, out var steps, out var direction);
		if (startCluster == Pointer.Chain.StartCluster) {
			SeekStart();
		} else if (startCluster == Pointer.Chain.EndCluster) {
			SeekEnd();
		} else if (startCluster == Pointer.CurrentCluster) {
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
		if (_integrityChecks) {
			Guard.Ensure(Pointer.Chain.StartCluster >= 0, "Cannot seek as there are no clusters to seek");
		}
		var walkedClusters = new HashSet<long> {
			Pointer.CurrentCluster
		};
		for (var i = 0; i < steps; i++) {
			Guard.Against(Pointer.CurrentTraits.HasFlag(ClusterTraits.End), $"Cannot walk past the last cluster on cluster chain ({steps - i} steps to go)");
			var nextCluster = _clusters.ReadClusterNext(Pointer.CurrentCluster);
			Guard.Ensure(!walkedClusters.Contains(nextCluster), $"Cyclic dependency detected (cluster {Pointer.CurrentCluster} has cyclic next {nextCluster})");
			Pointer.CurrentCluster = nextCluster;
			Pointer.CurrentIndex++;
			Pointer.CurrentTraits = CalculateCurrentTraits();
			if (_integrityChecks) {
				Pointer.CheckTraits();
			}
			walkedClusters.Add(Pointer.CurrentCluster);
		}
	}

	public void SeekBackwards(long steps) {
		Guard.Ensure(Pointer.Chain.StartCluster >= 0, "Cannot seek as there are no clusters to seek");
		var walkedClusters = new HashSet<long> {
			Pointer.CurrentCluster
		};
		for (var i = 0; i < steps; i++) {
			Guard.Against(Pointer.CurrentTraits.HasFlag(ClusterTraits.Start), "Cannot walk before the first cluster on cluster chain");
			var prevCluster = _clusters.ReadClusterPrev(Pointer.CurrentCluster);
			Guard.Ensure(!walkedClusters.Contains(prevCluster), $"Cyclic dependency detected (cluster {Pointer.CurrentCluster} has cyclic prev {prevCluster})");
			Pointer.CurrentCluster = prevCluster;
			Pointer.CurrentIndex--;
			Pointer.CurrentTraits = CalculateCurrentTraits();
			if (_integrityChecks) {
				Pointer.CheckTraits();
			}
			walkedClusters.Add(Pointer.CurrentCluster);
		}
	}

	public static void CalculatePathToLogicalCluster(long fromLogicalCluster, long toLogicalCluster, long totalClusters, out long steps, out IterateDirection seekDirection) {
		Guard.Ensure(totalClusters >= 0, "No path exists since no clusters");
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

	public void ProcessClusterMapChanged(ClusterMapChangedEventArgs changedEvent) {
		// Process changes arising from tip migrations (no size changed here)
		if (changedEvent.MovedClusters.TryGetValue(Pointer.Chain.StartCluster, out var newStart)) 
			Pointer.Chain.StartCluster = newStart;

		if (changedEvent.MovedClusters.TryGetValue(Pointer.Chain.EndCluster, out var newEnd)) 
			Pointer.Chain.EndCluster = newEnd;
		
		if (changedEvent.MovedClusters.TryGetValue(Pointer.CurrentCluster, out var newCurrent)) 
			Pointer.CurrentCluster = newCurrent;
		
		// Process changes arising from this chain's resizing
		if (changedEvent.ChainTerminal == TerminalValue) {
			if (changedEvent.AddedChain) {
				Pointer.Chain.TotalClusters = changedEvent.ClusterCountDelta;
				Pointer.Chain.StartCluster = changedEvent.ChainNewStartCluster.Value;
				Pointer.Chain.EndCluster = changedEvent.ChainNewEndCluster.Value;
				Pointer.CurrentCluster = Pointer.Chain.StartCluster;
				Pointer.CurrentIndex = 0;
				// Ensure traits correct
				Pointer.CurrentTraits = CalculateCurrentTraits();
			} else if (changedEvent.RemovedChain) {
				Pointer.Chain.TotalClusters = 0;
				Pointer.Chain.StartCluster = Cluster.Null;
				Pointer.Chain.EndCluster = Cluster.Null;
				Pointer.CurrentCluster = Cluster.Null;
				Pointer.CurrentIndex = Cluster.Null;
			} else if (changedEvent.IncreasedChainSize || changedEvent.DecreasedChainSize) {
				Pointer.Chain.TotalClusters += changedEvent.ClusterCountDelta;
				Pointer.Chain.EndCluster = changedEvent.ChainNewEndCluster.Value;

				// current pointer potentially wiped out by resize down
				if (Pointer.CurrentIndex >= Pointer.Chain.TotalClusters) {
					Pointer.CurrentCluster = Pointer.Chain.StartCluster;
					Pointer.CurrentIndex =  Pointer.Chain.StartCluster != Cluster.Null ? 0 : Cluster.Null;
				}
				// Ensure traits correct
				Pointer.CurrentTraits = CalculateCurrentTraits();
			}
		}
	}

	public void ProcessStreamSwapped(long record1Index, ClusteredStreamDescriptor descriptor1Data, long record2Index, ClusteredStreamDescriptor descriptor2Data) {
		if (TerminalValue == record1Index) {
			TerminalValue = record2Index;
		}
		else if (TerminalValue == record2Index) {
			TerminalValue = record1Index;
		}
	}

	protected void FindShortestPathToLogicalCluster(long logicalCluster, out long closestKnownCluster, out long steps, out IterateDirection seekDirection) {
		CheckNotEmpty();
		var paths = new List<(long FromKnownCluster, long Steps, IterateDirection Dir)>();

		// Consider path from start
		CalculatePathToLogicalCluster(0, logicalCluster, Pointer.Chain.TotalClusters, out var steps_, out var dir);
		paths.Add((Pointer.Chain.StartCluster, steps_, dir));

		// Consider path from end
		CalculatePathToLogicalCluster(Pointer.Chain.TotalClusters - 1, logicalCluster, Pointer.Chain.TotalClusters, out steps_, out dir);
		paths.Add((Pointer.Chain.EndCluster, steps_, dir));

		// Consider path from current position
		CalculatePathToLogicalCluster(Pointer.CurrentIndex, logicalCluster, Pointer.Chain.TotalClusters, out steps_, out dir);
		paths.Add((Pointer.CurrentCluster, steps_, dir));

		// TODO: future optimizations can intermittently remember cluster positions as walks (say 1024 positions, equidistant)

		var result = paths.OrderBy(x => x.Steps).First();
		closestKnownCluster = result.FromKnownCluster;
		steps = result.Steps;
		seekDirection = result.Dir;
	}

	private ClusterTraits CalculateCurrentTraits() {
		var traits = ClusterTraits.None;
		traits.SetFlags(ClusterTraits.Start, Pointer.Chain.StartCluster != Cluster.Null &&  Pointer.CurrentCluster == Pointer.Chain.StartCluster);
		traits.SetFlags(ClusterTraits.End, Pointer.Chain.EndCluster != Cluster.Null && Pointer.CurrentCluster == Pointer.Chain.EndCluster);
		return traits;
	}

	private void CheckNotEmpty() => Guard.Ensure(Pointer.Chain.StartCluster >= 0 && Pointer.Chain.EndCluster >= 0, "Cluster chain is empty");

	public class ClusterChain {
		public long StartCluster;
		public long EndCluster;
		public long TotalClusters;
	}

	internal class ClusterPointer {
		public ClusterChain Chain;
		public long CurrentIndex;
		public long CurrentCluster;
		public ClusterTraits CurrentTraits;

		public void CheckTraits() {
			if (CurrentTraits.HasFlag(ClusterTraits.Start))
				Guard.Ensure(CurrentIndex == 0, $"Start cluster is START but has index {CurrentIndex}");
			if (CurrentTraits.HasFlag(ClusterTraits.End))
				Guard.Ensure(CurrentIndex == Chain.TotalClusters - 1, $"End cluster {CurrentCluster} is END but index {CurrentIndex} does not match chain total clusters {Chain.TotalClusters}");
			if (CurrentIndex == 0 && !CurrentTraits.HasFlag(ClusterTraits.Start))
				Guard.Ensure(CurrentTraits.HasFlag(ClusterTraits.Start), $"Cluster {CurrentCluster} index is 0 but traits {CurrentTraits} do not marked START");
			if (CurrentIndex == Chain.TotalClusters - 1 && !CurrentTraits.HasFlag(ClusterTraits.End))
				Guard.Ensure(CurrentTraits.HasFlag(ClusterTraits.Start), $"Cluster index {CurrentTraits} is end of chain but not marked END");
		}
	}
}
