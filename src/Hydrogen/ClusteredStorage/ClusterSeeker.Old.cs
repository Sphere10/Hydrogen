//using System;
//using System.Collections.Generic;
//using System.ComponentModel.Design;
//using System.Linq;

//namespace Hydrogen;

///// <summary>
///// Used to track a (very large) logical sequence of clusters within a <see cref="ClusterMap{TInner}"/>.
///// </summary>
//internal class ClusterSeeker : IDisposable {
//	// TODO: optimizations include:
//	// - Intelligent memoization of LogicalCluster -> Cluster. Algorithm should strive for equidistant memoization along chain, eg. max 1024
//	//   this would minimize seek-time on reused streams.

//	private readonly IClusterMap _clusters;

//	public ClusterSeeker(IClusterMap clusteredMap, long startCluster, long endCluster, long totalClusters, long terminalValue) {
//		_clusters = clusteredMap;
//		// The seeker needs to be reset whenever a start/end cluster in the cluster chain is changed
//		_clusters.ClusterChainCreated += ClusteredChainCreatedHandler;
//		_clusters.ClusterChainStartChanged += ClusteredChainStartChangedHandler;
//		_clusters.ClusterChainEndChanged += ClusterChainEndChangedHandler;
//		_clusters.ClusterMoved += ClusterMovedHandler;
//		_clusters.ClusterChainRemoved += ClusterChainRemovedHandler;
//		Reset(startCluster, endCluster, totalClusters, terminalValue, null, null, null);
//	}

//	public void Reset(long startCluster, long endCluster, long totalClusters, long terminalValue, long? currentCluster, long? currentLogicalCluster, ClusterTraits? currentLogicalTraits) {

//		if (startCluster >= 0)
//			Guard.Ensure(_clusters.FastReadClusterPrev(startCluster) == terminalValue, $"Start cluster {startCluster} did not point to expected terminal value");
//		if (endCluster >= 0)
//			Guard.Ensure(_clusters.FastReadClusterNext(endCluster) == terminalValue, $"End cluster {endCluster} did not point to expected terminal value");

//		if (currentCluster != null)
//			Guard.Ensure(_clusters.FastReadClusterTraits(currentCluster.Value) == currentLogicalTraits, $"Current cluster {currentCluster} did not have expected traits {currentLogicalTraits}");

//#if ENABLE_CLUSTER_DIAGNOSTICS

//#warning Below can lead to false positive errors
//		// NOTE: This can lead to false-positive errors since in the middle of removing clusters from a chain,
//		// the clusters are in an invalid state an a migration may raise an event which leads to seeker reset
//		// and this catches the temporarily invalid state. ClusterDiagnostics.
//		VerifySeekResetStreamOptimized(_clusters, startCluster, endCluster, totalClusters, terminalValue, resetCurrentPointer ? null : CurrentCluster, resetCurrentPointer ? null : CurrentLogicalCluster, resetCurrentPointer ? null : CurrentTraits, new HashSet<long>());
//#endif

//		StartCluster = startCluster;
//		EndCluster = endCluster;
//		TotalClusters = totalClusters;
//		TerminalValue = terminalValue;

//		CurrentCluster = currentCluster;
//		CurrentLogicalCluster = currentLogicalCluster;
//		CurrentTraits = currentLogicalTraits;
//		if (CurrentCluster.HasValue && !CurrentTraits.HasValue)
//			CurrentTraits = _clusters.FastReadClusterTraits(CurrentCluster.Value);

//	}

//	public long StartCluster { get; private set; }

//	public long EndCluster { get; private set; }

//	public long TerminalValue { get; private set; }

//	public long TotalClusters { get; private set; }

//	public long? CurrentCluster { get; private set; }

//	public long? CurrentLogicalCluster { get; private set; }

//	public ClusterTraits? CurrentTraits { get; private set; }

