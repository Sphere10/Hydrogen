using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;


namespace Hydrogen;

internal static class ClusterDiagnostics {

	public static long VerifyPath(Cluster[] clusterContainer, long start, long end, long terminalValue, HashSet<long> visitedClusters) {
		bool IsValidCluster(long ix) => 0 <= ix && ix < clusterContainer.LongLength;

		if (start == -1 || end == -1) {
			Guard.Ensure(start == -1, "Start cluster was not -1");
			Guard.Ensure(end == -1, "Start cluster was not -1");
			return 0;
		}

		var cluster = start;
		var foundLast = false;
		var i = 0;
		while (!foundLast) {
			if (visitedClusters.Contains(cluster)) {
				throw new InvalidOperationException($"Cluster {cluster} (index: {i}) has already been visited.");
			}

			var traits = clusterContainer[cluster].Traits;
			var prev = clusterContainer[cluster].Prev;
			var next = clusterContainer[cluster].Next;

			// check next pointer
			if (!traits.HasFlag(ClusterTraits.End)) {
				Guard.Ensure(IsValidCluster(next), $"Cluster {cluster} (chain index: {i}) next points to non-existent cluster: {next}.");
				Guard.Ensure(clusterContainer[next].Prev == cluster, $"Cluster {cluster} (index: {i}) next cluster {next} does not point back to current cluster.");
			} else {
				Guard.Ensure(next == terminalValue, $"Cluster {cluster} (chain index: {i}) was marked End with terminal value was {next} but was expecting {terminalValue}");
				Guard.Ensure(next == terminalValue, $"End terminal value was {next} but was expecting {terminalValue}");
				foundLast = true;
			}

			// check previous pointer
			if (!traits.HasFlag(ClusterTraits.Start)) {
				Guard.Ensure(IsValidCluster(prev), $"Cluster {cluster} (chain index: {i}) prev points to non-existent cluster: {prev}.");
				Guard.Ensure(clusterContainer[prev].Next == cluster, $"Cluster {cluster} (chain index: {i}) prev cluster {prev} does not point back to current cluster.");
			} else {
				Guard.Ensure(prev == terminalValue, $"Cluster {cluster} (chain index: {i}) was marked Start with terminal value was {prev} but was expecting {terminalValue}");
				Guard.Ensure(prev == terminalValue, $"Start terminal value was {prev} but was expecting {terminalValue}");
			}

			// check first traits
			if (i == 0) {
				// beginning
				Guard.Ensure(traits.HasFlag(ClusterTraits.Start), $"Start cluster was not marked start, was {traits}");

			}

			// advance to next cluster
			visitedClusters.Add(cluster);
			cluster = next;
			i++;
		}
		return i;
	}

	public static void VerifyClusters(IClusterContainer clusterContainer) => VerifyClusters(clusterContainer.Clusters.ToArray(), null);

	public static void VerifyClusters(IClusteredStorage clusteredStorage) => VerifyClusters(clusteredStorage.ClusterContainer.Clusters.ToArray(), clusteredStorage.Records.ToArray());

