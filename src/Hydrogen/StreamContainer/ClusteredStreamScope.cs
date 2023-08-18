using System;
using System.IO;

namespace Hydrogen;

/// <summary>
/// A <see cref="Stream"/> whose contents are clustered across a <see cref="ClusterMap"/>. Instances of <see cref="ClusteredStream"/> are
/// the component streams managed by a <see cref="StreamContainer"/>. A <see cref="ClusteredStream"/> is for all intensive purposes to be
/// considered a usual <see cref="Stream"/> except it's contents (and <see cref="ClusteredStreamDescriptor"/>) managed entirely by it's
/// owning <see cref="StreamContainer"/>.
/// </summary>
public class ClusteredStream : StreamDecorator {
	public event EventHandlerEx<long> StreamLengthChanged;

	private readonly StreamContainer _streamContainer;
	private readonly ClusteredStreamFragmentProvider _fragmentProvider;
	private readonly Action _finalizeAction;

	internal ClusteredStream(StreamContainer streamContainer, long recordIndex, bool readOnly, Action finalizeAction = null)
		: base(CreateInternalStream(streamContainer, recordIndex, readOnly, out var streamDescriptor, out var fragmentProvider)) {
		_streamContainer = streamContainer;
		_finalizeAction = finalizeAction;
		RecordIndex = recordIndex;
		Descriptor = streamDescriptor;
		ReadOnly = readOnly;
		_fragmentProvider = fragmentProvider;

		// track when stream length changes so we can update the scope's descriptor
		if (!readOnly) {
			_fragmentProvider.StreamLengthChanged += (_, length) => {
				Descriptor.Size = length;
				NotifyStreamLengthChanged(length);
			};
		}
	}

	public bool ReadOnly { get; }

	public long RecordIndex { get; private set;}

	public ClusteredStreamDescriptor Descriptor; // TODO: MAKE PROPERTY (check won't break when is struct)

	public void ProcessClusterMapChanged(ClusterMapChangedEventArgs changedEvent) {
		
		// Track any changes to the descriptor's start/end cluster arising from migrating tips
		if (changedEvent.MovedTerminals.TryGetValue(RecordIndex, out var newTerminal)) {
			if (newTerminal.NewStart.HasValue)
				Descriptor.StartCluster = newTerminal.NewStart.Value;
			
			if (newTerminal.NewEnd.HasValue)
				Descriptor.EndCluster = newTerminal.NewEnd.Value;
		}

		if (changedEvent.ChainTerminal == RecordIndex) {
			if (changedEvent.AddedChain) {
				Descriptor.StartCluster = changedEvent.ChainNewStartCluster.Value;
				Descriptor.EndCluster = changedEvent.ChainNewEndCluster.Value;
				// Size is determined by fragment provider event
			} else if (changedEvent.RemovedChain) {
				Descriptor.StartCluster = Cluster.Null;
				Descriptor.EndCluster = Cluster.Null;
				Descriptor.Size = 0;
			} else if (changedEvent.IncreasedChainSize || changedEvent.DecreasedChainSize) {
				Descriptor.EndCluster = changedEvent.ChainNewEndCluster.Value;
			}
		}
	
		// Inform fragment provider of the changes
		_fragmentProvider.ProcessClusterMapChanged(changedEvent);
	}

	public void ProcessStreamSwapped(long stream1, ClusteredStreamDescriptor streamDescriptor1, long stream2, ClusteredStreamDescriptor stream2Descriptor) {
		if (RecordIndex == stream1) {
			RecordIndex = stream2;
			_fragmentProvider.Terminal = stream2;
		}
		else if (RecordIndex == stream2) {
			RecordIndex = stream1;
			_fragmentProvider.Terminal = stream1;
		}
		_fragmentProvider.ProcessStreamSwapped(stream1, streamDescriptor1, stream2, stream2Descriptor);
	}

	// Close() is called by Dispose
	public override void Close() {
	
		if (!ReadOnly) {
			_streamContainer.UpdateStreamDescriptor(RecordIndex, Descriptor);
		}
		_finalizeAction?.Invoke();
#if ENABLE_CLUSTER_DIAGNOSTICS
		ClusterDiagnostics.VerifyClusters(_streamContainer.ClusterMap);
#endif
		base.Close();
	}

	private void NotifyStreamLengthChanged(long newSize) {
		StreamLengthChanged?.Invoke(newSize);
	}

	private static Stream CreateInternalStream(StreamContainer streamContainer, long streamIndex, bool readOnly, out ClusteredStreamDescriptor streamDescriptor, out ClusteredStreamFragmentProvider fragmentProvider) {
		streamDescriptor = streamContainer.GetStreamDescriptor(streamIndex);
		fragmentProvider = new ClusteredStreamFragmentProvider(
			streamContainer.ClusterMap, 
			streamIndex, 
			streamDescriptor.Size,
			streamDescriptor.StartCluster,
			streamDescriptor.EndCluster,
			streamContainer.ClusterMap.CalculateClusterChainLength(streamDescriptor.Size),
			streamContainer.Policy.HasFlag(StreamContainerPolicy.IntegrityChecks)
		);
		Stream stream = new FragmentedStream(fragmentProvider);
		if (readOnly)
			stream = stream.AsReadOnly();
		return stream;
	}

}