//	public void SeekStart() {
//		Guard.Ensure(StartCluster >= 0, $"Start cluster {StartCluster} is not defined for empty cluster chain");
//		CurrentCluster = StartCluster;
//		CurrentLogicalCluster = 0;
//		CurrentTraits = _clusters.FastReadClusterTraits(CurrentCluster.Value);
//		Guard.Ensure(CurrentTraits.Value.HasFlag(ClusterTraits.Start), $"Start cluster {StartCluster} is not a start cluster");
//	}

//	public void SeekEnd() {
//		Guard.Ensure(EndCluster >= 0, $"Last cluster {EndCluster} is not defined for empty cluster chain");
//		CurrentCluster = EndCluster;
//		CurrentLogicalCluster = TotalClusters - 1;
//		CurrentTraits = _clusters.FastReadClusterTraits(CurrentCluster.Value);
//		Guard.Ensure(CurrentTraits.Value.HasFlag(ClusterTraits.End), $"End cluster {CurrentCluster.Value} was not marked end");
//	}

//	public void SeekTo(long logicalCluster) {
//		Guard.Ensure(StartCluster != -1, "Seeker parameters have not been set");
//		FindShortestPathToLogicalCluster(logicalCluster, out var startCluster, out var steps, out var direction);
//		if (startCluster == StartCluster) {
//			SeekStart();
//		} else if (startCluster == EndCluster) {
//			SeekEnd();
//		} else if (startCluster == CurrentCluster) {
//			// already at start point
//		} else {
//			throw new InvalidOperationException($"Unknown index to start cluster seek from: {startCluster}");
//		}

//		switch (direction) {
//			case IterateDirection.LeftToRight:
//				SeekForward(steps);
//				break;
//			case IterateDirection.RightToLeft:
//				SeekBackwards(steps);
//				break;
//			default:
//				throw new NotSupportedException($"{direction}");
//		}
//	}

//	public void SeekForward(long steps) {
//		var walkedClusters = new HashSet<long>();
//		if (CurrentCluster == null) {
//			CurrentCluster = StartCluster;
//			CurrentLogicalCluster = 0;
//			CurrentTraits = _clusters.FastReadClusterTraits(CurrentCluster.Value);
//		}
//		walkedClusters.Add(CurrentCluster.Value);
//		for (var i = 0; i < steps; i++) {
//			Guard.Against(CurrentTraits.Value.HasFlag(ClusterTraits.End), "Cannot walk past the last cluster on cluster chain");
//			var nextCluster = _clusters.FastReadClusterNext(CurrentCluster.Value);
//			if (walkedClusters.Contains(nextCluster))
//				throw new CorruptDataException($"Cyclic dependency detected (cluster {CurrentCluster.Value} has cyclic next {nextCluster})");
//			CurrentCluster = nextCluster;
//			CurrentTraits = _clusters.FastReadClusterTraits(nextCluster);
//			CurrentLogicalCluster++;
//			walkedClusters.Add(CurrentCluster.Value);
//		}
//	}

//	public void SeekBackwards(long steps) {
//		var walkedClusters = new HashSet<long>();
//		for (var i = 0; i < steps; i++) {
//			if (CurrentCluster == null) {
//				CurrentCluster = EndCluster;
//				CurrentLogicalCluster = TotalClusters - 1;
//				CurrentTraits = _clusters.FastReadClusterTraits(CurrentCluster.Value);
//			} else {
//				Guard.Against(CurrentTraits.Value.HasFlag(ClusterTraits.Start), "Cannot walk before the first cluster on cluster chain");
//				var prevCluster = _clusters.FastReadClusterPrev(CurrentCluster.Value);
//				if (walkedClusters.Contains(prevCluster))
//					throw new CorruptDataException($"Cyclic dependency detected (cluster {CurrentCluster.Value} has cyclic prev {prevCluster})");
//				CurrentCluster = prevCluster;
//				CurrentTraits = _clusters.FastReadClusterTraits(prevCluster);
//				CurrentLogicalCluster--;
//			}
//			walkedClusters.Add(CurrentCluster.Value);
//		}
//	}


