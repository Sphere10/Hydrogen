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
using System.Text;

namespace Hydrogen;

/// <summary>
/// Diagnostic helpers for validating and visualizing clustered stream topology.
/// </summary>
public static class ClusterDiagnostics {

	public static string ToTextDump(ClusteredStreams streams) {
		using (streams.EnterAccessScope()) {
			var stringBuilder = new FastStringBuilder();
			stringBuilder.AppendLine(streams.ToString());
			stringBuilder.AppendLine("Stream Descriptors:");
			for (var i = 0; i < streams.Count; i++) {
				var descriptor = streams.GetStreamDescriptor(i);
				stringBuilder.AppendLine($"\t{i}: {descriptor}");
			}
			stringBuilder.AppendLine("Cluster Container:");
			stringBuilder.AppendLine(ToTextDump(streams.ClusterMap));
			return stringBuilder.ToString();
		}
	}

	public static string ToTextDump(ClusterMap clusterMap) => ToTextDump(clusterMap, clusterMap.Clusters.Count);

	public static string ToTextDump(ClusterMap clusterMap, int logicalCount) {
		var stringBuilder = new StringBuilder();
		for (var i = 0; i < logicalCount; i++) {
			var cluster = clusterMap.Clusters[i];
			stringBuilder.AppendLine($"\t{i}: {cluster}");
		}
		return stringBuilder.ToString();
	}

