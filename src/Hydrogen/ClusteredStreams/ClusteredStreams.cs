// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;


namespace Hydrogen;

/// <summary>
/// A collection of <see cref="Stream"/>'s which are multiplexed onto a single <see cref="Stream"/> using a cluster-based approach similar in principle to how a file-system works.
/// This stream permits the basis of a "virtual file system" allowing any arbitrary number of <see cref="Stream"/>'s to be stored in a sequential manner, and arbitrarily updated both in content
/// and size. This class (and <see cref="ObjectStream{T}"/>) serve a vital component of <see cref="IStreamMappedList{TItem}"/>'s, <see cref="IStreamMappedDictionary{TKey,TValue}"/>'s and <see cref="IStreamMappedHashSet{TItem}"/>'s
/// implementations.
/// <remarks>
/// The structure of the root clustered stream is depicted below:
/// [ClusteredStreamsHeader] Version: 1, ClusterSize: 4, TotalClusters: 17, StreamCount: 2, StreamDescriptorsEndCluster: 14, ReservedStreams: 0, Policy: 0
/// [Stream Descriptors]:
/// 0: [ClusteredStreamDescriptor] Size: 5, StartCluster: 7, EndCluster: 8, Traits: Default, KeyChecksum: 0, Key: 
/// 1: [ClusteredStreamDescriptor] Size: 5, StartCluster: 15, EndCluster: 16, Traits: Default, KeyChecksum: 0, Key: 
/// [Cluster Map]:
/// 0: [Cluster] Traits: Start, Prev: -1, Next: 1, Data: 00070000
/// 1: [Cluster] Traits: None, Prev: 0, Next: 2, Data: 00000000
/// 2: [Cluster] Traits: None, Prev: 1, Next: 3, Data: 00080000
/// 3: [Cluster] Traits: None, Prev: 2, Next: 4, Data: 00000000
/// 4: [Cluster] Traits: None, Prev: 3, Next: 5, Data: 00050000
/// 5: [Cluster] Traits: None, Prev: 4, Next: 6, Data: 00000000
/// 6: [Cluster] Traits: None, Prev: 5, Next: 9, Data: 00000f00
/// 7: [Cluster] Traits: Start, Prev: 0, Next: 8, Data: 00010203
/// 8: [Cluster] Traits: End, Prev: 7, Next: 0, Data: 04000000
/// 9: [Cluster] Traits: None, Prev: 6, Next: 10, Data: 00000000
/// 10: [Cluster] Traits: None, Prev: 9, Next: 11, Data: 00001000
/// 11: [Cluster] Traits: None, Prev: 10, Next: 12, Data: 00000000
/// 12: [Cluster] Traits: None, Prev: 11, Next: 13, Data: 00000500
/// 13: [Cluster] Traits: None, Prev: 12, Next: 14, Data: 00000000
/// 14: [Cluster] Traits: End, Prev: 13, Next: -1, Data: 00000000
/// 15: [Cluster] Traits: Start, Prev: 1, Next: 16, Data: 05060708
/// 16: [Cluster] Traits: End, Prev: 15, Next: 1, Data: 09000000
///  
///  Notes:
///  - Header is fixed 256b and has space for user-defined fields, such as merkle-root's and encryption keys.
///  - Has pluggable <see cref="IClusteredStreamsAttachment"/> which mutate with stream operations (used for indexing, merkle-tree's, etc)
///  - Meta-data based streams are saved as "Reserved Streams" which are always open.
///  - Cluster chains start with a cluster marked Start and end with a cluster marked End.
///  - Cluster chains are bi-directionally linked, for forward/backward seeking.
///  - A Start cluster's "previous" link, since it is unused, is called a "Terminal" value instead denotes the index of the stream within the collection.
///  - Similarly, an End cluster's "end" pointer is also a Terminal value and instead denotes index of the stream within the index .
///  - All streams are stored in a single cluster chain.
///  - Stream Descriptors are meta-data describing a Stream, where they start/end and other info.
///  - Stream Descriptors are are stored in a cluster chain starting at cluster 0 having Terminal -1.
/// </remarks>
public class ClusteredStreams : SyncLoadableBase, ICriticalObject, IDisposable {
	public event EventHandlerEx<ClusteredStreamDescriptor> StreamCreated;
	public event EventHandlerEx<long, ClusteredStreamDescriptor> StreamAdded;
	public event EventHandlerEx<long, ClusteredStreamDescriptor> StreamInserted;
	public event EventHandlerEx<long, ClusteredStreamDescriptor> StreamUpdated;
	public event EventHandlerEx<(long, ClusteredStreamDescriptor), (long, ClusteredStreamDescriptor)> StreamSwapped;
	public event EventHandlerEx<long, long> StreamLengthChanged;
	public event EventHandlerEx<long> StreamRemoved;
	public event EventHandlerEx Cleared;
	public event EventHandlerEx Clearing;
	public event EventHandlerEx ReservedStreamsCreated;
	public event EventHandlerEx ReservedStreamsCreating;