//	protected void FindShortestPathToLogicalCluster(long logicalCluster, out long closestKnownCluster, out long steps, out IterateDirection seekDirection) {
//		CheckNotEmpty();
//		var paths = new List<(long FromKnownCluster, long Steps, IterateDirection Dir)>();

//		// Consider path from start
//		CalculatePathToFragment(0, logicalCluster, TotalClusters, out var steps_, out var dir);
//		paths.Add((StartCluster, steps_, dir));

//		// Consider path from end
//		CalculatePathToFragment(TotalClusters - 1, logicalCluster, TotalClusters, out steps_, out dir);
//		paths.Add((EndCluster, steps_, dir));

//		// Consider path from current position
//		if (CurrentLogicalCluster.HasValue) {
//			CalculatePathToFragment(CurrentLogicalCluster.Value, logicalCluster, TotalClusters, out steps_, out dir);
//			paths.Add((CurrentCluster.Value, steps_, dir));
//		}

//		// TODO: future optimizations can intermittenly remember cluster positions as walks (say 1024 positions, equidistant)

//		var result = paths.OrderBy(x => x.Steps).First();
//		closestKnownCluster = result.FromKnownCluster;
//		steps = result.Steps;
//		seekDirection = result.Dir;
//	}

//	public static void CalculatePathToFragment(long fromLogicalCluster, long toLogicalCluster, long totalClusters, out long steps, out IterateDirection seekDirection) {
//		var lastClusterIX = totalClusters - 1;
//		Guard.ArgumentInRange(fromLogicalCluster, 0, lastClusterIX, nameof(fromLogicalCluster));
//		Guard.ArgumentInRange(toLogicalCluster, 0, lastClusterIX, nameof(toLogicalCluster));

//		if (fromLogicalCluster == toLogicalCluster) {
//			steps = 0;
//			seekDirection = IterateDirection.LeftToRight;
//		} else if (fromLogicalCluster < toLogicalCluster) {
//			steps = toLogicalCluster - fromLogicalCluster;
//			seekDirection = IterateDirection.LeftToRight;
//		} else {
//			steps = fromLogicalCluster - toLogicalCluster;
//			seekDirection = IterateDirection.RightToLeft;
//		}
//	}

//	public void Dispose() {
//		_clusters.ClusterChainCreated -= ClusteredChainCreatedHandler;
//		_clusters.ClusterChainStartChanged -= ClusteredChainStartChangedHandler;
//		_clusters.ClusterChainEndChanged -= ClusterChainEndChangedHandler;
//		_clusters.ClusterMoved -= ClusterMovedHandler;
//		_clusters.ClusterChainRemoved -= ClusterChainRemovedHandler;
//	}

//	private void CheckNotEmpty() => Guard.Ensure(StartCluster >= 0 && EndCluster >= 0, "Cluster chain is empty");

//	private void ClusteredChainCreatedHandler(object sender, long startCluster, long endCluster, long totalClusters, long terminalValue) {
//		if (terminalValue == TerminalValue) {
//			Reset(startCluster, endCluster, totalClusters, terminalValue, null, null, null);
//		}
//	}

//	private void ClusterChainEndChangedHandler(object sender, long cluster, long terminalValue, long clusterCountDelta) {

//		if (terminalValue == TerminalValue) {
//			long? newCurrentCluster = CurrentCluster;
//			long? newCurrentLogicalIndex = CurrentLogicalCluster;
//			ClusterTraits? newCurrentTraits = CurrentTraits;
//			var newTotal = TotalClusters + clusterCountDelta;
//			if (newTotal == 0) {
//				// Case: chain removed (zero clusters left)
//				Reset(-1, -1, 0, TerminalValue, null, null, null);
//			} else if (newTotal == 1) {
//				// Case: end reduced now a single cluster chain
//				if (CurrentLogicalCluster == 0) {
//					newCurrentTraits = CurrentTraits.Value.CopyAndSetFlags(ClusterTraits.End);
//				} else {
//					newCurrentCluster = null;
//					newCurrentLogicalIndex = null;
//					newCurrentTraits = null;					
//				}

