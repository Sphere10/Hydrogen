using System;

namespace Hydrogen;

internal class ClusteredStreamFragmentProvider : IStreamFragmentProvider {
	public event EventHandlerEx<ClusteredStreamFragmentProvider, long> StreamLengthChanged;

	private readonly ClusterMap _parent;

	private long _totalBytes;

	public ClusteredStreamFragmentProvider(ClusterMap clusteredMap, long logicalRecordID, long totalBytes, long startCluster, long endCluster, long totalClusters) {
		_parent = clusteredMap;
		_totalBytes = totalBytes;
		FragmentCount = clusteredMap.CalculateClusterChainLength(totalBytes);	
		Seeker = new ClusterSeeker(clusteredMap, logicalRecordID, startCluster, endCluster, totalClusters);
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
		
		// allocate/deallocate clusters are required
		if (newTotalClusters > currentTotalClusters) {
			if (currentTotalClusters == 0) 
				_parent.NewClusterChain(newTotalClusters, LogicalRecordID);
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

	public void ProcessRecordSwapped(long record1Index, ClusteredStreamRecord record1Data, long record2Index, ClusteredStreamRecord record2Data) {
		if (LogicalRecordID == record1Index) {
			LogicalRecordID = record2Index;
		}
		else if (LogicalRecordID == record2Index) {
			LogicalRecordID = record1Index;
		}
		Seeker.ProcessRecordSwapped(record1Index, record1Data, record2Index, record2Data);
	}

	private void CheckNotEmpty() {
		Guard.Ensure(TotalBytes > 0, "Stream is empty.");
	}

	private void NotifyStreamLengthChanged(long newLength) => StreamLengthChanged?.Invoke(this, newLength);
}