	private StreamMappedClusterMap _clusters;
	private ClusteredStreamFragmentProvider _streamDescriptorsFragmentProvider;
	private UpdateOnlyList<ClusteredStreamDescriptor, StreamPagedList<ClusteredStreamDescriptor>> _streamDescriptors;
	private ICache<long, ClusteredStreamDescriptor> _streamDescriptorCache;
	private ClusteredStreamsHeader _header;
	private readonly IDictionary<long, ClusteredStream> _openStreams;
	private readonly DictionaryList<string, IClusteredStreamsAttachment> _attachments;
	private readonly List<Action> _initActions;

	private readonly ConcurrentStream _rootStream;
	private readonly int _clusterSize;
	private readonly long _reservedStreams;
	private readonly bool _integrityChecks;
	private bool _suppressEvents;

	public ClusteredStreams(
		Stream rootStream,
		int clusterSize = HydrogenDefaults.ClusterSize,
		ClusteredStreamsPolicy policy = ClusteredStreamsPolicy.Default,
		long reservedStreams = 0,
		Endianness endianness = Endianness.LittleEndian,
		bool autoLoad = false
	) {
		Guard.ArgumentNotNull(rootStream, nameof(rootStream));
		Guard.ArgumentGTE(clusterSize, 1, nameof(clusterSize));
		Guard.ArgumentGTE(reservedStreams, 0, nameof(reservedStreams));
		Policy = policy;
		Endianness = endianness;
		_clusters = null;
		_streamDescriptorsFragmentProvider = null;
		_streamDescriptors = null;
		_streamDescriptorCache = null;
		_header = null;
		_openStreams = new Dictionary<long, ClusteredStream>();
		_attachments = new DictionaryList<string, IClusteredStreamsAttachment>();
		_initActions = new List<Action>();
		Initialized = false;
		_rootStream = rootStream.AsConcurrent();
		_clusterSize = clusterSize;
		_reservedStreams = reservedStreams;
		_integrityChecks = Policy.HasFlag(ClusteredStreamsPolicy.IntegrityChecks);
		_suppressEvents = false;

		if (autoLoad && RequiresLoad)
			Load();
	}

	public static ClusteredStreams FromStream(Stream rootStream, Endianness endianness = Endianness.LittleEndian, bool autoLoad = false) {
		Guard.Ensure(rootStream.Length >= ClusteredStreamsHeader.ByteLength, $"Corrupt header (stream was too small {rootStream.Length} bytes)");
		var reader = new EndianBinaryReader(EndianBitConverter.For(endianness), rootStream);

		// read cluster size
		rootStream.Seek(ClusteredStreamsHeader.ClusterSizeOffset, SeekOrigin.Begin);
		var clusterSize = reader.ReadInt32();
		Guard.Ensure(clusterSize > 0, $"Corrupt header (ClusterSize field was {clusterSize} bytes)");

		// read policy
		rootStream.Seek(ClusteredStreamsHeader.PolicyOffset, SeekOrigin.Begin);
		var policy = (ClusteredStreamsPolicy)reader.ReadUInt32();

		// read records offset
		rootStream.Seek(ClusteredStreamsHeader.ReservedStreamsOffset, SeekOrigin.Begin);
		var reservedStreams = reader.ReadInt64();

		rootStream.Position = 0;
		var storage = new ClusteredStreams(rootStream, clusterSize, policy, reservedStreams, endianness, autoLoad: autoLoad);

		return storage;
	}

	public ICriticalObject ParentCriticalObject { get => _rootStream.ParentCriticalObject; set => _rootStream.ParentCriticalObject = value; }

	public object Lock => _rootStream.Lock;

	public bool IsLocked => _rootStream.IsLocked;

	public bool Initialized { get; private set; }	

	public override bool RequiresLoad => !Initialized || base.RequiresLoad || RootStream is ILoadable { RequiresLoad: true };

	public bool OwnsStream { get; set; }

	public Stream RootStream => _rootStream;

	public ClusteredStreamsHeader Header {
		get {
			CheckInitialized();
			return _header;
		}
		private set => _header = value;
	}

	public ClusteredStreamsPolicy Policy { get; }

	public Endianness Endianness { get; }

	public long Count {
		get {
			CheckInitialized();
			return Header.StreamCount;
		}
	}

	public IReadOnlyDictionaryList<string, IClusteredStreamsAttachment> Attachments => _attachments;

	internal ClusterMap ClusterMap {
		get {
			CheckInitialized();
			return _clusters;
		}
	}

	internal int ClusterSize {
		get {
			CheckInitialized();
			return Header.ClusterSize;
		}
	}

	public bool SuppressEvents {
		get => _suppressEvents && _clusters.SuppressEvents;
		set {
			_suppressEvents = value;
			_clusters.SuppressEvents = value;
		}
	}