//				Reset(cluster, cluster, newTotal, terminalValue, newCurrentCluster, newCurrentLogicalIndex, newCurrentTraits);
//			} else if (newTotal < TotalClusters) {
//				// end was shortened, so current cluster may be affected as follows:
//				// - shortened to current cluster (becomes end)
//				// - shortened to before current cluster (becomes invalidated)
//				if (CurrentLogicalCluster == newTotal - 1) {
//					// shortened to current cluster (becomes new end)
//					newCurrentTraits = CurrentTraits.Value.CopyAndSetFlags(ClusterTraits.End);
//				} else if (CurrentLogicalCluster > newTotal - 1) {
//					// shorted beyond current, now invalidated
//					newCurrentCluster = null;
//					newCurrentLogicalIndex = null;
//					newCurrentTraits = null;
//				}
//				Reset(StartCluster, cluster, newTotal, terminalValue, newCurrentCluster, newCurrentLogicalIndex, newCurrentTraits);
//			} else if (newTotal > TotalClusters) {
//				// end was extend, so current cluster can be affected as follows:
//				// - current was at end and is now no longer at end 
//				// - current was not at end and is not affected
//				if (CurrentLogicalCluster == TotalClusters - 1) {
//					newCurrentTraits = newCurrentTraits.Value.CopyAndSetFlags(ClusterTraits.End, false);
//				}
//				Reset(StartCluster, cluster, newTotal, terminalValue, newCurrentCluster, newCurrentLogicalIndex, newCurrentTraits);
//			} else {
//				throw new InternalErrorException($"Unexpected condition");
//			}
//		}
//	}

//	private void ClusterMovedHandler(object sender, long fromCluster, long toCluster, ClusterTraits traits, long? terminalValue) {
//		// 1:  none        ; seeker points to nothing
//		// 2: [S, E]       ; start and end clusters are same, current is nil
//		// 3. [S, E, C]    ; start, end and current are same
//		// 4. [S] [E]      ; start and end are different, current is nil
//		// 5. [S] [C] [E]  ; start, current and end are different
//		// 6. [S, C] [E]   ; start and current are same, end is different
//		// 7. [S] [E, C]   ; start is different, end and current are same

