// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Hydrogen;

/// <summary>
/// A container of <see cref="Stream"/>'s whose contents are stored in a clustered manner over a root <see cref="Stream"/> (similar in principle to how a file-system works).
/// Fundamentally, this class can function as a "virtual file system" allowing an arbitrary number of <see cref="Stream"/>'s to be stored (and changed). This class
/// also serves as the base container for implementations of <see cref="IStreamMappedList{TItem}"/>'s, <see cref="IStreamMappedDictionary{TKey,TValue}"/>'s and <see cref="IStreamMappedHashSet{TItem}"/>'s.
/// <remarks>
/// The structure of the underlying stream is depicted below:
/// [HEADER] Version: 1, Cluster Size: 32, Total ClusterMap: 10, Records: 5
/// [STREAM DESCRIPTORS]
///   0: [StreamDescriptor] Size: 60, Start Cluster: 3
///   1: [StreamDescriptor] Size: 88, Start Cluster: 7
///   2: [StreamDescriptor] Size: 27, Start Cluster: 2
///   3: [StreamDescriptor] Size: 43, Start Cluster: 1
///   4: [StreamDescriptor] Size: 0, Start Cluster: -1
/// [ClusterMap]
///   0: [Cluster] Traits: First, Prev: -1, Next: 6, Data: 030000003c0000000700000058000000020000001b000000010000002b000000
///   1: [Cluster] Traits: First, Prev: 3, Next: 5, Data: 894538851b6655bb8d8a4b4517eaab2b22ada63e6e0000000000000000000000
///   2: [Cluster] Traits: First, Prev: 2, Next: -1, Data: 1e07b1f66b3a237ed9f438ec26093ca50dd05b798baa7de25f093f0000000000
///   3: [Cluster] Traits: First, Prev: 0, Next: 9, Data: ce178efbff3e3177069101b78453de5ca2d1a7d72c958485306fb400e0efc1f5
///   4: [Cluster] Traits: None, Prev: 8, Next: -1, Data: a3058b9856aaf271ab21153c040a05c15042abbf000000000000000000000000
///   5: [Cluster] Traits: None, Prev: 1, Next: -1, Data: 0000000000000000000000000000000000000000000000000000000000000000
///   6: [Cluster] Traits: Descriptor, Prev: 0, Next: -1, Data: ffffffff00000000000000000000000000000000000000000000000000000000
///   7: [Cluster] Traits: First, Data, Prev: 1, Next: 8, Data: 5aa2c04b9554fbe9425c2d52aa135ed8107bf9edbf44848326eb92cc9434b828
///   8: [Cluster] Traits: Data, Prev: 7, Next: 4, Data: c612bcb3e59fd0d7d88240797e649b5020d5090682c0f3151e3c24a9c12e540d
///   9: [Cluster] Traits: Data, Prev: 3, Next: -1, Data: 594ebf3d9241c837ffa3dea9ab0e550516ad18ed0f7b9c000000000000000000
///
///  Notes:
///  - Header is fixed 256b, and can be expanded to include other data (passwords, merkle roots, etc).
///  - ClusterMap are bi-directionally linked, to allow dynamic re-sizing on the fly. 
///  - Records contain the meta-data of all the streams and the entire records stream is also serialized over clusters.
///  - Cluster traits distinguish descriptor clusters from stream clusters. 
///  - Cluster 0, when allocated, is always the first descriptor cluster.
///  - Records always link to the (First | Data) cluster of their stream.
///  - ClusterMap with traits (First | Data) re-purpose the Prev field to denote the descriptor.
/// </remarks>
public class StreamContainer : SyncLoadableBase, ICriticalObject {
	public event EventHandlerEx<ClusteredStreamDescriptor> StreamCreated;
	public event EventHandlerEx<long, ClusteredStreamDescriptor> StreamAdded;
	public event EventHandlerEx<long, ClusteredStreamDescriptor> StreamInserted;
	public event EventHandlerEx<long, ClusteredStreamDescriptor> StreamUpdated;
	public event EventHandlerEx<(long, ClusteredStreamDescriptor), (long, ClusteredStreamDescriptor)> StreamSwapped;
	public event EventHandlerEx<long, long> StreamLengthChanged;
	public event EventHandlerEx<long> StreamRemoved;