	#region Streams

	public IDisposable EnterAccessScope() {
		CheckInitialized();
		return
			_rootStream
			.EnterAccessScope();
	}

	public ClusteredStream Add() {
		CheckInitialized();
		using var accessScope = EnterAccessScope();
		AddStreamDescriptor(out var index, NewStreamDescriptor()); // the first descriptor add will allocate cluster 0 for the records stream
		return OpenStreamInternal(index, false, true);
	}

	public ClusteredStream OpenRead(long index) => Open(index, true, true);

	public ClusteredStream OpenWrite(long index) => Open(index, false, true);

	public ClusteredStream Open(long index, bool readOnly, bool acquireThreadLock) {
		CheckInitialized();
		using var accessScope = EnterAccessScope();
		CheckStreamDescriptorIndex(index);
		return OpenStreamInternal(index, readOnly, acquireThreadLock);
	}

	public void Remove(long index) {
		CheckInitialized();
		CheckNotReserved(index);
		using var accessScope = EnterAccessScope();
		CheckNoOpenedStreams(true);
		var countBeforeRemove = Header.StreamCount;
		CheckStreamDescriptorIndex(index);
		var record = GetStreamDescriptor(index);
		Guard.Against(record.Size == 0 && (record.StartCluster != Cluster.Null || record.EndCluster != Cluster.Null), "Invalid empty descriptor");
		if (record.Size > 0) {
			_clusters.RemoveNextClusters(record.StartCluster);
		}
		RemoveStreamDescriptor(index); // descriptor must be removed last, in case it deletes genesis cluster
		var countAfterRemove = Header.StreamCount;
		Guard.Ensure(countAfterRemove == countBeforeRemove - 1, $"Failed to remove descriptor {index}");
#if ENABLE_CLUSTER_DIAGNOSTICS
		ClusterDiagnostics.VerifyClusters(this);
#endif

	}

	public ClusteredStream Insert(long index) {
		CheckInitialized();
		CheckNotReserved(index);
		using var accessScope = EnterAccessScope();
		CheckNoOpenedStreams(true);
		CheckStreamDescriptorIndex(index, allowEnd: true);
		InsertStreamDescriptor(index, NewStreamDescriptor());
		return OpenStreamInternal(index, false, true);
	}

	public void Swap(long first, long second) {
		CheckInitialized();
		using var accessScope = EnterAccessScope();
		Guard.Ensure(!_openStreams.ContainsKey(first), $"Cannot swap stream {first} as it is open");
		Guard.Ensure(!_openStreams.ContainsKey(second), $"Cannot swap stream {second} as it is open");

		CheckStreamDescriptorIndex(first);
		CheckStreamDescriptorIndex(second);

		if (first == second)
			return;

		// Get records
		var firstDescriptor = GetStreamDescriptor(first);
		var secondDescriptor = GetStreamDescriptor(second);

		// Swap records
		UpdateStreamDescriptor(first, secondDescriptor);
		UpdateStreamDescriptor(second, firstDescriptor);

		// Update cluster chain terminals to match new descriptor backlinks  (if applicable)
		if (firstDescriptor.StartCluster != Cluster.Null)
			_clusters.WriteClusterPrev(firstDescriptor.StartCluster, second);
		if (firstDescriptor.EndCluster != Cluster.Null)
			_clusters.WriteClusterNext(firstDescriptor.EndCluster, second);
		if (secondDescriptor.StartCluster != Cluster.Null)
			_clusters.WriteClusterPrev(secondDescriptor.StartCluster, first);
		if (secondDescriptor.EndCluster != Cluster.Null)
			_clusters.WriteClusterNext(secondDescriptor.EndCluster, first);

		NotifyStreamSwapped(first, firstDescriptor, second, secondDescriptor);

#if ENABLE_CLUSTER_DIAGNOSTICS
		ClusterDiagnostics.VerifyClusters(this);
#endif
	}

	public void Clear(long index) {
		CheckInitialized();
		using var accessScope = EnterAccessScope();
		CheckStreamDescriptorIndex(index);
		using var stream = OpenWrite(index);
		stream.SetLength(0);

#if ENABLE_CLUSTER_DIAGNOSTICS
		ClusterDiagnostics.VerifyClusters(this);
#endif
	}

	public void Clear() {
		CheckInitialized();
		NotifyClearing(); // notify before checking opened streams to allow them to be closed
		using var accessScope = EnterAccessScope();
		CheckNoOpenedStreams(false);
		SuppressEvents = true;
		try {
			_streamDescriptors.Clear();
			_streamDescriptorCache?.Purge();
			_clusters.Clear();
			Header.StreamCount = 0;
			Header.TotalClusters = 0;
			Header.StreamDescriptorsEndCluster = Cluster.Null;
		} finally {
			SuppressEvents = false;
		}
		CreateReservedStreams();
#if ENABLE_CLUSTER_DIAGNOSTICS
		ClusterDiagnostics.VerifyClusters(this);
#endif
		NotifyCleared();
	}