	public static void VerifyClusters(Cluster[] clusters, ClusteredStreamRecord[] records) {
		Guard.ArgumentNotNull(clusters, nameof(clusters));
		// get all the start clusters
		var startClusters =
			clusters
				.Select((cluster, i) => (cluster, i))
				.Where(x => x.cluster.Traits.HasFlag(ClusterTraits.Start)).OrderBy(x => x.cluster.Prev).ToExtendedList();
		
		// get all the end clusters
		var endClusters =
			clusters
				.Select((cluster, i) => (cluster, i))
				.Where(x => x.cluster.Traits.HasFlag(ClusterTraits.End))
				.OrderBy(x => x.cluster.Next).ToExtendedList();

		// ensure all start clusters point to unique terminal
		var startClustersWithDuplicateTerminals =
			startClusters
				.GroupBy(x => x.cluster.Prev)
				.Where(x => x.Count() > 1)
				.Select(x => $"[Clusters: {x.ToDelimittedString(", ", toStringFunc: y => y.i.ToString())} Terminal: {x.Key}]")
				.ToArray();
		if (startClustersWithDuplicateTerminals.Any()) 
			throw new InvalidOperationException($"Start clusters have duplicate terminals: {startClustersWithDuplicateTerminals.ToDelimittedString(", ")}");


		// ensure all start clusters point to unique terminal
		var endClustersWithDuplicateTerminals =
			endClusters
				.GroupBy(x => x.cluster.Next)
				.Where(x => x.Count() > 1)
				.Select(x => $"[Clusters: {x.ToDelimittedString(", ", toStringFunc: y => y.i.ToString())} Terminal: {x.Key} ]")
				.ToArray();
		if (endClustersWithDuplicateTerminals.Any()) 
			throw new InvalidOperationException($"End clusters have duplicate terminals: {endClustersWithDuplicateTerminals.ToDelimittedString(", ")}");
		// ensure same number of start and end clusters
		Guard.Ensure(startClusters.Count == endClusters.Count, $"Start clusters count {startClusters.Count} does not match end clusters count {endClusters.Count}");
		
		// ensure all start clusters have commensurate end cluster matched by terminal
		var endTerminals = endClusters.Select(x => x.cluster.Next).ToHashSet();
		var startClustersWithoutEndClusters = startClusters.Where(x => !endTerminals.Contains(x.cluster.Prev)).Select(x => (cluster: x.i, terminal:x.cluster.Prev)).ToArray();
		if (startClustersWithoutEndClusters.Any())
			throw new InvalidOperationException($"Start clusters {startClustersWithoutEndClusters.ToDelimittedString(", ", toStringFunc: x => $"(Cluster: {x.cluster}, Terminal: {x.terminal}" )} do not have commensurate end clusters.");
		// Don't need to check reverse since other checks would've prevented it

		// Ensure all non-start/end clusters do not loop back to themselves
		var loopBackClusters = clusters
			.Where(x => !x.Traits.HasFlag(ClusterTraits.Start) && !x.Traits.HasFlag(ClusterTraits.End))
			.Where(x => x.Prev == x.Next)
			.ToArray();
		if (loopBackClusters.Any())
			throw new InvalidOperationException($"Clusters {loopBackClusters.ToDelimittedString(", ", toStringFunc: x => x.ToString())} loop back to themselves.");
	
		// visit all paths, ensuring no cycles and measuring their lengths
		var visited = new HashSet<long>();
		var clusterChains = new Dictionary<long, (long start, long end, long terminal, long length)>();
		for (var i = 0; i < startClusters.Count; i++) {
			var start = startClusters[i];
			var end = endClusters[i];
			Guard.Ensure(start.cluster.Prev == end.cluster.Next, $"Mismatched terminals for start cluster {start.cluster} and end cluster {end.cluster} (terminals were {start.cluster.Prev} and {end.cluster.Next})");
			var length = VerifyPath(clusters, start.i, end.i, start.cluster.Prev, visited);
			if (start.cluster.Prev != -1) // ignore record cluster chain (special case chain for tracking all other chains)
				clusterChains.Add(start.cluster.Prev, (start.i, end.i, start.cluster.Prev, length));
		}

		// ensure all clusters visited
		var notVisited = Tools.Collection.RangeL(0, clusters.Length).Except(visited).ToExtendedList();
		if (notVisited.Any()) {
			throw new InvalidOperationException($"Clusters {notVisited.ToDelimittedString(", ")} were not visited.");
		}

		if (records is not null) {
			bool IsValidCluster(long c) => 0 <= c && c < clusters.Length || c == -1;
			
			// ensure all records have valid clusters
			var recordsReferencingOutOfBoundsClusters = 
				records
				.Select((record, i) => (record, i))
				.Where(x => !IsValidCluster(x.record.StartCluster) || !IsValidCluster(x.record.EndCluster))
				.ToArray();
			if (recordsReferencingOutOfBoundsClusters.Any()) 
				throw new InvalidOperationException($"Records {recordsReferencingOutOfBoundsClusters.ToDelimittedString(", ")} reference out of bounds clusters (min:{0} max:{clusters.Length})");
	
			// ensure all records have dangling -1 terminals
			var recordsWithDanglingTerminals =
				records
				.Where(x => x.StartCluster == -1 && x.EndCluster != -1 || x.StartCluster != -1 && x.EndCluster == -1)
				.ToArray();
			if (recordsWithDanglingTerminals.Any()) 
				throw new InvalidOperationException($"Records {recordsWithDanglingTerminals.ToDelimittedString(", ")} have dangling null terminals (start:{0} end:{1})");
			
			// Ensure no records point to same start cluster
			var recordsWithDuplicateStartClusters =
				records
					.Select((record, i) => (record, i))
					.Where(x => x.record.StartCluster != -1)
					.GroupBy(x => x.record.StartCluster)
					.Where(x => x.Count() > 1)
					.ToArray();
			if (recordsWithDuplicateStartClusters.Any())
				throw new InvalidOperationException($"Records {recordsWithDuplicateStartClusters.SelectMany(x => x).ToDelimittedString(", ")} have duplicate start cluster(s).");

			// Ensure no records point to same end cluster
			var recordsWithDuplicateEndClusters =
				records
					.Select((record, i) => (record, i))
					.Where(x => x.record.EndCluster != -1)
					.GroupBy(x => x.record.EndCluster)
					.Where(x => x.Count() > 1)
					.ToArray();
			if (recordsWithDuplicateEndClusters.Any())
				throw new InvalidOperationException($"Records {recordsWithDuplicateEndClusters.SelectMany(x => x).ToDelimittedString(", ")} have duplicate end clusters.");

			// Ensure no records exist that aren't referenced by a chain (except for null records)
			var recordsNotAddressedByAChain = 
				records
					.Where((_, i) => !clusterChains.ContainsKey(i))
					.Select((r, i) => (record: r, index: i))
					.Where(x => x.record.StartCluster != -1 && x.record.EndCluster != -1 && x.record.Size == 0) // exclude null records
					.ToArray();
			if (recordsNotAddressedByAChain.Any()) 
				throw new InvalidOperationException($"Non-null records {recordsNotAddressedByAChain.ToDelimittedString(", ", toStringFunc: x => $"(Index: {x.index}, Record: {x.record})")} are not addressed by a cluster chain.");

			// Ensure all chains (except record chain) link to a valid record
			var chainsLinkingToInvalidRecords =
				clusterChains
					.Where(x => !(0 <= x.Key && x.Key <= records.Length))
					.Select(x => (start: x.Value.start, end: x.Value.end, record: x.Key))
					.ToArray();
			if (chainsLinkingToInvalidRecords.Any())
				throw new InvalidOperationException($"Cluster Chains {chainsLinkingToInvalidRecords.ToDelimittedString(", ", toStringFunc: x => $"(Start: {x.start}, End: {x.end}, Record: {x.record}")} reference(s) a non-existent record.");

			// Ensure chains link to non-null records
			var chainsLinkingToNullRecords =
				clusterChains
					.Where(x => records[x.Key].StartCluster == -1 || records[x.Key].EndCluster == -1 || records[x.Key].Size == 0)
					.Select(x => (start: x.Value.start, end: x.Value.end, record: x.Key))
					.ToArray();
			if (chainsLinkingToNullRecords.Any())
				throw new InvalidOperationException($"Cluster Chains {chainsLinkingToNullRecords.ToDelimittedString(", ", toStringFunc: x => $"(Start: {x.start}, End: {x.end}, Record: {x.record}")} reference(s) a null record.");


			// Ensure records don't link to non-terminal clusters
			var recordsReferencingNonTerminalClusters =
				records
					.Where(x => x.StartCluster != -1 && x.EndCluster != -1)
					.Where(x => !clusters[x.StartCluster].Traits.HasFlag(ClusterTraits.Start) || !clusters[x.EndCluster].Traits.HasFlag(ClusterTraits.End))
					.ToArray();
			if (recordsReferencingNonTerminalClusters.Any())
				throw new InvalidOperationException($"Records {recordsReferencingNonTerminalClusters.ToDelimittedString(", ")} reference(s) non-terminal clusters.");

			// Ensure records match chain terminals
			var recordsNotMatchingChains =
				records
					.Select((r, i) => (record: r, index: i))
					.Where(x => x.record.StartCluster != -1 && x.record.EndCluster != -1)
					.Select((r, i) => (record: r.record, chain: clusterChains[r.index]))
					.Where(x => x.record.StartCluster != x.chain.start || x.record.EndCluster != x.chain.end)
					.ToArray();
			if (recordsNotMatchingChains.Any())
				throw new InvalidOperationException($"Records {recordsNotMatchingChains.ToDelimittedString(", ", toStringFunc: x => $"(Record: {x.record}, Chain:[{x.chain.start}, {x.chain.end}])")} reference non-terminal clusters.");

		}
	}

	#region Seek reset verification

	public static void VerifySeekResetStreamOptimized(IClusterContainer clusterContainer, long start, long end, long totalClusters, long terminalValue, long? currentCluster, long? currentIndex, ClusterTraits? currentTraits, HashSet<long> visitedClusters) {
		bool IsValidCluster(long ix) => 0 <= ix && ix < clusterContainer.Clusters.Count;

		if (start == -1 || end == -1 || totalClusters == 0) {
			Guard.Ensure(start == -1, "Start cluster was not -1");
			Guard.Ensure(end == -1, "Start cluster was not -1");
			Guard.Ensure(totalClusters == 0, "Total clusters was not 0");
			//Guard.Ensure(terminalValue == -1, "Terminal was not -1");
			Guard.Ensure(currentCluster == null, "Current cluster was not null");
			Guard.Ensure(currentIndex == null, "Current index was not null");
			Guard.Ensure(currentTraits == null, "Current traits was not null");
			return;
		}

		var cluster = start;
		for (var i = 0; i < totalClusters; i++) {
			if (visitedClusters.Contains(cluster)) {
				throw new InvalidOperationException($"Cluster {cluster} (index: {i}) has already been visited.");
			}

			var traits = clusterContainer.FastReadClusterTraits(cluster);
			var prev = clusterContainer.FastReadClusterPrev(cluster);
			var next = clusterContainer.FastReadClusterNext(cluster);

			// ensure cluster doesn't point to itself
			Guard.Against(!traits.HasFlag(ClusterTraits.Start) && prev == cluster, $"Cluster {cluster} prev pointer pointed to itself");
			Guard.Against(!traits.HasFlag(ClusterTraits.End) && next == cluster, $"Cluster {cluster} next pointer pointed to itself");

			// check next pointer
			if (!traits.HasFlag(ClusterTraits.End)) {
				Guard.Ensure(IsValidCluster(next), $"Cluster {cluster} (index: {i}) next points to non-existent cluster: {next}.");
				Guard.Ensure(clusterContainer.FastReadClusterPrev(next) == cluster, $"Cluster {cluster} (index: {i}) next cluster {next} does not point back to current cluster.");
			} else {
				Guard.Ensure(next == terminalValue, $"Cluster {cluster} (index: {i}) was marked End with terminal value was {next} but was expecting {terminalValue}");
			}

			// check previous pointer
			if (!traits.HasFlag(ClusterTraits.Start)) {
				Guard.Ensure(IsValidCluster(prev), $"Cluster {cluster} (index: {i}) prev points to non-existent cluster: {prev}.");
				Guard.Ensure(clusterContainer.FastReadClusterNext(prev) == cluster, $"Cluster {cluster} (index: {i}) prev cluster {prev} does not point back to current cluster.");
			} else {
				Guard.Ensure(prev == terminalValue, $"Cluster {cluster} (index: {i}) was marked Start with terminal value was {prev} but was expecting {terminalValue}");
			}

			// check first traits
			if (i == 0) {
				// beginning
				Guard.Ensure(traits.HasFlag(ClusterTraits.Start), $"Start cluster {cluster} was not marked start, was {traits}");
				Guard.Ensure(prev == terminalValue, $"Start terminal value was {prev} but was expecting {terminalValue}");
			}

			// check last cluster
			if (i == totalClusters - 1) {
				// end
				Guard.Ensure(traits.HasFlag(ClusterTraits.End), $"End cluster {cluster} was not marked end, was {traits}");
				Guard.Ensure(next == terminalValue, $"End terminal value was {next} but was expecting {terminalValue}");
			}

			// check middle cluster
			if (i > 0 && i < totalClusters - 1) {
				// middle
				Guard.Ensure(traits == ClusterTraits.None, $"Middle cluster {cluster} was marked with a traits {traits}");
			}

			// check current
			if (i == currentIndex || cluster == currentCluster) {
				Guard.Ensure(i == currentIndex, $"At current cluster, it's actual index was {i} but was expecting {currentIndex}");
				Guard.Ensure(cluster == currentCluster, $"At current cluster index, the actual cluster is {cluster} but was expecting {currentIndex}");
				Guard.Ensure(traits == currentTraits, $"At current cluster index, the actual traits is {traits} but was expecting {currentTraits}");
			}

			// advance to next cluster
			visitedClusters.Add(cluster);
			cluster = next;
		}

	}

	public static void VerifySeekReset(IReadOnlyExtendedList<Cluster> clusterContainer, long start, long end, long totalClusters, long terminalValue, long? currentCluster, long? currentIndex, ClusterTraits? currentTraits, HashSet<long> visitedClusters) {
		bool IsValidCluster(long ix) => 0 <= ix && ix < clusterContainer.Count;

		if (start == -1 || end == -1 || totalClusters == 0) {
			Guard.Ensure(start == -1, "Start cluster was not -1");
			Guard.Ensure(end == -1, "Start cluster was not -1");
			Guard.Ensure(totalClusters == 0, "Total clusters was not 0");
			//Guard.Ensure(terminalValue == -1, "Terminal was not -1");
			Guard.Ensure(currentCluster == null, "Current cluster was not null");
			Guard.Ensure(currentIndex == null, "Current index was not null");
			Guard.Ensure(currentTraits == null, "Current traits was not null");
			return;
		}

		var cluster = start;
		for (var i = 0; i < totalClusters; i++) {
			if (visitedClusters.Contains(cluster)) {
				throw new InvalidOperationException($"Cluster {cluster} (index: {i}) has already been visited.");
			}

			var traits = clusterContainer[cluster].Traits;
			var prev = clusterContainer[cluster].Prev;
			var next = clusterContainer[cluster].Next;

			// check next pointer
			if (!traits.HasFlag(ClusterTraits.End)) {
				Guard.Ensure(IsValidCluster(next), $"Cluster {cluster} (index: {i}) next points to non-existent cluster: {next}.");
				Guard.Ensure(clusterContainer[next].Prev == cluster, $"Cluster {cluster} (index: {i}) next cluster {next} does not point back to current cluster.");
			} else {
				Guard.Ensure(next == terminalValue, $"Cluster {cluster} (index: {i}) was marked End with terminal value was {next} but was expecting {terminalValue}");
			}

			// check previous pointer
			if (!traits.HasFlag(ClusterTraits.Start)) {
				Guard.Ensure(IsValidCluster(prev), $"Cluster {cluster} (index: {i}) prev points to non-existent cluster: {prev}.");
				Guard.Ensure(clusterContainer[prev].Next == cluster, $"Cluster {cluster} (index: {i}) prev cluster {prev} does not point back to current cluster.");
			} else {
				Guard.Ensure(prev == terminalValue, $"Cluster {cluster} (index: {i}) was marked Start with terminal value was {prev} but was expecting {terminalValue}");
			}

			// check first traits
			if (i == 0) {
				// beginning
				Guard.Ensure(traits.HasFlag(ClusterTraits.Start), $"Start cluster was not marked start, was {traits}");
				Guard.Ensure(prev == terminalValue, $"Start terminal value was {prev} but was expecting {terminalValue}");
			}

			// check last cluster
			if (i == totalClusters - 1) {
				// end
				Guard.Ensure(traits.HasFlag(ClusterTraits.End), $"End cluster was not marked end, was {traits}");
				Guard.Ensure(next == terminalValue, $"End terminal value was {next} but was expecting {terminalValue}");
			}

			// check middle cluster
			if (i > 0 && i < totalClusters - 1) {
				// middle
				Guard.Ensure(traits == ClusterTraits.None, $"Middle cluster was marked with a traits {traits}");
			}

			// check current
			if (i == currentIndex || cluster == currentCluster) {
				Guard.Ensure(i == currentIndex, $"At current cluster, it's actual index was {i} but was expecting {currentIndex}");
				Guard.Ensure(cluster == currentCluster, $"At current cluster index, the actual cluster is {cluster} but was expecting {currentIndex}");
				Guard.Ensure(traits == currentTraits, $"At current cluster index, the actual traits is {traits} but was expecting {currentTraits}");
			}

			// advance to next cluster
			visitedClusters.Add(cluster);
			cluster = next;
		}
	}


	#endregion

}
