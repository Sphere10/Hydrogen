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
			Record.Size,
			Record.StartCluster,
			Record.EndCluster,
			clusteredStorage.ClusterMap.CalculateClusterChainLength(Record.Size)
		);

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

	public void ProcessClusterMapChanged(ClusterMapChangedEventArgs changedEvent) {
		
		// Track any changes to the record's start/end cluster arising from migrating tips
		if (changedEvent.MovedTerminals.TryGetValue(RecordIndex, out var newTerminal)) {
			if (newTerminal.NewStart.HasValue)
				Record.StartCluster = newTerminal.NewStart.Value;
			
			if (newTerminal.NewEnd.HasValue)
				Record.EndCluster = newTerminal.NewEnd.Value;
		}

		if (changedEvent.ChainTerminal == RecordIndex) {
			if (changedEvent.AddedChain) {
				Record.StartCluster = changedEvent.ChainNewStartCluster.Value;
				Record.EndCluster = changedEvent.ChainNewEndCluster.Value;
				// Size is determined by fragment provider event
			} else if (changedEvent.RemovedChain) {
				Record.StartCluster = Cluster.Null;
				Record.EndCluster = Cluster.Null;
				Record.Size = 0;
			} else if (changedEvent.IncreasedChainSize || changedEvent.DecreasedChainSize) {
				Record.EndCluster = changedEvent.ChainNewEndCluster.Value;
			}
		}
	
		// Inform fragment provider of the changes
		_fragmentProvider.ProcessClusterMapChanged(changedEvent);
	}

	public void ProcessRecordSwapped(long record1Index, ClusteredStreamRecord record1Data, long record2Index, ClusteredStreamRecord record2Data) {
		if (RecordIndex == record1Index) {
			RecordIndex = record2Index;
			_fragmentProvider.LogicalRecordID = record2Index;
		}
		else if (RecordIndex == record2Index) {
			RecordIndex = record1Index;
			_fragmentProvider.LogicalRecordID = record1Index;
		}
		_fragmentProvider.ProcessRecordSwapped(record1Index, record1Data, record2Index, record2Data);
	}

	public void Dispose() {
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

}
