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
	/// <summary>
	/// Raised when the logical length of this stream changes as clusters are appended/truncated.
	/// </summary>
	/// <remarks>
	/// This event is emitted from the underlying <see cref="ClusteredStreamFragmentProvider"/> and is only wired for writable streams.
	/// </remarks>
 	public event EventHandlerEx<long> StreamLengthChanged;
 
 	private readonly ClusteredStreams _streams;
 	private readonly ClusteredStreamFragmentProvider _fragmentProvider;
 	private ClusteredStreamDescriptor _descriptor;
 	private readonly Action _finalizeAction;
 	private readonly Disposables  _disposables;
 
	/// <summary>
	/// Creates a clustered stream wrapper for the specified terminal stream index.
	/// </summary>
	/// <param name="streams">Owning <see cref="ClusteredStreams"/> manager.</param>
	/// <param name="streamIndex">The terminal identifier used by the <see cref="ClusterMap"/> to locate this stream's cluster chain.</param>
	/// <param name="readOnly">Whether the returned stream should be read-only.</param>
	/// <param name="finalizeAction">
	/// Optional action invoked during <see cref="Close"/> after any descriptor update but before underlying resources are disposed.
	/// </param>
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
 
	/// <summary>
	/// Gets a value indicating whether this stream was opened in read-only mode.
	/// </summary>
 	public bool ReadOnly { get; }
 
	/// <summary>
	/// Gets the stream's current terminal index within the owning <see cref="ClusteredStreams"/> container.
	/// </summary>
	/// <remarks>
	/// This value may change when streams are swapped internally (see <see cref="ProcessStreamSwapped"/>).
	/// </remarks>
 	public long StreamIndex { get; private set;}
 
	/// <summary>
	/// Gets the trait flags associated with this stream as stored in its <see cref="ClusteredStreamDescriptor"/>.
	/// </summary>
 	public ClusteredStreamTraits Traits => _descriptor.Traits;
 
	/// <summary>
	/// Registers an action to be executed when this stream is closed/disposed.
	/// </summary>
	/// <param name="action">The finalizer action to run.</param>
	/// <remarks>
	/// Finalizers are executed from <see cref="Close"/> after descriptor persistence (if applicable).
	/// </remarks>
 	public void AddFinalizer(Action action) => _disposables.Add(action);
 
	/// <summary>
	/// Gets or sets whether this stream is logically considered <c>null</c> within the clustered stream container.
	/// </summary>
 	public bool IsNull { 
 		get => _descriptor.Traits.HasFlag(ClusteredStreamTraits.Null); 
 		set => _descriptor.Traits = _descriptor.Traits.CopyAndSetFlags(ClusteredStreamTraits.Null, value);
 	}
 
	/// <summary>
	/// Gets or sets whether this stream has been marked as reaped (logically deleted) within the clustered stream container.
	/// </summary>
 	public bool IsReaped { 
 		get => _descriptor.Traits.HasFlag(ClusteredStreamTraits.Reaped); 
 		set => _descriptor.Traits = _descriptor.Traits.CopyAndSetFlags(ClusteredStreamTraits.Reaped, value);
 	}
 
	/// <summary>
	/// Applies a <see cref="ClusterMap"/> change notification to this stream's descriptor and fragment provider.
	/// </summary>
	/// <param name="changedEvent">The change details emitted by the owning <see cref="ClusterMap"/>.</param>
	/// <remarks>
	/// This keeps the local <see cref="ClusteredStreamDescriptor"/> in sync with cluster migrations and chain changes so that,
	/// on close, the descriptor persisted back to <see cref="ClusteredStreams"/> reflects the current start/end clusters.
	/// </remarks>
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
 
	/// <summary>
	/// Updates this instance to reflect a stream swap operation performed by the owning <see cref="ClusteredStreams"/>.
	/// </summary>
	/// <param name="stream1">First stream index involved in the swap.</param>
	/// <param name="streamDescriptor1">Descriptor for <paramref name="stream1"/>.</param>
	/// <param name="stream2">Second stream index involved in the swap.</param>
	/// <param name="stream2Descriptor">Descriptor for <paramref name="stream2"/>.</param>
	/// <remarks>
	/// Swaps can change which terminal index this instance represents; the fragment provider is updated accordingly.
	/// </remarks>
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
 	
	/// <summary>
	/// Closes the stream and persists any descriptor changes back to the owning <see cref="ClusteredStreams"/> (when writable).
	/// </summary>
	/// <remarks>
	/// <see cref="Close"/> is invoked by <see cref="Stream.Dispose()"/>; descriptor updates are deferred until here to
	/// avoid churn while the stream is being mutated.
	/// </remarks>
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
 
	/// <summary>
	/// Builds the internal stream pipeline used by <see cref="ClusteredStream"/> for a given descriptor.
	/// </summary>
	/// <param name="streams">Owning <see cref="ClusteredStreams"/> container.</param>
	/// <param name="streamIndex">Terminal identifier for the stream.</param>
	/// <param name="readOnly">Whether the internal stream should be wrapped as read-only.</param>
	/// <param name="streamDescriptor">Returns the descriptor used to initialize the fragment provider.</param>
	/// <param name="fragmentProvider">Returns the fragment provider responsible for mapping stream offsets to clusters.</param>
	/// <returns>A stream whose reads/writes are translated into cluster chain operations.</returns>
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