	private StreamMappedClusterMap _clusters;
	private ClusteredStreamFragmentProvider _streamDescriptorsFragmentProvider;
	private UpdateOnlyList<ClusteredStreamDescriptor, StreamPagedList<ClusteredStreamDescriptor>> _streamDescriptors;
	private ICache<long, ClusteredStreamDescriptor> _streamDescriptorCache;
	private StreamContainerHeader _header;
	private readonly IDictionary<long, ClusteredStream> _openStreams;

	private bool _initialized;
	private readonly ConcurrentStream _rootStream;
	private readonly int _clusterSize;
	private readonly long _streamDescriptorKeySize;
	private readonly long _reservedStreams;
	private readonly bool _integrityChecks;
	private readonly bool _preAllocateOptimization;
	private int _clusterEnvelopeSize;
	private bool _suppressEvents;

	public StreamContainer(Stream rootStream, int clusterSize = HydrogenDefaults.ClusterSize, StreamContainerPolicy policy = StreamContainerPolicy.Default, long streamDescriptorKeySize = 0, long reservedStreams = 0, Endianness endianness = Endianness.LittleEndian, bool autoLoad = false) {
		Guard.ArgumentNotNull(rootStream, nameof(rootStream));
		Guard.ArgumentGTE(clusterSize, 1, nameof(clusterSize));
		Guard.ArgumentInRange(streamDescriptorKeySize, 0, ushort.MaxValue, nameof(streamDescriptorKeySize));
		Guard.ArgumentGTE(reservedStreams, 0, nameof(reservedStreams));
		if (policy.HasFlag(StreamContainerPolicy.TrackKey))
			Guard.Argument(streamDescriptorKeySize > 0, nameof(streamDescriptorKeySize), $"Must be greater than 0 when {nameof(StreamContainerPolicy.TrackKey)}");
		Policy = policy;
		Endianness = endianness;
		_clusters = null;
		_streamDescriptorsFragmentProvider = null;
		_streamDescriptors = null;
		_streamDescriptorCache = null;
		_header = null;
		_openStreams = new Dictionary<long, ClusteredStream>();
		_initialized = false;
		_rootStream = rootStream.AsConcurrent();
		_clusterSize = clusterSize;
		_streamDescriptorKeySize = streamDescriptorKeySize;
		_reservedStreams = reservedStreams;
		_integrityChecks = Policy.HasFlag(StreamContainerPolicy.IntegrityChecks);
		_preAllocateOptimization = Policy.HasFlag(StreamContainerPolicy.FastAllocate);
		_clusterEnvelopeSize = 0;
		_suppressEvents = false;

		if (autoLoad)
			Load();
	}

	public static StreamContainer FromStream(Stream rootStream, Endianness endianness = Endianness.LittleEndian, bool autoLoad = false) {
		Guard.Ensure(rootStream.Length >= StreamContainerHeader.ByteLength, $"Corrupt header (stream was too small {rootStream.Length} bytes)");
		var reader = new EndianBinaryReader(EndianBitConverter.For(endianness), rootStream);

		// read cluster size
		rootStream.Seek(StreamContainerHeader.ClusterSizeOffset, SeekOrigin.Begin);
		var clusterSize = reader.ReadInt32();
		Guard.Ensure(clusterSize > 0, $"Corrupt header (ClusterSize field was {clusterSize} bytes)");

		// read policy
		rootStream.Seek(StreamContainerHeader.PolicyOffset, SeekOrigin.Begin);
		var policy = (StreamContainerPolicy)reader.ReadUInt32();

		// read descriptor key size
		rootStream.Seek(StreamContainerHeader.StreamDescriptorKeySizeOffset, SeekOrigin.Begin);
		var recordKeySize = reader.ReadUInt16();

		// read records offset
		rootStream.Seek(StreamContainerHeader.StreamDescriptorRecordsOffset, SeekOrigin.Begin);
		var reservedStreams = reader.ReadInt64();

		rootStream.Position = 0;
		var storage = new StreamContainer(rootStream, clusterSize, policy, recordKeySize, reservedStreams, endianness, autoLoad: autoLoad);

		return storage;
	}