//		switch (ClassifySeekerState(StartCluster, EndCluster, CurrentCluster)) {
//			case 1:
//				#region State 1: seeker points to nothing
//				// do nothing
//				#endregion
//				break;
//			case 2:
//				#region State 2: [S, E]       ; start and end clusters are same, current is nil
//				// Case 2.1: moved entire chain 
//				// Case 2.2: deleted entire chain 
//				if (fromCluster == StartCluster)  // 2.1
//					Reset(toCluster, toCluster, 1, TerminalValue, null, null, null);
//				else if (toCluster == StartCluster) // 2.2 
//					Reset(-1, -1, 0, TerminalValue, null, null, null);
//				#endregion
//				break;
//			case 3:
//				#region State 3. [S, E, C]    ; start, end and current are same
//				// case 3.1: moved entire chain
//				// case 3.2: deleted entire chain
//				if (fromCluster == StartCluster) // 3.1
//					Reset(toCluster, toCluster, 1, TerminalValue, toCluster, 0, CurrentTraits);
//				else if (toCluster == StartCluster) // 3.2
//					Reset(-1, -1, 0, TerminalValue, null, null, null);
//				#endregion
//				break;
//			case 4:
//				#region State 4. [S] [E]      ; start and end are different, current is nil
//				// case 4.1: moved start to end (error)
//				// case 4.2: moved end to start
//				// case 4.3: moved start elsewhere
//				// case 4.4: moved end elsewhere
//				// case 4.5: moved something to start (deleted)
//				// case 4.6: moved something to end (invalidated)
//				if (fromCluster == StartCluster) {
//					if (toCluster == EndCluster) { // 4.1
//						//throw new InternalErrorException("Start cluster merged with end, should never happen");
//						// do nothing
//					}  else  // 4.3
//						Reset(toCluster, EndCluster, TotalClusters, TerminalValue, null, null, null);
//				}
//				if (fromCluster == EndCluster) {
//					if (toCluster == StartCluster) { // 4.2
//													 //Reset(toCluster, toCluster, 1, TerminalValue, null, null, null);
//													 // Do nothing (will be fixed by later handler)
//					} else // 4.4
//						Reset(StartCluster, toCluster, TotalClusters, TerminalValue, null, null, null);
//				} else if (toCluster == StartCluster) { // 4.5
//					Reset(-1, -1, 0, TerminalValue, null, null, null);
//				} else if (toCluster == EndCluster) { // 4.6
//													  // do nothing (will be fixed by later handler)
//				}
//				#endregion
//				break;
//			case 5:
//				#region State 5. [S] [C] [E]  ; start, current and end are different 
//				// case 5.1: moved start to end (error)
//				// case 5.2: moved end to start
//				// case 5.3: moved start elsewhere
//				// case 5.4: moved end elsewhere
//				// case 5.5: moved something to start (deleted)
//				// case 5.6: moved something to end (invalidated)
//				// case 5.7: moved something to current
//				// case 5.8: moved current elsewhere
//				if (fromCluster == StartCluster) {
//					if (toCluster == EndCluster) {
//						// do nothing
//					}  else  // 5.3
//						Reset(toCluster, EndCluster, TotalClusters, TerminalValue, null, null, null);
//				}
//				if (fromCluster == EndCluster) {
//					if (toCluster == StartCluster) { // 5.2
//													 // Reset(toCluster, toCluster, 1, TerminalValue, null, null, null);
//													 // do nothing (will be fixed by later handler)

