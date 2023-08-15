using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hydrogen;

internal class ClusteredStreamFragmentProvider : IStreamFragmentProvider, IDisposable {
	public event EventHandlerEx<ClusteredStreamFragmentProvider, long> StreamLengthChanged;

	private readonly IClusterMap _parent;

	private long _totalBytes;

	public ClusteredStreamFragmentProvider(IClusterMap clusteredMap, long logicalRecordID, long totalBytes, Func<ClusterChain> clusterChainLoader) {
		_parent = clusteredMap;
		_totalBytes = totalBytes;
		FragmentCount = clusteredMap.CalculateClusterChainLength(totalBytes);	
		Seeker = new ClusterSeeker(clusteredMap, logicalRecordID, clusterChainLoader);
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

	private void CheckNotSurgery() {
		Guard.Ensure(!_parent.PreventClusterNavigation, "Cannot perform this operation while clusters are being surgically modified.");
	}

	public ReadOnlySpan<byte> GetFragment(long index) {
		CheckNotSurgery();
		Guard.ArgumentInRange(index, 0, _parent.Clusters.Count - 1, nameof(index));
		return _parent.FastReadClusterData(index, 0, _parent.ClusterSize);
	}

	public void MapStreamPosition(long position, out long fragmentID, out long fragmentPosition) {
		CheckNotSurgery();
		var logicalClusterIndex = position / _parent.ClusterSize;
		Seeker.SeekTo(logicalClusterIndex);
		fragmentID = Seeker.ClusterPointer.Value.CurrentCluster.Value;
		fragmentPosition = position % _parent.ClusterSize;
	}

	public void UpdateFragment(long fragmentID, long fragmentPosition, ReadOnlySpan<byte> updateSpan) {
		CheckNotSurgery();
		_parent.FastWriteClusterData(fragmentID, fragmentPosition, updateSpan);		
	}

	public void SetTotalBytes(long length) {
		CheckNotSurgery();
		var oldLength = TotalBytes;
		var newTotalClusters = _parent.CalculateClusterChainLength(length);
		var oldTotalClusters = FragmentCount;
		var currentTotalClusters = oldTotalClusters;
		
		// allocate/deallocate clusters are required
		if (newTotalClusters > currentTotalClusters) {
			if (currentTotalClusters == 0) 
				_parent.NewClusterChain(newTotalClusters, LogicalRecordID);
			else 
				_parent.AppendClustersToEnd(Seeker.ClusterPointer.Value.Chain.EndCluster, newTotalClusters - currentTotalClusters);
		} else if (newTotalClusters < currentTotalClusters) {
			_parent.RemoveBackwards(Seeker.ClusterPointer.Value.Chain.EndCluster, currentTotalClusters - newTotalClusters);
		}

		TotalBytes = length;
		FragmentCount = newTotalClusters;

		// Erase unused portion of tip cluster when shrinking stream
		if (length < oldLength) {
			var unusedTipClusterBytes = newTotalClusters * _parent.ClusterSize - length;
			if (unusedTipClusterBytes > 0) {
				var unusedTipClusterBytesI = Tools.Collection.CheckNotImplemented64bitAddressingLength(unusedTipClusterBytes);
				_parent.FastWriteClusterData(Seeker.ClusterPointer.Value.Chain.EndCluster, _parent.ClusterSize - unusedTipClusterBytes, _parent.ZeroClusterBytes.AsSpan().Slice(..unusedTipClusterBytesI));
			}
		}

	}

	private void NotifyStreamLengthChanged(long newLength) => StreamLengthChanged?.Invoke(this, newLength);

	public void Dispose() {
		CheckNotSurgery();
		Seeker.Dispose();
	}
}