	public static long VerifyPath(Cluster[] clusterContainer, long start, long end, long terminalValue, HashSet<long> visitedClusters) {
		bool IsValidCluster(long ix) => 0 <= ix && ix < clusterContainer.LongLength;

		if (start == Cluster.Null || end == Cluster.Null) {
			Guard.Ensure(start == Cluster.Null, $"Start cluster was not {Cluster.Null}");
			Guard.Ensure(end == Cluster.Null, $"Start cluster was not {Cluster.Null}");
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

	public static void VerifyClusters(ClusterMap clusterMap) => VerifyClusters(clusterMap.Clusters.ToArray(), null);

	public static void VerifyClusters(ClusteredStreams streams) {
		using (streams.EnterAccessScope()) {
			VerifyClusters(streams.ClusterMap.Clusters.ToArray(), Tools.Collection.RangeL(0, streams.Count).Select(streams.GetStreamDescriptor).ToArray());
		}
	}

	public static void VerifyClusters(Cluster[] clusters, ClusteredStreamDescriptor[] streamDescriptors) {
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
		var startClustersWithoutEndClusters = startClusters.Where(x => !endTerminals.Contains(x.cluster.Prev)).Select(x => (cluster: x.i, terminal: x.cluster.Prev)).ToArray();
		if (startClustersWithoutEndClusters.Any())
			throw new InvalidOperationException($"Start clusters {startClustersWithoutEndClusters.ToDelimittedString(", ", toStringFunc: x => $"(Cluster: {x.cluster}, Terminal: {x.terminal}")} do not have commensurate end clusters.");
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
			if (start.cluster.Prev != Cluster.Null) // ignore record cluster chain (special case chain for tracking all other chains)
				clusterChains.Add(start.cluster.Prev, (start.i, end.i, start.cluster.Prev, length));
		}

		// ensure all clusters visited
		var notVisited = Tools.Collection.RangeL(0, clusters.Length).Except(visited).ToExtendedList();
		if (notVisited.Any()) {
			throw new InvalidOperationException($"Clusters {notVisited.ToDelimittedString(", ")} were not visited.");
		}

		if (streamDescriptors is not null) {
			bool IsValidCluster(long c) => 0 <= c && c < clusters.Length || c == Cluster.Null;

			// ensure all streamDescriptors have valid clusters
			var recordsReferencingOutOfBoundsClusters =
				streamDescriptors
				.Select((descriptor, i) => (descriptor, i))
				.Where(x => !IsValidCluster(x.descriptor.StartCluster) || !IsValidCluster(x.descriptor.EndCluster))
				.ToArray();
			if (recordsReferencingOutOfBoundsClusters.Any())
				throw new InvalidOperationException($"Stream descriptors {recordsReferencingOutOfBoundsClusters.ToDelimittedString(", ")} reference out of bounds clusters (min:{0} max:{clusters.Length})");

			// ensure all streamDescriptors have dangling NULL (-1) terminals
			var recordsWithDanglingTerminals =
				streamDescriptors
				.Where(x => x.StartCluster == Cluster.Null && x.EndCluster != Cluster.Null || x.StartCluster != Cluster.Null && x.EndCluster == Cluster.Null)
				.ToArray();
			if (recordsWithDanglingTerminals.Any())
				throw new InvalidOperationException($"Stream descriptors {recordsWithDanglingTerminals.ToDelimittedString(", ")} have dangling null terminals (start:{0} end:{1})");

			// Ensure no streamDescriptors point to same start cluster
			var recordsWithDuplicateStartClusters =
				streamDescriptors
					.Select((record, i) => (record, i))
					.Where(x => x.record.StartCluster != Cluster.Null)
					.GroupBy(x => x.record.StartCluster)
					.Where(x => x.Count() > 1)
					.ToArray();
			if (recordsWithDuplicateStartClusters.Any())
				throw new InvalidOperationException($"Stream descriptors {recordsWithDuplicateStartClusters.SelectMany(x => x).ToDelimittedString(", ")} have duplicate start cluster(s).");

			// Ensure no streamDescriptors point to same end cluster
			var recordsWithDuplicateEndClusters =
				streamDescriptors
					.Select((record, i) => (record, i))
					.Where(x => x.record.EndCluster != Cluster.Null)
					.GroupBy(x => x.record.EndCluster)
					.Where(x => x.Count() > 1)
					.ToArray();
			if (recordsWithDuplicateEndClusters.Any())
				throw new InvalidOperationException($"Stream descriptors {recordsWithDuplicateEndClusters.SelectMany(x => x).ToDelimittedString(", ")} have duplicate end clusters.");

			// Ensure no streamDescriptors exist that aren't referenced by a chain (except for null streamDescriptors)
			var recordsNotAddressedByAChain =
				streamDescriptors
					.Where((_, i) => !clusterChains.ContainsKey(i))
					.Select((r, i) => (descriptor: r, index: i))
					.Where(x => x.descriptor.StartCluster != Cluster.Null && x.descriptor.EndCluster != Cluster.Null && x.descriptor.Size == 0) // exclude null streamDescriptors
					.ToArray();
			if (recordsNotAddressedByAChain.Any())
				throw new InvalidOperationException($"Non-null stream descriptors {recordsNotAddressedByAChain.ToDelimittedString(", ", toStringFunc: x => $"(Index: {x.index}, Descriptor: {x.descriptor})")} are not addressed by a cluster chain.");

			// Ensure all chains (except record chain) link to a valid record
			var chainsLinkingToInvalidRecords =
				clusterChains
					.Where(x => !(0 <= x.Key && x.Key <= streamDescriptors.Length))
					.Select(x => (start: x.Value.start, end: x.Value.end, record: x.Key))
					.ToArray();
			if (chainsLinkingToInvalidRecords.Any())
				throw new InvalidOperationException($"Cluster Chains {chainsLinkingToInvalidRecords.ToDelimittedString(", ", toStringFunc: x => $"(Start: {x.start}, End: {x.end}, Descriptor: {x.record}")} reference(s) a non-existent record.");

			// Ensure chains link to non-null streamDescriptors
			var chainsLinkingToNullRecords =
				clusterChains
					.Where(x => streamDescriptors[x.Key].StartCluster == Cluster.Null || streamDescriptors[x.Key].EndCluster == Cluster.Null || streamDescriptors[x.Key].Size == 0)
					.Select(x => (start: x.Value.start, end: x.Value.end, record: x.Key))
					.ToArray();
			if (chainsLinkingToNullRecords.Any())
				throw new InvalidOperationException($"Cluster Chains {chainsLinkingToNullRecords.ToDelimittedString(", ", toStringFunc: x => $"(Start: {x.start}, End: {x.end}, Descriptor: {x.record}")} reference(s) a null record.");


			// Ensure streamDescriptors don't link to non-terminal clusters
			var recordsReferencingNonTerminalClusters =
				streamDescriptors
					.Where(x => x.StartCluster != Cluster.Null && x.EndCluster != Cluster.Null)
					.Where(x => !clusters[x.StartCluster].Traits.HasFlag(ClusterTraits.Start) || !clusters[x.EndCluster].Traits.HasFlag(ClusterTraits.End))
					.ToArray();
			if (recordsReferencingNonTerminalClusters.Any())
				throw new InvalidOperationException($"Stream descriptors {recordsReferencingNonTerminalClusters.ToDelimittedString(", ")} reference(s) non-terminal clusters.");

			// Ensure streamDescriptors match chain terminals
			var recordsNotMatchingChains =
				streamDescriptors
					.Select((r, i) => (descriptor: r, index: i))
					.Where(x => x.descriptor.StartCluster != Cluster.Null && x.descriptor.EndCluster != Cluster.Null)
					.Select((r, i) => (record: r.descriptor, chain: clusterChains[r.index]))
					.Where(x => x.record.StartCluster != x.chain.start || x.record.EndCluster != x.chain.end)
					.ToArray();
			if (recordsNotMatchingChains.Any())
				throw new InvalidOperationException($"Stream descriptors {recordsNotMatchingChains.ToDelimittedString(", ", toStringFunc: x => $"(Descriptor: {x.record}, Chain:[{x.chain.start}, {x.chain.end}])")} reference non-terminal clusters.");

		}
	}

	#region Seek reset verification

	public static void VerifySeekResetStreamOptimized(ClusterMap clusterMap, long start, long end, long totalClusters, long terminalValue, long? currentCluster, long? currentIndex, ClusterTraits? currentTraits, HashSet<long> visitedClusters) {
		bool IsValidCluster(long ix) => 0 <= ix && ix < clusterMap.Clusters.Count;

		if (start == Cluster.Null || end == Cluster.Null || totalClusters == 0) {
			Guard.Ensure(start == Cluster.Null, $"Start cluster was not {Cluster.Null}");
			Guard.Ensure(end == Cluster.Null, $"Start cluster was not {Cluster.Null}");
			Guard.Ensure(totalClusters == 0, "Total clusters was not 0");
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

			var traits = clusterMap.ReadClusterTraits(cluster);
			var prev = clusterMap.ReadClusterPrev(cluster);
			var next = clusterMap.ReadClusterNext(cluster);

			// ensure cluster doesn't point to itself
			Guard.Against(!traits.HasFlag(ClusterTraits.Start) && prev == cluster, $"Cluster {cluster} prev pointer pointed to itself");
			Guard.Against(!traits.HasFlag(ClusterTraits.End) && next == cluster, $"Cluster {cluster} next pointer pointed to itself");

			// check next pointer
			if (!traits.HasFlag(ClusterTraits.End)) {
				Guard.Ensure(IsValidCluster(next), $"Cluster {cluster} (index: {i}) next points to non-existent cluster: {next}.");
				Guard.Ensure(clusterMap.ReadClusterPrev(next) == cluster, $"Cluster {cluster} (index: {i}) next cluster {next} does not point back to current cluster.");
			} else {
				Guard.Ensure(next == terminalValue, $"Cluster {cluster} (index: {i}) was marked End with terminal value was {next} but was expecting {terminalValue}");
			}

			// check previous pointer
			if (!traits.HasFlag(ClusterTraits.Start)) {
				Guard.Ensure(IsValidCluster(prev), $"Cluster {cluster} (index: {i}) prev points to non-existent cluster: {prev}.");
				Guard.Ensure(clusterMap.ReadClusterNext(prev) == cluster, $"Cluster {cluster} (index: {i}) prev cluster {prev} does not point back to current cluster.");
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

		if (start == Cluster.Null || end == Cluster.Null || totalClusters == 0) {
			Guard.Ensure(start == Cluster.Null, $"Start cluster was not {Cluster.Null}");
			Guard.Ensure(end == Cluster.Null, $"Start cluster was not {Cluster.Null}");
			Guard.Ensure(totalClusters == 0, "Total clusters was not 0");
			//Guard.Ensure(terminalValue == Cluster.Null, $"Terminal was not {Cluster.Null}");
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