	public ICriticalObject ParentCriticalObject { get => _rootStream.ParentCriticalObject; set => _rootStream.ParentCriticalObject = value; }

	public object Lock => _rootStream.Lock;

	public bool IsLocked => _rootStream.IsLocked;

	public override bool RequiresLoad {
		get => !_initialized || base.RequiresLoad || RootStream is ILoadable { RequiresLoad: true };
		set => base.RequiresLoad = value;
	}

	public Stream RootStream => _rootStream;

	public StreamContainerHeader Header {
		get {
			CheckInitialized();
			return _header;
		}
		private set => _header = value;
	}

	public StreamContainerPolicy Policy { get; }

	public Endianness Endianness { get; }

	public long Count {
		get {
			CheckInitialized();
			return Header.StreamCount;
		}
	}

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

	internal int ClusterEnvelopeSize {
		get {
			CheckInitialized();
			return _clusterEnvelopeSize;
		}
		private set => _clusterEnvelopeSize = value;
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

	public ClusteredStream EnterSaveItemScope<TItem>(long index, TItem item, IItemSerializer<TItem> serializer, ListOperationType operationType) {
		// initialized and reentrancy checks done by one of below called methods
		var stream = operationType switch {
			ListOperationType.Add => Add(),
			ListOperationType.Update => OpenWrite(index),
			ListOperationType.Insert => Insert(index),
			_ => throw new ArgumentException($@"List operation type '{operationType}' not supported", nameof(operationType)),
		};
		try {
			using var writer = new EndianBinaryWriter(EndianBitConverter.For(Endianness), stream);
			if (item != null) {
				stream.Descriptor.Traits = stream.Descriptor.Traits.CopyAndSetFlags(ClusteredStreamTraits.IsNull, false);
				if (_preAllocateOptimization) {
					// pre-setting the stream length before serialization improves performance since it avoids
					// re-allocating fragmented stream on individual properties of the serialized item
					var expectedSize = serializer.CalculateSize(item);
					stream.SetLength(expectedSize);
					serializer.Serialize(item, writer);
				} else {
					var byteLength = serializer.Serialize(item, writer);
					stream.SetLength(byteLength);
				}

			} else {
				stream.Descriptor.Traits = stream.Descriptor.Traits.CopyAndSetFlags(ClusteredStreamTraits.IsNull, true);
				stream.SetLength(0); // open descriptor will save when closed
			}
			return stream;
		} catch {
			// need to dispose explicitly if not returned
			stream.Dispose();
			throw;
		}
	}

	public ClusteredStream EnterLoadItemScope<TItem>(long index, IItemSerializer<TItem> serializer, out TItem item) {
		// initialized and reentrancy checks done by Open
		var stream = OpenWrite(index);
		try {
			if (!stream.Descriptor.Traits.HasFlag(ClusteredStreamTraits.IsNull)) {
				using var reader = new EndianBinaryReader(EndianBitConverter.For(Endianness), stream);
				item = serializer.Deserialize(stream.Descriptor.Size, reader);
			} else item = default;
			return stream;
		} catch {
			// need to dispose explicitly if not returned
			stream.Dispose();
			throw;
		}
	}

	public ClusteredStream Add() {
		CheckInitialized();
		using var accessScope = EnterAccessScope();
		AddStreamDescriptor(out var index, NewStreamDescriptor()); // the first descriptor add will allocate cluster 0 for the records stream
		return OpenStreamInternal(index, false);
	}

	public ClusteredStream OpenRead(long index) => Open(index, true);

	public ClusteredStream OpenWrite(long index) => Open(index, false);

	public ClusteredStream Open(long index, bool readOnly) {
		CheckInitialized();
		using var accessScope = EnterAccessScope();
		CheckStreamDescriptorIndex(index);
		return OpenStreamInternal(index, readOnly);
	}

	public void Remove(long index) {
		CheckInitialized();
		using var accessScope = EnterAccessScope();
		CheckNoOpenedStreams();
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
		using var accessScope = EnterAccessScope();
		CheckNoOpenedStreams();
		CheckStreamDescriptorIndex(index, allowEnd: true);
		InsertStreamDescriptor(index, NewStreamDescriptor());
		return OpenStreamInternal(index, false);
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
		using var accessScope = EnterAccessScope();
		CheckNoOpenedStreams();
		SuppressEvents = true;
		try {
			_streamDescriptors.Clear();
			_streamDescriptorCache?.Flush();
			_clusters.Clear();
			Header.StreamCount = 0;
			Header.TotalClusters = 0;
			Header.StreamDescriptorsEndCluster = Cluster.Null;
			Header.ResetMerkleRoot();
		} finally {
			SuppressEvents = false;
		}
		CreateReservedStreams();
#if ENABLE_CLUSTER_DIAGNOSTICS
		ClusterDiagnostics.VerifyClusters(this);
#endif
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

	private ClusteredStream OpenStreamInternal(long streamIndex, bool readOnly) {
		Guard.Ensure(!_openStreams.ContainsKey(streamIndex), $"Stream {streamIndex} is already open");

		var accessScope = EnterAccessScope();
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
		record.Key = new byte[_streamDescriptorKeySize];
		NotifyStreamCreated(record);
		return record;
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

	#region Initialization & Loading

	protected override void LoadInternal() {
		if (_rootStream is ILoadable loadableStream)
			loadableStream.Load();

		if (!_initialized)
			Initialize();

		if (_clusters.RequiresLoad)
			_clusters.Load();
	}

	private void Initialize() {
		var recordSerializer = new ClusteredStreamDescriptorSerializer(Policy, _streamDescriptorKeySize);
		var clusterSerializer = new ClusterSerializer(_clusterSize);

		// acquire lock on root stream for entire initialize
		using var _ = _rootStream.EnterAccessScope();

		if (_rootStream.InnerStream is ILoadable loadableStream)
			loadableStream.Load();

		// Header
		_header = new StreamContainerHeader(_rootStream, Endianness);
		var wasEmptyStream = _rootStream.Length == 0;
		if (!wasEmptyStream) {
			_header.Load();
			_header.CheckHeaderIntegrity();
			CheckHeaderDataIntegrity(_rootStream.Length, _header, clusterSerializer, recordSerializer);
		} else {
			_header.Create(1, _clusterSize, _streamDescriptorKeySize, _reservedStreams);
		}
		Guard.Ensure(_header.ClusterSize == _clusterSize, $"Inconsistent cluster size {_clusterSize} (header had '{_header.ClusterSize}')");

		// ClusterMap
		// - stored in a StreamPagedList (single page, statically sized items)
		// - when a start/end cluster is moved, we must update the descriptor that points to it (the descriptor is stored as the terminal values of a cluster chain)
		ClusterEnvelopeSize = checked((int)clusterSerializer.StaticSize) - _header.ClusterSize; // the serializer includes the envelope, the header is the data size
		_clusters = new StreamMappedClusterMap(_rootStream, StreamContainerHeader.ByteLength, clusterSerializer, Endianness, autoLoad: true);
		_clusters.Changed += ClusterMapChangedHandler;

		// Records
		//  - are stored StreamPagedList or ClusteredStreamRecords (single page, statically sized items) which mapped over the cluster chain starting from 0
		//  - the end cluster of the cluster chain is tracked in the header
		//  - the descriptor count is also tracked in the header
		//  - this list of records maintains all the other lists stored in the cluster container
		//var recordsCount = _header.StreamCount;
		_streamDescriptorsFragmentProvider = new ClusteredStreamFragmentProvider(
			_clusters,
			-1,
			_header.StreamCount * recordSerializer.StaticSize,
			_header.StreamCount > 0 ? 0 : Cluster.Null,
			_header.StreamCount > 0 ? _header.StreamDescriptorsEndCluster : Cluster.Null,
			_header.StreamCount > 0 ? _clusters.CalculateClusterChainLength(_header.StreamCount * recordSerializer.StaticSize) : 0,
			_integrityChecks
		);

		// track descriptor stream length in header
		_streamDescriptorsFragmentProvider.StreamLengthChanged += (_, newLength) => {
			if (_suppressEvents) // event generated from fragment provider
				return;

			_header.StreamCount = newLength / recordSerializer.StaticSize;
		};

		// The actual records collection is stored is update optimized
		_streamDescriptors =
			new StreamPagedList<ClusteredStreamDescriptor>(
				recordSerializer,
				new FragmentedStream(_streamDescriptorsFragmentProvider, _header.StreamCount * recordSerializer.StaticSize),
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

		if (Policy.HasFlag(StreamContainerPolicy.CacheRecords)) {
			_streamDescriptorCache = new ActionCache<long, ClusteredStreamDescriptor>(
				FetchStreamDescriptor,
				sizeEstimator: _ => recordSerializer.StaticSize,
				reapStrategy: CacheReapPolicy.LeastUsed,
				ExpirationPolicy.SinceLastAccessedTime,
				maxCapacity: HydrogenDefaults.RecordCacheSize
			);
		} else {
			_streamDescriptorCache = null;
		}

		_initialized = true;

		if (wasEmptyStream)
			CreateReservedStreams();
	}

	private void ClusterMapChangedHandler(object source, ClusterMapChangedEventArgs changedEvent) {
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

	#endregion

	#region Aux methods

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckLocked() => Guard.Ensure(IsLocked, "An access-scope is required for this operation");

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckNoOpenedStreams(string errorMessage = "This operation cannot be executed whilst there are open scopes")
		=> Guard.Ensure(_openStreams.Count == 0, errorMessage);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckInitialized() {
		if (!_initialized)
			throw new InvalidOperationException("Clustered Streams not initialized");
	}

	private void CheckHeaderDataIntegrity(long rootStreamLength, StreamContainerHeader header, IItemSerializer<Cluster> clusterSerializer, IItemSerializer<ClusteredStreamDescriptor> recordSerializer) {
		var clusterEnvelopeSize = clusterSerializer.StaticSize - header.ClusterSize;
		var recordClusters = (long)Math.Ceiling(header.StreamCount * recordSerializer.StaticSize / (float)header.ClusterSize);
		Guard.Ensure(header.TotalClusters >= recordClusters, $"Inconsistency in {nameof(StreamContainerHeader.TotalClusters)}/{nameof(StreamContainerHeader.StreamCount)}");
		var minStreamSize = header.TotalClusters * (header.ClusterSize + clusterEnvelopeSize) + StreamContainerHeader.ByteLength;
		Guard.Ensure(rootStreamLength >= minStreamSize, $"Stream too small (header gives minimum size {minStreamSize} but was {rootStreamLength})");
	}

	private void CreateReservedStreams() {
		Guard.Ensure(Header.StreamCount == 0, "Records are already existing");
		for (var i = 0; i < Header.ReservedStreams; i++) {
			AddStreamDescriptor(out var index, NewStreamDescriptor());
		}
		Header.StreamCount = _streamDescriptors.Count; // this has to be done explicitly here since the handler which sets RecordCount may not be called in certain scenarios
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