	public void Optimize() {
		CheckInitialized();
		using var accessScope = EnterAccessScope();
		// TODO: defrag cluster map
		throw new NotImplementedException();
	}

	public override string ToString() {
		CheckInitialized();
		return Header.ToString();
	}

	private ClusteredStream OpenStreamInternal(long streamIndex, bool readOnly, bool acquireThreadLock) {
		Guard.Ensure(!_openStreams.ContainsKey(streamIndex), $"Stream {streamIndex} is already open");

		var accessScope = acquireThreadLock ? EnterAccessScope() : new NoOpScope();
		try {
			var stream = new ClusteredStream(this, streamIndex, readOnly, EndScopeCleanup);
			if (!readOnly) {
				stream.StreamLengthChanged += size => {
					CheckLocked();
					FastWriteStreamDescriptorSize(streamIndex, size);
					NotifyStreamLengthChanged(streamIndex, size);
				};
			}
			_openStreams.Add(streamIndex, stream);
			return stream;

			void EndScopeCleanup() {
				_openStreams.Remove(streamIndex);
				accessScope.Dispose();
			}
		} catch {
			// this is needed in case exception occurs before return
			accessScope.Dispose();
			throw;
		}
	}

	#endregion

	#region Stream Descriptors
	
	public ClusteredStreamDescriptor GetStreamDescriptor(long index) {
		CheckInitialized();
		using var accessScope = EnterAccessScope();
		return _streamDescriptorCache is not null ? _streamDescriptorCache[index] : FetchStreamDescriptor(index);
	}

	internal void UpdateStreamDescriptor(long index, ClusteredStreamDescriptor descriptor) {
		using (EnterAccessScope()) {
			if (_integrityChecks)
				CheckStreamDescriptorIntegrity(index, descriptor);
			_streamDescriptors.Update(index, descriptor);
			_streamDescriptorCache?.Set(index, descriptor);
			NotifyStreamUpdated(index, descriptor);
		}
	}

