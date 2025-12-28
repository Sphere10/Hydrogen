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
/// Provides cluster-level fragments that back a single logical clustered stream, handling resizing and cluster map updates.
/// </summary>
internal class ClusteredStreamFragmentProvider : IStreamFragmentProvider {
	public event EventHandlerEx<ClusteredStreamFragmentProvider, long> StreamLengthChanged;

	private readonly ClusterMap _parent;

	private long _totalBytes;

	public ClusteredStreamFragmentProvider(ClusterMap clusteredMap, long terminal, long totalBytes, long startCluster, long endCluster, long totalClusters, bool integrityChecks) {
		_parent = clusteredMap;
		_totalBytes = totalBytes;
		FragmentCount = clusteredMap.CalculateClusterChainLength(totalBytes);	
		Seeker = new ClusterSeeker(clusteredMap, terminal, startCluster, endCluster, totalClusters, integrityChecks);
		Terminal = terminal;
	}

	public long TotalBytes { 
		get => _totalBytes; 
		private set {
			if (_totalBytes != value) {
				_totalBytes = value;
				NotifyStreamLengthChanged(value);
			}
		}
	}

	public long Terminal { get; internal set;}

	public long FragmentCount { get; private set; }
	
	internal ClusterSeeker Seeker { get; }

	public ReadOnlySpan<byte> GetFragment(long index) {
		Guard.ArgumentInRange(index, 0, _parent.Clusters.Count - 1, nameof(index));
		return _parent.ReadClusterData(index, 0, _parent.ClusterSize);
	}

	public void MapStreamPosition(long position, out long fragmentID, out long fragmentPosition) {
		CheckNotEmpty();
		var logicalClusterIndex = position / _parent.ClusterSize;
		Seeker.SeekTo(logicalClusterIndex);
		fragmentID = Seeker.Pointer.CurrentCluster;
		fragmentPosition = position % _parent.ClusterSize;
	}

	public void UpdateFragment(long fragmentID, long fragmentPosition, ReadOnlySpan<byte> updateSpan) {
		CheckNotEmpty();
		_parent.WriteClusterData(fragmentID, fragmentPosition, updateSpan);		
	}

	public void SetTotalBytes(long length) {
		var oldLength = TotalBytes;
		var newTotalClusters = _parent.CalculateClusterChainLength(length);
		var oldTotalClusters = FragmentCount;
		var currentTotalClusters = oldTotalClusters;
		
		// allocate/deallocate clusters as required
		if (newTotalClusters > currentTotalClusters) {
			if (currentTotalClusters == 0) 
				_parent.NewClusterChain(newTotalClusters, Terminal);
			else 
				_parent.AppendClustersToEnd(Seeker.Pointer.Chain.EndCluster, newTotalClusters - currentTotalClusters);
		} else if (newTotalClusters < currentTotalClusters) {
			_parent.RemoveBackwards(Seeker.Pointer.Chain.EndCluster, currentTotalClusters - newTotalClusters);
		}

		TotalBytes = length;
		FragmentCount = newTotalClusters;

		// Erase unused portion of tip cluster when shrinking stream
		if (length < oldLength) {
			var unusedTipClusterBytes = newTotalClusters * _parent.ClusterSize - length;
			if (unusedTipClusterBytes > 0) {
				var unusedTipClusterBytesI = Tools.Collection.CheckNotImplemented64bitAddressingLength(unusedTipClusterBytes);
				_parent.WriteClusterData(Seeker.Pointer.Chain.EndCluster, _parent.ClusterSize - unusedTipClusterBytes, _parent.ZeroClusterBytes.AsSpan().Slice(..unusedTipClusterBytesI));
			}
		}
	}

	public void ProcessClusterMapChanged(ClusterMapChangedEventArgs changedEvent) {
		Seeker.ProcessClusterMapChanged(changedEvent);
	}

	public void ProcessStreamSwapped(long stream1, ClusteredStreamDescriptor stream1Descriptor, long stream2, ClusteredStreamDescriptor streamDescriptor2) {
		if (Terminal == stream1) {
			Terminal = stream2;
		}
		else if (Terminal == stream2) {
			Terminal = stream1;
		}
		Seeker.ProcessStreamSwapped(stream1, stream1Descriptor, stream2, streamDescriptor2);
	}

	public void ClearEventHandlers() {
		StreamLengthChanged = null;
	}
	private void CheckNotEmpty() {
		Guard.Ensure(TotalBytes > 0, "Stream is empty.");
	}

	private void NotifyStreamLengthChanged(long newLength) => StreamLengthChanged?.Invoke(this, newLength);

}
