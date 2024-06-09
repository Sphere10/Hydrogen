// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace Hydrogen;

/// <summary>
/// A <see cref="Stream"/> whose contents are clustered across a <see cref="ClusterMap"/>. Instances of <see cref="ClusteredStream"/> are
/// the component streams managed by a <see cref="ClusteredStreams"/>. A <see cref="ClusteredStream"/> is for all intensive purposes to be
/// considered a usual <see cref="Stream"/> except it's contents (and <see cref="ClusteredStreamDescriptor"/>) managed entirely by it's
/// owning <see cref="ClusteredStreams"/>.
/// </summary>
public class ClusteredStream : StreamDecorator {
	public event EventHandlerEx<long> StreamLengthChanged;

	private readonly ClusteredStreams _streams;
	private readonly ClusteredStreamFragmentProvider _fragmentProvider;
	private ClusteredStreamDescriptor _descriptor;
	private readonly Action _finalizeAction;
	private readonly Disposables  _disposables;

	internal ClusteredStream(ClusteredStreams streams, long streamIndex, bool readOnly, Action finalizeAction = null)
		: base(CreateInternalStream(streams, streamIndex, readOnly, out var streamDescriptor, out var fragmentProvider)) {
		_streams = streams;
		_finalizeAction = finalizeAction;
		StreamIndex = streamIndex;
		_descriptor = streamDescriptor;
		ReadOnly = readOnly;
		_fragmentProvider = fragmentProvider;
		_disposables = new();
		// track when stream length changes so we can update the scope's descriptor
		if (!readOnly) {
			_fragmentProvider.StreamLengthChanged += (_, length) => {
				_descriptor.Size = length;
				NotifyStreamLengthChanged(length);
			};
		}
	}

	public bool ReadOnly { get; }

	public long StreamIndex { get; private set;}

	public ClusteredStreamTraits Traits => _descriptor.Traits;

	public void AddFinalizer(Action action) => _disposables.Add(action);

	public bool IsNull { 
		get => _descriptor.Traits.HasFlag(ClusteredStreamTraits.Null); 
		set => _descriptor.Traits = _descriptor.Traits.CopyAndSetFlags(ClusteredStreamTraits.Null, value);
	}

	public bool IsReaped { 
		get => _descriptor.Traits.HasFlag(ClusteredStreamTraits.Reaped); 
		set => _descriptor.Traits = _descriptor.Traits.CopyAndSetFlags(ClusteredStreamTraits.Reaped, value);
	}

	public void ProcessClusterMapChanged(ClusterMapChangedEventArgs changedEvent) {
		
		// Track any changes to the descriptor's start/end cluster arising from migrating tips
		if (changedEvent.MovedTerminals.TryGetValue(StreamIndex, out var newTerminal)) {
			if (newTerminal.NewStart.HasValue)
				_descriptor.StartCluster = newTerminal.NewStart.Value;
			
			if (newTerminal.NewEnd.HasValue)
				_descriptor.EndCluster = newTerminal.NewEnd.Value;
		}

		if (changedEvent.ChainTerminal == StreamIndex) {
			if (changedEvent.AddedChain) {
				_descriptor.StartCluster = changedEvent.ChainNewStartCluster.Value;
				_descriptor.EndCluster = changedEvent.ChainNewEndCluster.Value;
				// Size is determined by fragment provider event
			} else if (changedEvent.RemovedChain) {
				_descriptor.StartCluster = Cluster.Null;
				_descriptor.EndCluster = Cluster.Null;
				_descriptor.Size = 0;
			} else if (changedEvent.IncreasedChainSize || changedEvent.DecreasedChainSize) {
				_descriptor.EndCluster = changedEvent.ChainNewEndCluster.Value;
			}
		}
	
		// Inform fragment provider of the changes
		_fragmentProvider.ProcessClusterMapChanged(changedEvent);
	}

	public void ProcessStreamSwapped(long stream1, ClusteredStreamDescriptor streamDescriptor1, long stream2, ClusteredStreamDescriptor stream2Descriptor) {
		if (StreamIndex == stream1) {
			StreamIndex = stream2;
			_fragmentProvider.Terminal = stream2;
		}
		else if (StreamIndex == stream2) {
			StreamIndex = stream1;
			_fragmentProvider.Terminal = stream1;
		}
		_fragmentProvider.ProcessStreamSwapped(stream1, streamDescriptor1, stream2, stream2Descriptor);
	}
	
	public override void Close() {
		// Close() is called by Dispose
		if (!ReadOnly)
			_streams.UpdateStreamDescriptor(StreamIndex, _descriptor);
		
		_finalizeAction?.Invoke();
		_disposables.ForEach(d => d.Dispose());
		

#if ENABLE_CLUSTER_DIAGNOSTICS
		ClusterDiagnostics.VerifyClusters(_streamContainer.ClusterMap);
#endif
		base.Close();
	}

	private void NotifyStreamLengthChanged(long newSize) {
		StreamLengthChanged?.Invoke(newSize);
	}

	private static Stream CreateInternalStream(ClusteredStreams streams, long streamIndex, bool readOnly, out ClusteredStreamDescriptor streamDescriptor, out ClusteredStreamFragmentProvider fragmentProvider) {
		streamDescriptor = streams.GetStreamDescriptor(streamIndex);
		fragmentProvider = new ClusteredStreamFragmentProvider(
			streams.ClusterMap, 
			streamIndex, 
			streamDescriptor.Size,
			streamDescriptor.StartCluster,
			streamDescriptor.EndCluster,
			streams.ClusterMap.CalculateClusterChainLength(streamDescriptor.Size),
			streams.Policy.HasFlag(ClusteredStreamsPolicy.IntegrityChecks)
		);
		Stream stream = new FragmentedStream(fragmentProvider);
		if (readOnly)
			stream = stream.AsReadOnly();
		return stream;
	}

}