//					} else // 5.4
//						Reset(StartCluster, toCluster, TotalClusters, TerminalValue, null, null, null);
//				} else if (toCluster == StartCluster) { // 5.5
//					Reset(-1, -1, 0, TerminalValue, null, null, null);
//				} else if (toCluster == EndCluster) { // 5.6
//													  // do nothing (will be fixed by later handler)
//				} else if (fromCluster == CurrentCluster) { // 5.7
//					Reset(StartCluster, EndCluster, TotalClusters, TerminalValue, toCluster, CurrentLogicalCluster, CurrentTraits);
//				} else if (toCluster == CurrentCluster) { // 5.8
//														  // do nothing (will be fixed by later handler)
//					Reset(StartCluster, EndCluster, TotalClusters, TerminalValue, null, null, null);
//				}
//				#endregion
//				break;
//			case 6:
//				#region State 6. [S, C] [E]   ; start and current are same, end is different
//				// case 6.1: moved start to end (error)
//				// case 6.2: moved end to start
//				// case 6.3: moved start elsewhere
//				// case 6.4: moved end elsewhere
//				// case 6.5: moved something to start (error)
//				// case 6.6: moved something to end (invalidated)
//				if (fromCluster == StartCluster) {
//					if (toCluster == EndCluster) {
//						// do nothing
//					}  else  // 6.3
//						Reset(toCluster, EndCluster, TotalClusters, TerminalValue, null, null, null);
//				}
//				if (fromCluster == EndCluster) {
//					if (toCluster == StartCluster) { // 6.2
//													 //Reset(toCluster, toCluster, 1, TerminalValue, toCluster, 0, CurrentTraits.Value.CopyAndSetFlags(ClusterTraits.Start));
//													 // Do nothing (will be fixed by later handler)
//					} else // 6.4
//						Reset(StartCluster, toCluster, TotalClusters, TerminalValue, CurrentCluster, CurrentLogicalCluster, CurrentTraits);
//				} else if (toCluster == StartCluster) { // 6.5
//					Reset(-1, -1, 0, TerminalValue, null, null, null);
//				} else if (toCluster == EndCluster) { // 6.6
//													  // do nothing (will be fixed by later handler)
//				}
//				#endregion
//				break;
//			case 7:
//				#region State 7. [S] [E, C]   ; start is different, end and current are same
//				// case 7.1: moved start to end
//				// case 7.2: moved end to start
//				// case 7.3: moved start elsewhere
//				// case 7.4: moved end elsewhere
//				// case 7.5: moved something to start (error)
//				// case 7.6: moved something to end (invalidated)
//				if (fromCluster == StartCluster) {
//					if (toCluster == EndCluster) { // 7.1
//						// do nothing (will be fixed by later handler)
//					}  else  // 7.3
//						Reset(toCluster, EndCluster, TotalClusters, TerminalValue, CurrentCluster, CurrentLogicalCluster, CurrentTraits);
//				}
//				if (fromCluster == EndCluster) {
//					if (toCluster == StartCluster) { // 7.2
//													 //Reset(toCluster, toCluster, 1, TerminalValue, toCluster, 0, CurrentTraits.Value.CopyAndSetFlags(ClusterTraits.Start));
//													 // Do nothing (will be fixed by later handler)
//					} else // 7.4
//						Reset(StartCluster, toCluster, TotalClusters, TerminalValue, toCluster, CurrentLogicalCluster, CurrentTraits);
//				} else if (toCluster == StartCluster) { // 7.5
//					Reset(-1, -1, 0, TerminalValue, null, null, null);
//				} else if (toCluster == EndCluster) { // 7.6
//					// do nothing (will be fixed by later handler)
//				}
//				#endregion
//				break;
//			default:
//				throw new ArgumentOutOfRangeException();
//		}


//	}

//	private void ClusteredChainStartChangedHandler(object sender, long cluster, long terminalValue, long clusterCountDelta) {
//		throw new NotSupportedException("Cluster chain start moved");
//		if (terminalValue == TerminalValue) {
//			Reset(cluster, EndCluster, TotalClusters + clusterCountDelta, terminalValue, null, null, null);
//		}
//	}

//	private void ClusterChainRemovedHandler(object sender, long terminalValue) {
//		if (terminalValue == TerminalValue) {
//			TotalClusters = 0;
//			Reset(-1, -1, 0, terminalValue, null, null, null);
//		}
//	}


//	private static int ClassifySeekerState(long start, long end, long? current) {
//		// Will determine which state the seeker is in:
//		// 1:  none        ; seeker points to nothing
//		// 2: [S, E]       ; start and end clusters are same, current is nil
//		// 3. [S, E, C]    ; start, end and current are same
//		// 4. [S] [E]      ; start and end are different, current is nil
//		// 5. [S] [C] [E]  ; start, current and end are different
//		// 6. [S, C] [E]   ; start and current are same, end is different
//		// 7. [S] [E, C]   ; start is different, end and current are same
//		if (start == -1 && end == -1 && current == null) {
//			return 1;
//		} else if (start == end && current == null) {
//			return 2;
//		} else if (start == end && current == start) {
//			return 3;
//		} else if (start != end && current == null) {
//			return 4;
//		} else if (start != end && current != null && current != start && current != end) {
//			return 5;
//		} else if (start != end && current == start) {
//			return 6;
//		} else if (start != end && current == end) {
//			return 7;
//		} else {
//			throw new Exception($"Invalid seeker state: S: {start}, E: {end}, C: {current}");
//		}
//	}
//}