	private ClusteredStreamDescriptor NewStreamDescriptor() {
		CheckLocked();
		var record = new ClusteredStreamDescriptor();
		record.Size = 0;
		record.StartCluster = Cluster.Null;
		record.EndCluster = Cluster.Null;
		NotifyStreamCreated(record);
		return record;
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ClusteredStreamTraits FastReadStreamDescriptorTraits(long index) {
		CheckLocked();
		_streamDescriptors.InternalCollection.ReadItemBytes(index, ClusteredStreamDescriptorSerializer.TraitsOffset, ClusteredStreamDescriptorSerializer.TraitsLength, out var traitsBytes);
		return (ClusteredStreamTraits)traitsBytes[0];		
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ClusteredStreamDescriptor FetchStreamDescriptor(long index) {
		CheckLocked();
		var record = _streamDescriptors.Read(index);
		if (_integrityChecks)
			CheckStreamDescriptorIntegrity(index, record);
		return record;

	}

	private ClusteredStreamDescriptor AddStreamDescriptor(out long index, ClusteredStreamDescriptor descriptor) {
		CheckLocked();
		_streamDescriptors.Add(descriptor);
		index = Header.StreamCount - 1;
		_streamDescriptorCache?.Set(index, descriptor);
		NotifyStreamAdded(index, descriptor);
		return descriptor;

	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FastWriteStreamDescriptorStartCluster(long record, long startCluster) {
		var bytes = _streamDescriptors.InternalCollection.Writer.BitConverter.GetBytes(startCluster);
		_streamDescriptors.InternalCollection.WriteItemBytes(record, ClusteredStreamDescriptorSerializer.StartClusterOffset, bytes);
		_streamDescriptorCache?.Invalidate(record);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FastWriteStreamDescriptorEndCluster(long record, long endCluster) {
		var bytes = _streamDescriptors.InternalCollection.Writer.BitConverter.GetBytes(endCluster);
		_streamDescriptors.InternalCollection.WriteItemBytes(record, ClusteredStreamDescriptorSerializer.EndClusterOffset, bytes);
		_streamDescriptorCache?.Invalidate(record);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FastWriteStreamDescriptorSize(long record, long size) {
		var bytes = _streamDescriptors.InternalCollection.Writer.BitConverter.GetBytes(size);
		_streamDescriptors.InternalCollection.WriteItemBytes(record, ClusteredStreamDescriptorSerializer.SizeOffset, bytes);
		_streamDescriptorCache?.Invalidate(record);
	}

	/// <remarks>
	/// This has O(N) complexity in worst case (inserting at 0), use with care
	/// </remarks>
	private void InsertStreamDescriptor(long index, ClusteredStreamDescriptor descriptor) {
		CheckLocked();
		// Update genesis clusters 
		for (var i = _streamDescriptors.Count - 1; i >= index; i--) {
			var shiftedRecord = GetStreamDescriptor(i);
			if (shiftedRecord.StartCluster != Cluster.Null)
				_clusters.WriteClusterPrev(shiftedRecord.StartCluster, i + 1);
			if (shiftedRecord.EndCluster != Cluster.Null)
				_clusters.WriteClusterNext(shiftedRecord.EndCluster, i + 1);
			_streamDescriptorCache?.Invalidate(i);
			_streamDescriptorCache?.Set(i + 1, shiftedRecord);
		}
		_streamDescriptors.Insert(index, descriptor);
		_streamDescriptorCache?.Set(index, descriptor);
		//Header.StreamCount++;
		// TODO: restore this by removing RecordCount setting from handler (after bug fixes)
		NotifyStreamInserted(index, descriptor);
	}

	private void RemoveStreamDescriptor(long index) {
		CheckLocked();
		// Need to update terminals of adjacent records (O(N/2) complexity here)
		for (var i = index + 1; i < _streamDescriptors.Count; i++) {
			var higherRecord = GetStreamDescriptor(i);
			if (higherRecord.StartCluster != Cluster.Null)
				_clusters.WriteClusterPrev(higherRecord.StartCluster, i - 1);
			if (higherRecord.EndCluster != Cluster.Null)
				_clusters.WriteClusterNext(higherRecord.EndCluster, i - 1);
			_streamDescriptorCache?.Invalidate(i);
			_streamDescriptorCache?.Set(i - 1, higherRecord);
		}

		_streamDescriptors.RemoveAt(index);
		_streamDescriptorCache?.Invalidate(index);
		// Note: Header.StreamCount is adjusted automatically when removing from the collection
		NotifyStreamRemoved(index);

	}

	#endregion

	#region Lifecycle Management

	public void RegisterInitAction(Action action) {
		if (Initialized)
			action();
		else
			_initActions.Add(action);
	}

	protected override void LoadInternal() {
		if (_rootStream is ILoadable { RequiresLoad: true } loadableStream)
			loadableStream.Load();

		if (!Initialized)
			Initialize();

		if (_clusters.RequiresLoad)
			_clusters.Load();

		LoadAttachments();
	}

	internal void Initialize() {
		var recordSerializer = new ClusteredStreamDescriptorSerializer();
		var clusterSerializer = new ClusterSerializer(_clusterSize);

		// acquire lock on root stream for entire initialize
		using var _ = _rootStream.EnterAccessScope();

		if (_rootStream.InnerStream is ILoadable { RequiresLoad: true } loadableStream)
			loadableStream.Load();

		// Header
		_header = new ClusteredStreamsHeader(_rootStream, Endianness);
		var wasEmptyStream = _rootStream.Length == 0;
		if (!wasEmptyStream) {
			_header.Load();
			_header.CheckHeaderIntegrity();
			CheckHeaderDataIntegrity(_rootStream.Length, _header, clusterSerializer, recordSerializer);
		} else {
			_header.Create(1, _clusterSize, _reservedStreams);
		}
		Guard.Ensure(_header.ClusterSize == _clusterSize, $"Inconsistent cluster size {_clusterSize} (header had '{_header.ClusterSize}')");

		// ClusterMap
		// - stored in a StreamPagedList (single page, statically sized items)
		// - when a start/end cluster is moved, we must update the descriptor that points to it (the descriptor is stored as the terminal values of a cluster chain)
		if (_clusters is not null)
			_clusters.Changed -= ClusterMapChangedHandler;
		_clusters = new StreamMappedClusterMap(_rootStream, ClusteredStreamsHeader.ByteLength, clusterSerializer, Policy.HasFlag(ClusteredStreamsPolicy.CacheClusterHeaders), Endianness, autoLoad: true);
		_clusters.Changed += ClusterMapChangedHandler;

		// Records
		//  - are stored StreamPagedList or ClusteredStreamRecords (single page, statically sized items) which mapped over the cluster chain starting from 0
		//  - the end cluster of the cluster chain is tracked in the header
		//  - the descriptor count is also tracked in the header
		//  - this list of records maintains all the other lists stored in the cluster objectStream
		_streamDescriptorsFragmentProvider?.ClearEventHandlers();
		_streamDescriptorsFragmentProvider = new ClusteredStreamFragmentProvider(
			_clusters,
			-1,
			_header.StreamCount * recordSerializer.ConstantSize,
			_header.StreamCount > 0 ? 0 : Cluster.Null,
			_header.StreamCount > 0 ? _header.StreamDescriptorsEndCluster : Cluster.Null,
			_header.StreamCount > 0 ? _clusters.CalculateClusterChainLength(_header.StreamCount * recordSerializer.ConstantSize) : 0,
			_integrityChecks
		);

		// track descriptor stream length in header
		_streamDescriptorsFragmentProvider.StreamLengthChanged += (_, newLength) => {
			if (_suppressEvents) // event generated from fragment provider
				return;

			_header.StreamCount = newLength / recordSerializer.ConstantSize;
		};

		// The actual records collection is stored is update optimized
		_streamDescriptors?.InternalCollection.Stream.Dispose();
		_streamDescriptors =
			new StreamPagedList<ClusteredStreamDescriptor>(
				recordSerializer,
				new FragmentedStream(_streamDescriptorsFragmentProvider, _header.StreamCount * recordSerializer.ConstantSize),
				Endianness,
				includeListHeader: false,
				autoLoad: true
			)
			.AsUpdateOnly(
				_header.StreamCount,
				PreAllocationPolicy.MinimumRequired,
				0,
				NewStreamDescriptor
			);

		if (Policy.HasFlag(ClusteredStreamsPolicy.CacheDescriptors)) {
			_streamDescriptorCache?.Purge();
			_streamDescriptorCache = new ActionCache<long, ClusteredStreamDescriptor>(
				FetchStreamDescriptor,
				sizeEstimator: _ => recordSerializer.ConstantSize,
				reapStrategy: CacheReapPolicy.LeastUsed,
				ExpirationPolicy.SinceLastAccessedTime,
				maxCapacity: HydrogenDefaults.RecordCacheSize
			);
		} else {
			_streamDescriptorCache = null;
		}

		Initialized = true;

		if (wasEmptyStream)
			CreateReservedStreams();
		
		// Run sub-class initialization actions
		_initActions.ForEach(x => x());

		void ClusterMapChangedHandler(object source, ClusterMapChangedEventArgs changedEvent) {
			CheckLocked();
			SuppressEvents = true;
			try {
				var movedChainTerminals = changedEvent.MovedTerminals.OrderBy(x => x.Key).ToArray();

				// 1. Update header (no clusters affected)
				Header.TotalClusters += changedEvent.ClusterCountDelta;

				// 2. Track descriptor's end cluster 

				// Was it moved?
				if (movedChainTerminals.Length > 0 && movedChainTerminals[0].Key == Cluster.Null) {
					var recordTerminalChanges = movedChainTerminals[0];
					Guard.Against(recordTerminalChanges.Value.NewStart.HasValue, "Descriptor cluster chain start cannot be changed.");
					Guard.Ensure(recordTerminalChanges.Value.NewEnd.HasValue, "Descriptor cluster chain new end was moved but not defined.");
					Header.StreamDescriptorsEndCluster = recordTerminalChanges.Value.NewEnd.Value;
				}

				// Was it created/removed?
				if (changedEvent.ChainTerminal == Cluster.Null) {
					if (changedEvent.RemovedChain) {
						Header.StreamDescriptorsEndCluster = Cluster.Null;
					} else {
						if (changedEvent.AddedChain)
							Guard.Ensure(changedEvent.ChainNewStartCluster == 0, $"Descriptor chain created with invalid start cluster {changedEvent.ChainNewStartCluster.Value}.");
						Guard.Ensure(changedEvent.ChainNewEndCluster.HasValue, "Descriptor cluster chain new end was moved but not defined.");
						Header.StreamDescriptorsEndCluster = changedEvent.ChainNewEndCluster.Value;
					}
				}

				// 3. Inform descriptor's fragment provider and seeker of this event 
				_streamDescriptorsFragmentProvider.ProcessClusterMapChanged(changedEvent);

				// At this point the records collection should be usable, process all other streams

				// 4. If descriptor terminal for this stream moved, update descriptor's cluster pointers
				foreach (var movedTerminal in movedChainTerminals) {
					if (movedTerminal.Key == Cluster.Null)
						continue; // already processed special descriptor terminal above

					if (movedTerminal.Value.NewStart.HasValue)
						FastWriteStreamDescriptorStartCluster(movedTerminal.Key, movedTerminal.Value.NewStart.Value);

					if (movedTerminal.Value.NewEnd.HasValue)
						FastWriteStreamDescriptorEndCluster(movedTerminal.Key, movedTerminal.Value.NewEnd.Value);

				}

				// 5. For a changed stream, update cluster pointers in relevant descriptor 
				if (changedEvent.ChainTerminal.HasValue && changedEvent.ChainTerminal.Value != Cluster.Null) {
					if (changedEvent.AddedChain) {
						FastWriteStreamDescriptorStartCluster(changedEvent.ChainTerminal.Value, changedEvent.ChainNewStartCluster.Value);
						FastWriteStreamDescriptorEndCluster(changedEvent.ChainTerminal.Value, changedEvent.ChainNewEndCluster.Value);
					} else if (changedEvent.RemovedChain) {
						FastWriteStreamDescriptorStartCluster(changedEvent.ChainTerminal.Value, Cluster.Null);
						FastWriteStreamDescriptorEndCluster(changedEvent.ChainTerminal.Value, Cluster.Null);
						FastWriteStreamDescriptorSize(changedEvent.ChainTerminal.Value, 0);
					} else if (changedEvent.IncreasedChainSize || changedEvent.DecreasedChainSize) {
						FastWriteStreamDescriptorEndCluster(changedEvent.ChainTerminal.Value, changedEvent.ChainNewEndCluster.Value);
					}
				}

				// 6. Notify all open stream about cluster map change
				_openStreams.ForEach(x => x.Value.ProcessClusterMapChanged(changedEvent));

			} finally {
				SuppressEvents = false;
			}
		}
	}



	public void Dispose() {
		UnloadAttachments();
		CheckNoOpenedStreams(false);
		//using (_rootStream.EnterAccessScope())
		//	_rootStream.Flush();
		if (OwnsStream) {
			_rootStream.Dispose();
		}
	}

	#endregion

	#region Attachment Management
	
	public void RegisterAttachment(IClusteredStreamsAttachment attachment) {
		// NOTE: we cannot lock the streams since it's not initialized yet, nor can we verify 
		// the Header.ReservedStreams for same reason.

		Guard.ArgumentNotNull(attachment, nameof(attachment));
		Guard.Argument(attachment.AttachmentID is not null, nameof(attachment.AttachmentID), "Attachment did not specify an ID");
//		Guard.Against(Initialized, "Cannot register attachments after initialization");

		Guard.Argument(!_attachments.ContainsKey(attachment.AttachmentID), nameof(attachment), $" An attachment with ID '{attachment.AttachmentID}' was already attached to clustered streams instance");
		//Guard.Ensure(Header.ReservedStreams > _attachments.Count, $"Insufficient reserved streams available to register attachment '{attachment.ID}'") ;


		// track attachment
		_attachments.Add(attachment.AttachmentID, attachment);

		// If objectStream is already loaded, then attach now
		if (!RequiresLoad)
			attachment.Attach();
	
	}


	private void LoadAttachments() {
		foreach(var attachment in _attachments.Values.Where(x => !x.IsAttached))
			attachment.Attach();
	}

	internal void UnloadAttachments() {
		foreach(var (attachment, ix) in _attachments.Values.Where(x => x.IsAttached).WithIndex()) {
			attachment.Detach();
			_streamDescriptorCache?.Invalidate(ix);
		}

		_attachments.Clear();
	}

	#endregion

	#region Event methods

	protected virtual void OnStreamCreated(ClusteredStreamDescriptor descriptor) {
	}

	protected virtual void OnStreamAdded(long index, ClusteredStreamDescriptor descriptor) {
	}

	protected virtual void OnStreamInserted(long index, ClusteredStreamDescriptor descriptor) {
	}

	protected virtual void OnStreamUpdated(long index, ClusteredStreamDescriptor descriptor) {
	}

	protected virtual void OnStreamSwapped(long stream1, ClusteredStreamDescriptor stream1Descriptor, long stream2, ClusteredStreamDescriptor stream2Descriptor) {
		CheckLocked();
		_streamDescriptorCache?.Invalidate(stream1);
		_streamDescriptorCache?.Invalidate(stream2);
		_openStreams.ForEach(x => x.Value.ProcessStreamSwapped(stream1, stream1Descriptor, stream2, stream2Descriptor));
	}

	protected virtual void OnStreamSizeChanged(long index, long newSize) {
	}

	protected virtual void OnStreamRemoved(long index) {
	}

	protected virtual void OnClearing() {
		// Flush all the attachments
		foreach(var attachment in _attachments) 
			attachment.Value.Flush();
	}

	protected virtual void OnCleared() {
	}

	protected virtual void OnReservedStreamsCreating() {
	}

	protected virtual void OnReservedStreamsCreated() {
	}

	private void NotifyStreamCreated(ClusteredStreamDescriptor descriptor) {
		if (_suppressEvents)
			return;

		OnStreamCreated(descriptor);
		StreamCreated?.Invoke(descriptor);
	}

	private void NotifyStreamAdded(long index, ClusteredStreamDescriptor descriptor) {
		if (_suppressEvents)
			return;

		OnStreamAdded(index, descriptor);
		StreamAdded?.Invoke(index, descriptor);
	}

	private void NotifyStreamInserted(long index, ClusteredStreamDescriptor descriptor) {
		if (_suppressEvents)
			return;

		OnStreamInserted(index, descriptor);
		StreamInserted?.Invoke(index, descriptor);
	}

	private void NotifyStreamUpdated(long index, ClusteredStreamDescriptor descriptor) {
		if (_suppressEvents)
			return;

		OnStreamUpdated(index, descriptor);
		StreamUpdated?.Invoke(index, descriptor);
	}

	private void NotifyStreamSwapped(long stream1, ClusteredStreamDescriptor stream1Descriptor, long stream2, ClusteredStreamDescriptor stream2Descriptor) {
		if (_suppressEvents)
			return;
		OnStreamSwapped(stream1, stream1Descriptor, stream2, stream2Descriptor);
		StreamSwapped?.Invoke((stream1, stream1Descriptor), (stream2, stream2Descriptor));
	}

	private void NotifyStreamLengthChanged(long index, long newSize) {
		if (_suppressEvents)
			return;
		OnStreamSizeChanged(index, newSize);
		StreamLengthChanged?.Invoke(index, newSize);
	}

	private void NotifyStreamRemoved(long index) {
		if (_suppressEvents)
			return;

		OnStreamRemoved(index);
		StreamRemoved?.Invoke(index);
	}

	private void NotifyClearing() {
		if (_suppressEvents)
			return;

		OnClearing();
		Clearing?.Invoke();
	}


	private void NotifyCleared() {
		if (_suppressEvents)
			return;

		OnCleared();
		Cleared?.Invoke();
	}

	private void NotifyReservedStreamsCreating() {
		if (_suppressEvents)
			return;

		OnReservedStreamsCreating();
		ReservedStreamsCreating?.Invoke();
	}

	private void NotifyReservedStreamsCreated() {
		if (_suppressEvents)
			return;

		OnReservedStreamsCreated();
		ReservedStreamsCreated?.Invoke();
	}

	#endregion

	#region Aux methods

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckLocked() => Guard.Ensure(IsLocked, "An access-scope is required for this operation");

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckNoOpenedStreams(bool allowOpenedReservedStreams, string errorMessage = "This operation cannot be executed whilst there are open scopes") {
		//=> Guard.Ensure(_openStreams.Count == 0 || allowOpenedReservedStreams && _openStreams.All(x => x.Key < Header.ReservedStreams), errorMessage);
		if (_openStreams.Count > 0) {
			if (allowOpenedReservedStreams) 
				using (EnterAccessScope())
					if (_openStreams.All(x => x.Key < Header.ReservedStreams))
						return;
			throw new InvalidOperationException(errorMessage);
		}
	}
		
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckInitialized() {
		if (!Initialized)
			throw new InvalidOperationException("Clustered Streams not initialized");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckNotReserved(long index) {
		if (index < Header.ReservedStreams)
			throw new InvalidOperationException($"This operation cannot be performed on a reserved stream (index: {index}");
	}

	private void CheckHeaderDataIntegrity(long rootStreamLength, ClusteredStreamsHeader header, IItemSerializer<Cluster> clusterSerializer, IItemSerializer<ClusteredStreamDescriptor> recordSerializer) {
		var clusterEnvelopeSize = clusterSerializer.ConstantSize - header.ClusterSize;
		var recordClusters = (long)Math.Ceiling(header.StreamCount * recordSerializer.ConstantSize / (float)header.ClusterSize);
		Guard.Ensure(header.TotalClusters >= recordClusters, $"Inconsistency in {nameof(ClusteredStreamsHeader.TotalClusters)}/{nameof(ClusteredStreamsHeader.StreamCount)}");
		var minStreamSize = header.TotalClusters * (header.ClusterSize + clusterEnvelopeSize) + ClusteredStreamsHeader.ByteLength;
		Guard.Ensure(rootStreamLength >= minStreamSize, $"Stream too small (header gives minimum size {minStreamSize} but was {rootStreamLength})");
	}

	private void CreateReservedStreams() {
		Guard.Ensure(Header.StreamCount == 0, "Records are already existing");
		NotifyReservedStreamsCreating();
		for (var i = 0; i < Header.ReservedStreams; i++) {
			AddStreamDescriptor(out var index, NewStreamDescriptor());
		}
		Header.StreamCount = _streamDescriptors.Count; // this has to be done explicitly here since the handler which sets RecordCount may not be called in certain scenarios
		NotifyReservedStreamsCreated();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckStreamDescriptorIndex(long index, string msg = null, bool allowEnd = false)
		=> Guard.CheckIndex(index, 0, _streamDescriptors.Count, allowEnd);

	private void CheckStreamDescriptorIntegrity(long index, ClusteredStreamDescriptor descriptor) {
		if (descriptor.Size == 0) {
			Guard.Ensure(descriptor.StartCluster == Cluster.Null, $"Empty stream descriptor {index} should have start cluster {Cluster.Null} but was {descriptor.StartCluster}");
			Guard.Ensure(descriptor.EndCluster == Cluster.Null, $"Empty stream descriptor {index} should have end cluster {Cluster.Null} but was {descriptor.EndCluster}");
		} else Guard.Ensure(0 <= descriptor.StartCluster && descriptor.StartCluster < Header.TotalClusters, $"Stream descriptor {index} pointed to to non-existent cluster {descriptor.StartCluster}");
	}

	#endregion

}
