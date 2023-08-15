using System;
using System.IO;



namespace Hydrogen;

public class ClusteredStreamScope : IDisposable {
	public event EventHandlerEx<long> RecordSizeChanged;

	private readonly ClusteredStorage _clusteredStorage;
	private readonly ClusteredStreamFragmentProvider _fragmentProvider;
	private readonly Action _finalizeAction;

	internal ClusteredStreamScope(ClusteredStorage clusteredStorage, long recordIndex, bool readOnly, Action finalizeAction = null) {
		_clusteredStorage = clusteredStorage;
		_finalizeAction = finalizeAction;
		RecordIndex = recordIndex;
		Record = clusteredStorage.GetRecord(recordIndex);
		ReadOnly = readOnly;
		_fragmentProvider = new ClusteredStreamFragmentProvider(
			_clusteredStorage.ClusterMap, 
			recordIndex, 
			Record.Size, () =>  {
				var rec = clusteredStorage.GetRecord(recordIndex);
				return new ClusterChain { StartCluster = rec.StartCluster, EndCluster = rec.EndCluster, TotalClusters = clusteredStorage.ClusterMap.CalculateClusterChainLength(rec.Size)};
			}
		);

		// subscribing to the container events ensures that any shuffles of this record start/end cluster (caused by another opened stream)
		// will necessarily update this scope's record
		_clusteredStorage.ClusterMap.ClusterChainCreated += ClusterChainCreatedHandler;
		_clusteredStorage.ClusterMap.ClusterChainRemoved += ClusterChainRemovedHandler;
		_clusteredStorage.ClusterMap.ClusterChainStartChanged += ClusterChainStartChangedHandler;
		_clusteredStorage.ClusterMap.ClusterChainEndChanged += ClusterChainEndChangedHandler;
		_clusteredStorage.ClusterMap.ClusterMoved += ClusterMovedHandler;
		_clusteredStorage.RecordSwapped += RecordSwappedHandler;

		// track when stream length changes so we can update the scope's record
		if (!readOnly) {
			_fragmentProvider.StreamLengthChanged += (_, length) => {
				Record.Size = length;
				NotifyRecordSizeChanged(length);
			};
		}

		Stream = new FragmentedStream(_fragmentProvider);
		if (readOnly)
			Stream = Stream.AsReadOnly();
	}



	public bool ReadOnly { get; }

	public long RecordIndex { get; private set;}

	public ClusteredStreamRecord Record; // TODO: MAKE PROPERTY (check won't break when is struct)

	public Stream Stream { get; }

	public void Dispose() {
		_clusteredStorage.ClusterMap.ClusterChainCreated -= ClusterChainCreatedHandler;
		_clusteredStorage.ClusterMap.ClusterChainRemoved -= ClusterChainRemovedHandler;
		_clusteredStorage.ClusterMap.ClusterChainStartChanged -= ClusterChainStartChangedHandler;
		_clusteredStorage.ClusterMap.ClusterChainEndChanged -= ClusterChainEndChangedHandler;
		_clusteredStorage.ClusterMap.ClusterMoved -= ClusterMovedHandler;
		_clusteredStorage.RecordSwapped -= RecordSwappedHandler;
		Stream.Dispose();
		if (!ReadOnly) {
			_clusteredStorage.UpdateRecord(RecordIndex, Record);
		}
		_finalizeAction?.Invoke();
		#if ENABLE_CLUSTER_DIAGNOSTICS
		ClusterDiagnostics.VerifyClusters(_clusteredStorage.ClusterMap);
		#endif
	}

	private void NotifyRecordSizeChanged(long newSize) {
		RecordSizeChanged?.Invoke(newSize);
	}

	private void ClusterChainCreatedHandler(object sender, long startCluster, long endCluster, long totalClusters, long terminalValue) {
		if (terminalValue == RecordIndex) {
			Record.StartCluster = startCluster;
			Record.EndCluster = endCluster;
			// Size is determined by fragment provider event
		}
	}

	private void ClusterChainRemovedHandler(object sender, long terminalValue) {
		if (terminalValue == RecordIndex) {
			Record.StartCluster = -1;
			Record.EndCluster = -1;
			Record.Size = 0;
		}
	}

	private void ClusterChainStartChangedHandler(object sender, long cluster, long terminalValue, long clusterCountDelta) {
		if (terminalValue == RecordIndex) {
			Record.StartCluster = cluster; // update local record copy with new start cluster
		}
	}

	private void ClusterChainEndChangedHandler(object sender, long cluster, long terminalValue, long clusterCountDelta) {
		if (terminalValue == RecordIndex) {
			Record.EndCluster = cluster; // update local record copy with new end cluster
		}
	}
	
	private void ClusterMovedHandler(object sender, long fromCluster, long toCluster, ClusterTraits traits, long? terminalValue) {
		if (terminalValue == RecordIndex) {
			if (traits.HasFlag(ClusterTraits.Start)) {
				Record.StartCluster = toCluster;
			}
			if (traits.HasFlag(ClusterTraits.End)) {
				Record.EndCluster = toCluster;
			}
		}
	}

	private void RecordSwappedHandler((long index, ClusteredStreamRecord data) record1, (long index, ClusteredStreamRecord data) record2) {
		if (RecordIndex == record1.index) {
			RecordIndex = record2.index;
			_fragmentProvider.LogicalRecordID = record2.index;
		}
		else if (RecordIndex == record2.index) {
			RecordIndex = record1.index;
			_fragmentProvider.LogicalRecordID = record1.index;
		}
	}

}
