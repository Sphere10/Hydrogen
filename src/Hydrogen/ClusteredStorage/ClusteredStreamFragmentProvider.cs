using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hydrogen;

internal class ClusteredStreamFragmentProvider : IStreamFragmentProvider, IDisposable {
	public event EventHandlerEx<ClusteredStreamFragmentProvider, long> StreamLengthChanged;

	private readonly IClusterContainer _parent;
	
	private long _totalBytes;

	public ClusteredStreamFragmentProvider(IClusterContainer clusteredContainer, long logicalRecordID, long startCluster, long endCluster, long totalBytes) {
		_parent = clusteredContainer;
		_totalBytes = totalBytes;
		FragmentCount = clusteredContainer.CalculateClusterChainLength(totalBytes);	
		Seeker = new ClusterSeeker(clusteredContainer, startCluster, endCluster, FragmentCount, logicalRecordID);
		LogicalRecordID = logicalRecordID;
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

	public long LogicalRecordID { get; internal set;}

	public long FragmentCount { get; private set; }
	
	internal ClusterSeeker Seeker { get; }

	public ReadOnlySpan<byte> GetFragment(long index) {
		Guard.ArgumentInRange(index, 0, _parent.Clusters.Count - 1, nameof(index));
		return _parent.FastReadClusterData(index, 0, _parent.ClusterSize);
	}

	public void MapStreamPosition(long position, out long fragmentID, out long fragmentPosition) {
		var logicalClusterIndex = position / _parent.ClusterSize;
		Seeker.SeekTo(logicalClusterIndex);
		fragmentID = Seeker.CurrentCluster.Value;
		fragmentPosition = position % _parent.ClusterSize;
	}

	public void UpdateFragment(long fragmentID, long fragmentPosition, ReadOnlySpan<byte> updateSpan) {
		_parent.FastWriteClusterData(fragmentID, fragmentPosition, updateSpan);		
	}

	public void SetTotalBytes(long length) {
		var oldLength = TotalBytes;
		var newTotalClusters = _parent.CalculateClusterChainLength(length);
		var oldTotalClusters = FragmentCount;
		var currentTotalClusters = oldTotalClusters;
		
		// allocate/deallocate clusters are required
		if (newTotalClusters > currentTotalClusters) {
			if (currentTotalClusters == 0) 
				_parent.NewClusterChain(newTotalClusters, LogicalRecordID);
			else 
				_parent.AppendClustersToEnd(Seeker.EndCluster, newTotalClusters - currentTotalClusters);
		} else if (newTotalClusters < currentTotalClusters) {
			_parent.RemoveBackwards(Seeker.EndCluster, currentTotalClusters - newTotalClusters);
		}
		
		TotalBytes = length;
		FragmentCount = newTotalClusters;

		// Erase unused portion of tip cluster when shrinking stream
		if (length < oldLength) {
			var unusedTipClusterBytes = newTotalClusters * _parent.ClusterSize - length;
			if (unusedTipClusterBytes > 0) {
				var unusedTipClusterBytesI = Tools.Collection.CheckNotImplemented64bitAddressingLength(unusedTipClusterBytes);
				_parent.FastWriteClusterData(Seeker.EndCluster, _parent.ClusterSize - unusedTipClusterBytes, _parent.ZeroClusterBytes.AsSpan().Slice(..unusedTipClusterBytesI));
			}
		}

	}

	private void NotifyStreamLengthChanged(long newLength) => StreamLengthChanged?.Invoke(this, newLength);

	public void Dispose() {
		Seeker.Dispose();
	}
}
