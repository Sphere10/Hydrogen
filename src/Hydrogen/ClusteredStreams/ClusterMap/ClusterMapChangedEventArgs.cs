// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public class ClusterMapChangedEventArgs : EventArgs {

	public ClusterMapChangedEventArgs() {
		ClusterCountDelta = 0;
		AddedClusters = new HashSet<long>();
		ModifiedClusters = new HashSet<long>();
		RemovedClusters = new HashSet<long>();
		MovedClusters = new BijectiveDictionary<long, long>();
		MovedTerminals = new Dictionary<long, (long? NewStart, long? NewEnd)>();
		ChainTerminal = null;
		ChainOriginalStartCluster = null;
		ChainNewStartCluster = null;
		ChainOriginalEndCluster = null;
		ChainNewEndCluster = null;
	}

	public long ClusterCountDelta { get; set; }

	public ISet<long> AddedClusters { get; set; } 

	public ISet<long> ModifiedClusters { get; set; }

	public ISet<long> RemovedClusters { get; set; }

	public BijectiveDictionary<long, long> MovedClusters { get; set; }

	public IDictionary<long, (long? NewStart, long? NewEnd)> MovedTerminals { get; set; }

	public long? ChainTerminal { get; set; }
	
	public long? ChainOriginalStartCluster { get; set; }

	public long? ChainNewStartCluster { get; set; }

	public long? ChainOriginalEndCluster { get; set; }

	public long? ChainNewEndCluster { get; set; }

	public bool AddedChain => !ChainOriginalStartCluster.HasValue && ChainNewStartCluster.HasValue;
	
	public bool RemovedChain => ChainOriginalStartCluster.HasValue && !ChainNewStartCluster.HasValue;

	public bool IncreasedChainSize => ChainNewEndCluster.HasValue && ClusterCountDelta > 0;

	public bool DecreasedChainSize => ChainNewEndCluster.HasValue && ClusterCountDelta < 0;

	public void InformMovedCluster(long from, long to) {
		// Special case: When a cluster A is moved to B and then B is moved to C,
		// it means A is moved to C and B was delted
		if (MovedClusters.TryGetKey(from, out var originalFrom)) {
			RemovedClusters.Add(from);
			MovedClusters.Remove(originalFrom);
			from = originalFrom;
		}

		// Special case: When a cluster A is moved to C and then B is moved to C,
		// it means that A was deleted and B was moved to C
		if (MovedClusters.TryGetKey(to, out originalFrom)) {
			RemovedClusters.Add(originalFrom);
			MovedClusters.Remove(originalFrom);
		}

		MovedClusters.Add(from, to);
	}

	public void InformChainNewStart(long terminal, long cluster) {
		if (!MovedTerminals.TryGetValue(terminal, out var terminalMove)) {
			terminalMove = (null, null);
		}
		terminalMove.NewStart = cluster;
		MovedTerminals[terminal] = terminalMove;
	}

	public void InformChainNewEnd(long terminal, long cluster) {
		if (!MovedTerminals.TryGetValue(terminal, out var terminalMove)) {
			terminalMove = (null, null);
		}
		terminalMove.NewEnd = cluster;
		MovedTerminals[terminal] = terminalMove;
	}

	public enum MutationType {
		Added,
		Modified,
		Removed,
	}

}
