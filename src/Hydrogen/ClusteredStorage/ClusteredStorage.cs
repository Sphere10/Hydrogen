// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

namespace Hydrogen;

/// <summary>
/// A container of <see cref="Stream"/>'s whose contents are stored across clusters of data over a root <see cref="Stream"/> (similar in principle to how a file-system works).
/// Fundamentally, this class can function as a "virtual file system" allowing an arbitrary number of <see cref="Stream"/>'s to be stored (and changed). This class
/// also serves as the base container for implementations of <see cref="IStreamMappedList{TItem}"/>'s, <see cref="IStreamMappedDictionary{TKey,TValue}"/>'s and <see cref="IStreamMappedHashSet{TItem}"/>'s.
/// <remarks>
/// The structure of the underlying stream is depicted below:
/// [HEADER] Version: 1, Cluster Size: 32, Total Clusters: 10, Records: 5
/// [Records]
///   0: [StreamRecord] Size: 60, Start Cluster: 3
///   1: [StreamRecord] Size: 88, Start Cluster: 7
///   2: [StreamRecord] Size: 27, Start Cluster: 2
///   3: [StreamRecord] Size: 43, Start Cluster: 1
///   4: [StreamRecord] Size: 0, Start Cluster: -1
/// [Clusters]
///   0: [Cluster] Traits: First, Record, Prev: -1, Next: 6, Data: 030000003c0000000700000058000000020000001b000000010000002b000000
///   1: [Cluster] Traits: First, Data, Prev: 3, Next: 5, Data: 894538851b6655bb8d8a4b4517eaab2b22ada63e6e0000000000000000000000
///   2: [Cluster] Traits: First, Data, Prev: 2, Next: -1, Data: 1e07b1f66b3a237ed9f438ec26093ca50dd05b798baa7de25f093f0000000000
///   3: [Cluster] Traits: First, Data, Prev: 0, Next: 9, Data: ce178efbff3e3177069101b78453de5ca2d1a7d72c958485306fb400e0efc1f5
///   4: [Cluster] Traits: Data, Prev: 8, Next: -1, Data: a3058b9856aaf271ab21153c040a05c15042abbf000000000000000000000000
///   5: [Cluster] Traits: Data, Prev: 1, Next: -1, Data: 0000000000000000000000000000000000000000000000000000000000000000
///   6: [Cluster] Traits: Record, Prev: 0, Next: -1, Data: ffffffff00000000000000000000000000000000000000000000000000000000
///   7: [Cluster] Traits: First, Data, Prev: 1, Next: 8, Data: 5aa2c04b9554fbe9425c2d52aa135ed8107bf9edbf44848326eb92cc9434b828
///   8: [Cluster] Traits: Data, Prev: 7, Next: 4, Data: c612bcb3e59fd0d7d88240797e649b5020d5090682c0f3151e3c24a9c12e540d
///   9: [Cluster] Traits: Data, Prev: 3, Next: -1, Data: 594ebf3d9241c837ffa3dea9ab0e550516ad18ed0f7b9c000000000000000000
///
///  Notes:
///  - Header is fixed 256b, and can be expanded to include other data (passwords, merkle roots, etc).
///  - Clusters are bi-directionally linked, to allow dynamic re-sizing on the fly. 
///  - Records contain the meta-data of all the streams and the entire records stream is also serialized over clusters.
///  - Cluster traits distinguish record clusters from stream clusters. 
///  - Cluster 0, when allocated, is always the first record cluster.
///  - Records always link to the (First | Data) cluster of their stream.
///  - Clusters with traits (First | Data) re-purpose the Prev field to denote the record.
/// </remarks>
public class ClusteredStorage : SyncLoadableBase, IClusteredStorage {

	public event EventHandlerEx<ClusteredStreamRecord> RecordCreated;
	public event EventHandlerEx<long, ClusteredStreamRecord> RecordAdded;
	public event EventHandlerEx<long, ClusteredStreamRecord> RecordInserted;
	public event EventHandlerEx<long, ClusteredStreamRecord> RecordUpdated;
	public event EventHandlerEx<long> RecordRemoved;

	private const int DefaultClusterSize = 256; // 256b
	private const long DefaultRecordCacheSize = 1 << 20; // 1mb

	private StreamPagedList<Cluster> _clusters;
	private FragmentProvider _recordsFragmentProvider;
	private PreAllocatedList<ClusteredStreamRecord> _records;
	private ICache<long, ClusteredStreamRecord> _recordCache;
	private ClusteredStorageHeader _header;
	private ClusteredStreamScope _openScope;

	private bool _initialized;
	private readonly Stream _rootStream;
	private readonly int _clusterSize;
	private readonly long _recordKeySize;
	private readonly long _reservedRecords;
	private readonly object _lock;
	private readonly bool _integrityChecks;
	private readonly bool _preAllocateOptimization;
	private int _clusterEnvelopeSize;
	private long _allRecordsSize;

	public ClusteredStorage(Stream rootStream, int clusterSize = DefaultClusterSize, ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, long recordKeySize = 0, long reservedRecords = 0, Endianness endianness = Endianness.LittleEndian) {
		Guard.ArgumentNotNull(rootStream, nameof(rootStream));
		Guard.ArgumentGTE(clusterSize, 1, nameof(clusterSize));
		Guard.ArgumentInRange(recordKeySize, 0, ushort.MaxValue, nameof(recordKeySize));
		Guard.ArgumentGTE(reservedRecords, 0, nameof(reservedRecords));
		if (Policy.HasFlag(ClusteredStoragePolicy.TrackKey))
			Guard.Argument(recordKeySize > 0, nameof(recordKeySize), $"Must be greater than 0 when {nameof(ClusteredStoragePolicy.TrackKey)}");
		_rootStream = rootStream;
		_clusterSize = clusterSize;
		Policy = policy;
		_recordKeySize = recordKeySize;
		_reservedRecords = reservedRecords;
		Endianness = endianness;
		_lock = new object(); // new NonReentrantLock();
		_openScope = null;
		_preAllocateOptimization = Policy.HasFlag(ClusteredStoragePolicy.FastAllocate);
		_integrityChecks = Policy.HasFlag(ClusteredStoragePolicy.IntegrityChecks);
		ZeroClusterBytes = Tools.Array.Gen<byte>(clusterSize, 0);
		_header = null;
		_initialized = false;
	}

	public static ClusteredStorage FromStream(Stream rootStream, Endianness endianness = Endianness.LittleEndian) {
		if (rootStream.Length < ClusteredStorageHeader.ByteLength)
			throw new CorruptDataException($"Corrupt header (stream was too small {rootStream.Length} bytes)");
		var reader = new EndianBinaryReader(EndianBitConverter.For(endianness), rootStream);

		// read cluster size
		rootStream.Seek(ClusteredStorageHeader.ClusterSizeOffset, SeekOrigin.Begin);
		var clusterSize = reader.ReadInt32();
		if (clusterSize <= 0)
			throw new CorruptDataException($"Corrupt header (ClusterSize field was {clusterSize} bytes)");

		// read policy
		rootStream.Seek(ClusteredStorageHeader.PolicyOffset, SeekOrigin.Begin);
		var policy = (ClusteredStoragePolicy)reader.ReadUInt32();

		// read record key size
		rootStream.Seek(ClusteredStorageHeader.RecordKeySizeOffset, SeekOrigin.Begin);
		var recordKeySize = reader.ReadUInt16();

		// read records offset
		rootStream.Seek(ClusteredStorageHeader.ReservedRecordsOffset, SeekOrigin.Begin);
		var reservedRecords = reader.ReadInt64();

		rootStream.Position = 0;
		var storage = new ClusteredStorage(rootStream, clusterSize, policy, recordKeySize, reservedRecords, endianness);
		storage.Load();
		return storage;
	}

	public override bool RequiresLoad {
		get => !_initialized || base.RequiresLoad;
		set => base.RequiresLoad = value;
	}

	public ClusteredStorageHeader Header {
		get {
			CheckInitialized();
			return _header;
		}
		private set => _header = value;
	}

	public ClusteredStoragePolicy Policy { get; }

	public Endianness Endianness { get; }

	public long RecordCacheSize {
		get {
			CheckInitialized();
			return _recordCache.MaxCapacity;
		}
		set {
			CheckInitialized();
			throw new NotImplementedException();
		}
	}

	public long Count {
		get {
			CheckInitialized();
			return Header.RecordsCount;
		}
	}

	public IReadOnlyList<ClusteredStreamRecord> Records {
		get {
			CheckInitialized();
			return _records;
		}
	}

	internal IReadOnlyList<Cluster> Clusters {
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

	internal long AllRecordsSize {
		get {
			CheckInitialized();
			return _allRecordsSize;
		}
		private set => _allRecordsSize = value;
	}

	internal bool IsLocked => Monitor.IsEntered(_lock); // _lock?.IsLocked ?? false;

	private ReadOnlyMemory<byte> ZeroClusterBytes { get; }

	#region Initialization & Loading

	protected override void LoadInternal() {
		using (EnterLockScope()) {
			if (!_initialized)
				Initialize();

			if (_clusters.RequiresLoad)
				_clusters.Load();
		}
	}

	private void Initialize() {
		var recordSerializer = new ClusteredStreamRecordSerializer(Policy, _recordKeySize);
		var clusterSerializer = new ClusterSerializer(_clusterSize);

		_header = new ClusteredStorageHeader(_lock);
		var wasEmptyStream = _rootStream.Length == 0;
		if (!wasEmptyStream) {
			_header.AttachTo(_rootStream, Endianness);
			_header.CheckHeaderIntegrity();
			CheckHeaderDataIntegrity(_rootStream.Length, _header, clusterSerializer, recordSerializer);
		} else {
			_header.CreateIn(1, _rootStream, _clusterSize, _recordKeySize, _reservedRecords, Endianness);
		}

		Guard.Ensure(_header.ClusterSize == _clusterSize, $"Inconsistent cluster size {_clusterSize} (stream header had '{_header.ClusterSize}')");
		AllRecordsSize = _header.RecordsCount * recordSerializer.StaticSize;
		ClusterEnvelopeSize = checked((int)clusterSerializer.StaticSize) - _header.ClusterSize; // the serializer includes the envelope, the header is the data size

		// Clusters are stored in a StreamPagedList (single page, statically sized items)
		_clusters = new StreamPagedList<Cluster>(
			clusterSerializer,
			new NonClosingStream(new BoundedStream(_rootStream, ClusteredStorageHeader.ByteLength, long.MaxValue) { UseRelativeOffset = true, AllowInnerResize = true }),
			Endianness
		) { IncludeListHeader = false };
		if (_clusters.RequiresLoad)
			_clusters.Load();

		// Records are stored in record 0 as StreamPagedList (single page, statically sized items) which maps over the fragmented stream 
		_recordsFragmentProvider = new FragmentProvider(this, ClusterDataType.Record);
		var recordStorage = new StreamPagedList<ClusteredStreamRecord>(
			recordSerializer,
			new FragmentedStream(_recordsFragmentProvider, _header.RecordsCount * recordSerializer.StaticSize),
			Endianness
		) { IncludeListHeader = false };
		if (recordStorage.RequiresLoad)
			recordStorage.Load();

		// The actual records collection is a PreAllocated list over the StreamPagedList which allows INSERTS in the form of UPDATES.
		_records = new PreAllocatedList<ClusteredStreamRecord>(
			recordStorage,
			_header.RecordsCount,
			PreAllocationPolicy.MinimumRequired,
			0,
			NewRecord
		);
		_recordCache = Policy.HasFlag(ClusteredStoragePolicy.CacheRecords)
			? new ActionCache<long, ClusteredStreamRecord>(
				FetchRecord,
				sizeEstimator: _ => recordSerializer.StaticSize,
				reapStrategy: CacheReapPolicy.LeastUsed,
				ExpirationPolicy.SinceLastAccessedTime,
				maxCapacity: DefaultRecordCacheSize
			)
			: null;

		_initialized = true;

		if (wasEmptyStream)
			CreateReservedRecords();
	}

	#endregion

	#region Streams

	public ClusteredStreamScope EnterSaveItemScope<TItem>(long index, TItem item, IItemSerializer<TItem> serializer, ListOperationType operationType) {
		// initialized and reentrancy checks done by one of below called methods
		var scope = operationType switch {
			ListOperationType.Add => Add(),
			ListOperationType.Update => Open(index),
			ListOperationType.Insert => Insert(index),
			_ => throw new ArgumentException($@"List operation type '{operationType}' not supported", nameof(operationType)),
		};
		var writer = new EndianBinaryWriter(EndianBitConverter.For(Endianness), _openScope.Stream);
		if (item != null) {

			_openScope.Record.Traits = _openScope.Record.Traits.CopyAndSetFlags(ClusteredStreamTraits.IsNull, false);
			if (_preAllocateOptimization) {
				// pre-setting the stream length before serialization improves performance since it avoids
				// re-allocating fragmented stream on individual properties of the serialized item
				var expectedSize = serializer.CalculateSize(item);
				_openScope.Stream.SetLength(expectedSize);
				if (!serializer.TrySerialize(item, writer, out var size))
					throw new SerializationException("Failed to serialize item");
				if (_integrityChecks) {
					Guard.Ensure(expectedSize == size, "Calculated size did not match serialization size");
					Guard.Ensure(_openScope.Stream.Position == expectedSize, "Serialization did not serialize expected length");
				} else {
					Debug.Assert(expectedSize == size, "Calculated size did not match serialization size");
					Debug.Assert(_openScope.Stream.Position == expectedSize, "Serialization did not serialize expected length");
				}
			} else {
				var bytes = serializer.Serialize(item, writer);
				if (_integrityChecks) {
					Guard.Ensure(bytes == _openScope.Stream.Position);
				} else {
					Debug.Assert(bytes == _openScope.Stream.Position);
				}
				_openScope.Stream.SetLength(_openScope.Stream.Position);
			}

		} else {
			_openScope.Record.Traits = _openScope.Record.Traits.CopyAndSetFlags(ClusteredStreamTraits.IsNull, true);
			_openScope.Stream.SetLength(0); // open record will save when closed
		}
		return scope;
	}

	public ClusteredStreamScope EnterLoadItemScope<TItem>(long index, IItemSerializer<TItem> serializer, out TItem item) {
		// initialized and reentrancy checks done by Open
		var scope = Open(index);
		if (!_openScope.Record.Traits.HasFlag(ClusteredStreamTraits.IsNull)) {
			var reader = new EndianBinaryReader(EndianBitConverter.For(Endianness), _openScope.Stream);
			item = serializer.Deserialize(_openScope.Record.Size, reader);
		} else item = default;
		return scope;
	}

	public IDisposable EnterLockScope() {
		Monitor.Enter(_lock);
		return new ActionDisposable(() => Monitor.Exit(_lock));
	}

	public ClusteredStreamScope Add() {
		CheckInitialized();
		CheckReentrantLock();
		lock (_lock) {
			CheckNotOpened();
			AddRecord(out var index, NewRecord()); // the first record add will allocate cluster 0 for the records stream
			return BeginOpenScope(index); // scope will release _lock when closed
		}
	}

	public ClusteredStreamScope Open(long index) {
		CheckInitialized();
		CheckReentrantLock();
		return OpenInternal(index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected ClusteredStreamScope OpenInternal(long index) {
		lock (_lock) {
			CheckRecordIndex(index);
			CheckNotOpened();
			return BeginOpenScope(index); // scope will release _lock when closed
		}
	}


	public void Remove(long index) {
		CheckInitialized();
		CheckReentrantLock();
		lock (_lock) {
			var countBeforeRemove = Header.RecordsCount;
			CheckRecordIndex(index);
			CheckNotOpened();
			var record = GetRecord(index);
			if (record.StartCluster != -1)
				RemoveClusterChain(record.StartCluster);
			RemoveRecord(index); // record must be removed last, in case it deletes genesis cluster
			var countAfterRemove = Header.RecordsCount;
			if (countAfterRemove != countBeforeRemove - 1)
				throw new InvalidOperationException("Failed to remove record");
		}
	}

	public ClusteredStreamScope Insert(long index) {
		CheckInitialized();
		CheckReentrantLock();
		lock (_lock) {
			CheckRecordIndex(index, allowEnd: true);
			CheckNotOpened();
			InsertRecord(index, NewRecord());
			return BeginOpenScope(index); // scope will release _lock when closed
		}
	}

	public void Swap(long first, long second) {
		CheckInitialized();
		CheckReentrantLock();
		using (EnterLockScope()) {
			CheckRecordIndex(first);
			CheckRecordIndex(second);

			if (first == second)
				return;

			CheckNotOpened();

			// Get records
			var firstRecord = GetRecord(first);
			var secondRecord = GetRecord(second);

			// Swap records
			UpdateRecord(first, secondRecord);
			UpdateRecord(second, firstRecord);

			// Update genesis-to-record links in genesis clusters (if applicable)
			if (firstRecord.StartCluster != -1) {
				FastWriteClusterPrev(firstRecord.StartCluster, second);
			}

			if (secondRecord.StartCluster != -1) {
				FastWriteClusterPrev(secondRecord.StartCluster, first);
			}
		}
	}

	public void Clear(long index) {
		CheckInitialized();
		CheckReentrantLock();
		lock (_lock) {
			CheckRecordIndex(index);
			using (var scope = OpenInternal(index)) {
				scope.Stream.SetLength(0);
			}
		}
	}

	public void Clear() {
		CheckInitialized();
		CheckReentrantLock();
		using (EnterLockScope()) {
			CheckNotOpened();
			_records.Clear();
			_recordCache?.Flush();
			_clusters.Clear();
			Header.RecordsCount = 0;
			Header.TotalClusters = 0;
			AllRecordsSize = 0;
			_recordsFragmentProvider.Reset();
			CreateReservedRecords();
		}
	}

	public void Optimize() {
		CheckInitialized();
		CheckReentrantLock();
		// TODO: 
		//	- Organize clusters in sequential order
		//  - Do not try to organize nested ClusteredStreamStorage (dont know how to activate them)
		throw new NotImplementedException();
	}

	public override string ToString() {
		CheckInitialized();
		// Reentrancy check not required since Header is statically mapped to stream
		return Header.ToString();
	}

	internal string ToStringFullContents() {
		CheckInitialized();
		CheckReentrantLock();
		using (EnterLockScope()) {
			var stringBuilder = new FastStringBuilder();
			stringBuilder.AppendLine(this.ToString());
			stringBuilder.AppendLine("Records:");
			for (var i = 0; i < _records.Count; i++) {
				var record = GetRecord(i);
				stringBuilder.AppendLine($"\t{i}: {record}");
			}
			stringBuilder.AppendLine("Clusters:");
			for (var i = 0; i < _clusters.Count; i++) {
				var cluster = _clusters[i];
				stringBuilder.AppendLine($"\t{i}: {cluster}");
			}
			return stringBuilder.ToString();
		}
	}

	#endregion

	#region Stream Scope

	private ClusteredStreamScope BeginOpenScope(long index) {
		Guard.Ensure(_openScope == null, "A scope was already opened");
		Monitor.Enter(_lock);
		try {
			_openScope = new ClusteredStreamScope(index, GetRecord(index), EndOpenScope);
			// TODO: improve _openScope must be set before we can create FragmentProvider(this, ...) because it uses parent._openScope
			_openScope.Stream = new FragmentedStream(new FragmentProvider(this, ClusterDataType.Stream));
			return _openScope;
		} catch {
			Monitor.Exit(_lock);
			throw;
		}
	}

	private void EndOpenScope() {
		try {
			Guard.Ensure(_openScope != null);
			UpdateRecord(_openScope.RecordIndex, _openScope.Record, false);
			_openScope = null;
			//_lock.Release();
		} finally {
			Monitor.Exit(_lock);
		}
	}

	#endregion

	#region Records

	private ClusteredStreamRecord NewRecord() {
		Debug.Assert(IsLocked, "Mutating without a lock can lead to race-conditions and data corruption in multi-threaded scenarios");
		var record = new ClusteredStreamRecord();
		record.Size = 0;
		record.StartCluster = -1;
		record.Key = new byte[_recordKeySize];
		NotifyRecordCreated(record);
		return record;
	}

	// This is the interface implementation of UpdateRecord (used by friendly classes)
	public void UpdateRecord(long index, ClusteredStreamRecord record) {
		//Guard.ArgumentCast<TRecord>(record, out var recordT, nameof(record));
		UpdateRecord(index, record, true);
	}

	public ClusteredStreamRecord GetRecord(long index) {
		CheckInitialized();
		return _recordCache is not null ? _recordCache[index] : FetchRecord(index);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ClusteredStreamRecord FetchRecord(long index) {
		Debug.Assert(IsLocked, "Fetching results in mutation and requires a lock to avoid race-conditions and data corruption in multi-threaded scenarios");
		if (_openScope != null && index == _openScope.RecordIndex)
			return _openScope.Record;

		var record = _records.Read(index);
		if (_integrityChecks)
			CheckRecordIntegrity(index, record);
		return record;
	}

	private ClusteredStreamRecord AddRecord(out long index, ClusteredStreamRecord record) {
		Debug.Assert(IsLocked, "Mutating without a lock can lead to race-conditions and data corruption in multi-threaded scenarios");
		_records.Add(record);
		index = Header.RecordsCount++;
		_recordCache?.Set(index, record);
		NotifyRecordAdded(index, record);
		return record;
	}

	/// <remarks>
	/// This has O(N) complexity in worst case (inserting at 0), use with care
	/// </remarks>
	private void InsertRecord(long index, ClusteredStreamRecord record) {
		Debug.Assert(IsLocked, "Mutating without a lock can lead to race-conditions and data corruption in multi-threaded scenarios");
		// Update genesis clusters 
		for (var i = _records.Count - 1; i >= index; i--) {
			var shiftedRecord = GetRecord(i);
			if (shiftedRecord.StartCluster != -1)
				FastWriteClusterPrev(shiftedRecord.StartCluster, i + 1);
			_recordCache?.Invalidate(i);
			_recordCache?.Set(i + 1, shiftedRecord);
		}
		_records.Insert(index, record);
		_recordCache?.Set(index, record);
		Header.RecordsCount++;
		NotifyRecordInserted(index, record);
	}

	private void UpdateRecord(long index, ClusteredStreamRecord record, bool resetTrackingIfOpen) {
		Debug.Assert(IsLocked, "Mutating without a lock can lead to race-conditions and data corruption in multi-threaded scenarios");
		if (_integrityChecks)
			CheckRecordIntegrity(index, record);

		_records.Update(index, record);
		_recordCache?.Set(index, record);
		if (_openScope != null && _openScope.RecordIndex == index) {
			_openScope.Record = record;
			if (resetTrackingIfOpen)
				_openScope.ResetTracking();
		}
		NotifyRecordUpdated(index, record);
	}

	private void RemoveRecord(long index) {
		Debug.Assert(IsLocked, "Mutating without a lock can lead to race-conditions and data corruption in multi-threaded scenarios");
		for (var i = index + 1; i < _records.Count; i++) {
			var higherRecord = GetRecord(i);
			if (higherRecord.StartCluster != -1) {
				FastWriteClusterPrev(higherRecord.StartCluster, i - 1);
			}
			_recordCache?.Set(i - 1, higherRecord);
		}
		_records.RemoveAt(index);
		_recordCache?.Invalidate(index);
		Header.RecordsCount--;
		NotifyRecordRemoved(index);
	}

	#endregion

	#region Clusters

	private void AllocateNextClusters(ClusterDataType clusterDataType, long recordIndex, long previousCluster, long quantity, bool allocateFirst, out long startClusterIX) {
		if (quantity == 0) {
			startClusterIX = -1;
			return;
		}
		var typeTrait = clusterDataType switch { ClusterDataType.Record => ClusterTraits.Record, ClusterDataType.Stream => ClusterTraits.Data };
		var clusters = new Cluster[quantity];
		startClusterIX = _clusters.Count;
		var zeroData = ZeroClusterBytes.ToArray();
		for (var i = 0; i < quantity; i++) {
			clusters[i] = new Cluster {
				Traits = typeTrait,
				Prev = startClusterIX + i - 1,
				Next = startClusterIX + i + 1,
				Data = zeroData
			};
		}
		// Fix first new cluster
		if (allocateFirst) {
			clusters[0].Traits |= ClusterTraits.First;
			clusters[0].Prev = -1;
		} else {
			clusters[0].Prev = previousCluster;
		}
		// fix last cluster
		clusters[^1].Next = -1;

		if (previousCluster != -1)
			FastWriteClusterNext(previousCluster, startClusterIX);
		switch (clusterDataType) {
			case ClusterDataType.Record:
				clusters[0].Prev = allocateFirst ? -1 : previousCluster;
				break;
			case ClusterDataType.Stream:
				clusters[0].Prev = allocateFirst ? recordIndex : previousCluster;
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(clusterDataType), clusterDataType, null);
		}

		// Fix last new cluster
		clusters[^1].Next = -1;

		_clusters.AddRange(clusters);
		Header.TotalClusters += clusters.Length;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Cluster GetCluster(long clusterIndex) {
		var cluster = _clusters[clusterIndex];
		Debug.Assert(cluster != null);
		if (_integrityChecks) {
			CheckClusterTraits(clusterIndex, cluster.Traits);
			CheckClusterIndex(cluster.Next);
			if (!cluster.Traits.HasFlag(ClusterTraits.First | ClusterTraits.Data))
				CheckClusterIndex(cluster.Prev);
		}
		return cluster;
	}

	private void UpdateCluster(long clusterIndex, Cluster cluster) {
		if (_integrityChecks) {
			CheckClusterTraits(clusterIndex, cluster.Traits);
			CheckClusterIndex(cluster.Next);
			if (!cluster.Traits.HasFlag(ClusterTraits.First | ClusterTraits.Data))
				CheckClusterIndex(cluster.Prev);
		}
		_clusters[clusterIndex] = cluster;
	}

	private void RemoveCluster(long clusterIndex) {
		if (_integrityChecks) {
			CheckClusterIndex(clusterIndex, false);
		}

		var next = FastReadClusterNext(clusterIndex);
		Guard.Ensure(next == -1, $"Can only remove a cluster from end of cluster-linked chain (clusterIndex = {clusterIndex})");
		var traits = FastReadClusterTraits(clusterIndex);
		var prevPointsToRecord = traits.HasFlag(ClusterTraits.First) && traits.HasFlag(ClusterTraits.Data);
		if (!prevPointsToRecord) {
			var prev = FastReadClusterPrev(clusterIndex);
			if (prev >= Header.TotalClusters)
				throw new CorruptDataException(Header, clusterIndex, $"Prev index pointed to non-existent cluster {clusterIndex}");
			if (prev != -1)
				FastWriteClusterNext(prev, -1); // prev.next points to deleted cluster, so make it point to nothing
		}
		MigrateTipClusterTo(clusterIndex);
		var tipClusterIX = _clusters.Count - 1;

		InvalidateCachedCluster(clusterIndex);
		InvalidateCachedCluster(tipClusterIX);
		_clusters.RemoveAt(tipClusterIX);
		Header.TotalClusters--;
	}

	private void RemoveClusterChain(long startCluster) {
		var clustersRemoved = 0;
		var cluster = startCluster;
		Guard.Ensure(FastReadClusterTraits(cluster).HasFlag(ClusterTraits.First), "Cluster not a start cluster");

		while (cluster != -1) {
			var nextCluster = FastReadClusterNext(cluster);
			CheckClusterIndex(nextCluster);
			var tipIX = _clusters.Count - clustersRemoved - 1;
			// if next is the tip about to be migrated to cluster, the next is cluster
			if (tipIX == nextCluster)
				nextCluster = cluster;
			MigrateTipClusterTo(tipIX, cluster);
			InvalidateCachedCluster(tipIX);
			InvalidateCachedCluster(cluster);
			clustersRemoved++;
			cluster = nextCluster;
		}
		_clusters.RemoveRange(_clusters.Count - clustersRemoved, clustersRemoved);
		Header.TotalClusters -= clustersRemoved;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ClusterTraits FastReadClusterTraits(long clusterIndex) {
		CheckInitialized();
		_clusters.ReadItemRaw(clusterIndex, 0, 1, out var bytes);
		var traits = (ClusterTraits)bytes[0];
		if (_integrityChecks)
			CheckClusterTraits(clusterIndex, traits);
		return traits;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void FastWriteClusterTraits(long clusterIndex, ClusterTraits traits) {
		CheckInitialized();
		_clusters.WriteItemBytes(clusterIndex, Cluster.TraitsOffset, new[] { (byte)traits });
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal int FastReadClusterPrev(long clusterIndex) {
		_clusters.ReadItemRaw(clusterIndex, Cluster.PrevOffset, Cluster.PrevLength, out var bytes);
		var prevCluster = _clusters.Reader.BitConverter.ToInt32(bytes);
		if (_integrityChecks)
			CheckLinkedCluster(clusterIndex, prevCluster);
		return prevCluster;
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void FastWriteClusterPrev(long clusterIndex, long prev) {
		CheckInitialized();
		var bytes = _clusters.Writer.BitConverter.GetBytes(prev);
		_clusters.WriteItemBytes(clusterIndex, Cluster.PrevOffset, bytes);
	}

	internal long FastReadClusterNext(long clusterIndex) {
		_clusters.ReadItemRaw(clusterIndex, Cluster.NextOffset, Cluster.NextLength, out var bytes);
		var nextValue = _clusters.Reader.BitConverter.ToInt32(bytes);
		if (_integrityChecks)
			CheckLinkedCluster(clusterIndex, nextValue);
		return nextValue;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void FastWriteClusterNext(long clusterIndex, long next) {
		CheckInitialized();
		var bytes = _clusters.Writer.BitConverter.GetBytes(next);
		_clusters.WriteItemBytes(clusterIndex, Cluster.NextOffset, bytes);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void FastWriteClusterData(long clusterIndex, long offset, ReadOnlySpan<byte> data) {
		CheckInitialized();
		_clusters.WriteItemBytes(clusterIndex, Cluster.DataOffset + offset, data);
	}

	#endregion

	#region Event methods

	protected virtual void OnRecordCreated(ClusteredStreamRecord record) {
	}

	protected virtual void OnRecordAdded(long index, ClusteredStreamRecord record) {
	}

	protected virtual void OnRecordInserted(long index, ClusteredStreamRecord record) {
	}

	protected virtual void OnRecordUpdated(long index, ClusteredStreamRecord record) {
	}

	protected virtual void OnRecordRemoved(long index) {
	}

	private void NotifyRecordCreated(ClusteredStreamRecord record) {
		OnRecordCreated(record);
		RecordCreated?.Invoke(record);
	}

	private void NotifyRecordAdded(long index, ClusteredStreamRecord record) {
		OnRecordAdded(index, record);
		RecordAdded?.Invoke(index, record);
	}

	private void NotifyRecordInserted(long index, ClusteredStreamRecord record) {
		OnRecordInserted(index, record);
		RecordInserted?.Invoke(index, record);
	}

	private void NotifyRecordUpdated(long index, ClusteredStreamRecord record) {
		OnRecordUpdated(index, record);
		RecordUpdated?.Invoke(index, record);
	}

	private void NotifyRecordRemoved(long index) {
		OnRecordRemoved(index);
		RecordRemoved?.Invoke(index);
	}

	#endregion

	#region Aux methods

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckInitialized() {
		if (!_initialized)
			throw new InvalidOperationException("Clustered Storage not initialized");
	}

	private void CheckReentrantLock() {
		if (Monitor.IsEntered(_lock))
			throw new InvalidOperationException("A stream has already been opened by the current thread and must be closed");
	}

	private void CheckHeaderDataIntegrity(long rootStreamLength, ClusteredStorageHeader header, IItemSerializer<Cluster> clusterSerializer, IItemSerializer<ClusteredStreamRecord> recordSerializer) {
		var clusterEnvelopeSize = clusterSerializer.StaticSize - header.ClusterSize;
		var recordClusters = (long)Math.Ceiling(header.RecordsCount * recordSerializer.StaticSize / (float)header.ClusterSize);
		if (header.TotalClusters < recordClusters)
			throw new CorruptDataException($"Inconsistency in {nameof(ClusteredStorageHeader.TotalClusters)}/{nameof(ClusteredStorageHeader.RecordsCount)}");
		var minStreamSize = header.TotalClusters * (header.ClusterSize + clusterEnvelopeSize) + ClusteredStorageHeader.ByteLength;
		if (rootStreamLength < minStreamSize)
			throw new CorruptDataException($"Stream too small (header gives minimum size {minStreamSize} but was {rootStreamLength})");
	}

	private void CreateReservedRecords() {
		Guard.Ensure(Header.RecordsCount == 0, "Records are already existing");
		while (Header.RecordsCount < Header.ReservedRecords) {
			AddRecord(out var index, NewRecord());
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckRecordIndex(long index, string msg = null, bool allowEnd = false)
		=> Guard.CheckIndex(index, 0, _records.Count, allowEnd);

	private void CheckRecordIntegrity(long index, ClusteredStreamRecord record) {
		if (record.Size == 0) {
			if (record.StartCluster != -1)
				throw new CorruptDataException(Header, $"Empty record {index} should have start cluster -1 but was {record.StartCluster}");
		} else if (!(0 <= record.StartCluster && record.StartCluster < Header.TotalClusters))
			throw new CorruptDataException(Header, $"Record {index} pointed to to non-existent cluster {record.StartCluster}");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckClusterIndex(long index, bool allowNegOne = true, string msg = null) {
		if (allowNegOne && index == -1 || (0 <= index && index < _clusters.Count))
			return;
		throw new CorruptDataException(msg ?? $"Cluster index {index} out of bounds (or not -1)");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckLinkedCluster(long sourceCluster, int linkedCluster, string msg = null) {
		Guard.Argument(sourceCluster >= 0, nameof(sourceCluster), "Must be greater than or equal to 0");

		if (linkedCluster == -1)
			return;

		if (sourceCluster == linkedCluster)
			throw new CorruptDataException(Header, sourceCluster, $"Cluster links to itself {sourceCluster}");

		if (!(0 <= linkedCluster && linkedCluster < Header.TotalClusters))
			throw new CorruptDataException(Header, sourceCluster, msg ?? $"Cluster {sourceCluster} pointed to non-existent cluster {linkedCluster}");
	}

	private void CheckClusterTraits(long cluster, ClusterTraits traits) {
		var bTraits = (byte)traits;
		if (bTraits == 0 || bTraits > 7 || ((traits & ClusterTraits.Data) > 0 && (traits & ClusterTraits.Record) > 0))
			throw new CorruptDataException(Header, cluster, "Invalid traits");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckNotOpened() {
		if (_openScope != null)
			throw new InvalidOperationException("A stream is already opened");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void MigrateTipClusterTo(long to) => MigrateTipClusterTo(_clusters.Count - 1, to);

	private void MigrateTipClusterTo(long tipIndex, long to) {
		// Moved the right-most cluster into 'to' so that right-most can be deleted (since 'to' is now available).
		Guard.ArgumentInRange(to, 0, _clusters.Count - 1, nameof(to));
		Guard.Argument(to <= tipIndex, nameof(to), $"Cannot be greater than {nameof(tipIndex)}");

		// If migrating to self, return
		if (to == tipIndex)
			return;

		var tipCluster = GetCluster(tipIndex);
		var toCluster = GetCluster(to);
		toCluster.Traits = tipCluster.Traits;
		toCluster.Prev = tipCluster.Prev;
		toCluster.Next = tipCluster.Next;
		tipCluster.Data.AsSpan().CopyTo(toCluster.Data);
		if (tipCluster.Traits.HasFlag(ClusterTraits.First)) {
			var recordIndex = tipCluster.Prev; // convention, Prev points to Record index in first cluster of BLOB
			if (recordIndex < _records.Count) {
				var updatedRecord = GetRecord(recordIndex);
				updatedRecord.StartCluster = to;
				UpdateRecord(recordIndex, updatedRecord);
			}
		} else {
			// Update tip's previous cluster to point to new location of tip
			FastWriteClusterNext(tipCluster.Prev, to);
		}
		if (tipCluster.Next != -1)
			FastWriteClusterPrev(tipCluster.Next, to);

		_clusters.Update(to, toCluster);
	}

	private void InvalidateCachedCluster(long cluster) {
		_recordsFragmentProvider.InvalidateCluster(cluster);
		_openScope?.InvalidateCluster(cluster);
	}

	#endregion

	#region Inner Types

	internal class FragmentProvider : IStreamFragmentProvider {
		private readonly ClusteredStorage _parent;
		private readonly FragmentCache _fragmentCache;
		private readonly ClusterDataType _clusterDataType;
		private readonly bool _enableCache;
		private long _currentFragment;
		private long _currentCluster;


		public FragmentProvider(ClusteredStorage parent, ClusterDataType clusterDataType) {
			_parent = parent;
			_clusterDataType = clusterDataType;
			_fragmentCache = new FragmentCache();
			_enableCache = clusterDataType == ClusterDataType.Record && parent.Policy.HasFlag(ClusteredStoragePolicy.CacheRecordClusters) ||
			               clusterDataType == ClusterDataType.Stream && parent.Policy.HasFlag(ClusteredStoragePolicy.CacheOpenClusters);
			Reset();

		}

		public long TotalBytes => _clusterDataType switch {
			ClusterDataType.Record => _parent._allRecordsSize,
			ClusterDataType.Stream => _parent._openScope.Record.Size,
			_ => throw new ArgumentOutOfRangeException()
		};

		public long FragmentCount => (long)Math.Ceiling(TotalBytes / (float)_parent._clusterSize);

		public ReadOnlySpan<byte> GetFragment(long index) {
			Guard.ArgumentInRange(index, 0, FragmentCount - 1, nameof(index));
			TraverseToFragment(index);
			return _parent.GetCluster(_currentCluster).Data;
		}

		public bool TryMapStreamPosition(long position, out long fragmentIndex, out long fragmentPosition) {
			fragmentIndex = position / _parent._clusterSize;
			fragmentPosition = position % _parent._clusterSize;
			return true;
		}

		public void UpdateFragment(long fragmentIndex, long fragmentPosition, ReadOnlySpan<byte> updateSpan) {
			TraverseToFragment(fragmentIndex);
			var cluster = _parent.GetCluster(_currentCluster);
			var fragmentPositionI = Tools.Collection.CheckNotImplemented64bitAddressingLength(fragmentPosition);
			updateSpan.CopyTo(cluster.Data.AsSpan(fragmentPositionI));
			_parent.UpdateCluster(_currentCluster, cluster);
		}

		public bool TrySetTotalBytes(long length, out long[] newFragments, out long[] deletedFragments) {
			var oldLength = TotalBytes;
			var newTotalClusters = (long)Math.Ceiling(length / (float)_parent.ClusterSize);
			var oldTotalClusters = FragmentCount;
			var currentTotalClusters = oldTotalClusters;
			var newFragmentsL = new List<long>();
			var newClustersL = new List<long>();
			var deletedFragmentsL = new List<long>();
			var deletedClustersL = new List<long>();
			TraverseToEnd();
			if (newTotalClusters > currentTotalClusters) {
				// Fast Implementation, adds clusters in range
				var newClusters = newTotalClusters - currentTotalClusters;
				var needsFirstCluster = currentTotalClusters == 0;
				_parent.AllocateNextClusters(_clusterDataType, _parent._openScope?.RecordIndex ?? -1, _currentCluster, newClusters, needsFirstCluster, out var newStartIX);

				for (var i = 0; i < newClusters; i++) {
					var newClusterIX = newStartIX + i;
					var newFragmentIX = _currentFragment + i + 1;
					newClustersL.Add(newClusterIX);
					newFragmentsL.Add(newFragmentIX);
					if (_enableCache)
						_fragmentCache.SetCluster(newFragmentIX, newClusterIX);
				}

				// Set carrot to tip
				_currentCluster = newClustersL[^1];
				_currentFragment = newFragmentsL[^1];

			} else if (newTotalClusters < currentTotalClusters) {
				// remove clusters from tip
				while (currentTotalClusters > newTotalClusters) {
					var deleteCluster = _currentCluster;
					var deleteFragment = _currentFragment;
					var wasStartCluster = IsStartCluster(deleteCluster);
					TryStepBack();

					// Remember the current position after step back because RemoveCluster may reset this when shuffling record clusters
					var rememberCurrentCluster = _currentCluster;
					var rememberCurrentFragment = _currentFragment;
					var steppedBackToTip = rememberCurrentCluster == _parent._clusters.Count - 1; // remember this case because affects us
					_parent.RemoveCluster(deleteCluster);

					// Restore remembered position (note: when previous is tip, it got moved to deleted position when deleted was removed)
					if (wasStartCluster) {
						// deleted genesis cluster
						_currentCluster = -1;
						_currentFragment = -1;
					} else {
						_currentCluster = steppedBackToTip ? deleteCluster : rememberCurrentCluster; // this is because we deleted the tip cluster which moved back to where migrated cluster was
						_currentFragment = rememberCurrentFragment;
					}
					currentTotalClusters--;
					deletedFragmentsL.Add(deleteFragment);
					deletedClustersL.Add(deleteCluster);
				}
			}

			// Erase unused portion of tip cluster when shrinking stream
			if (length < oldLength) {
				var unusedTipClusterBytes = newTotalClusters * _parent.ClusterSize - length;
				if (unusedTipClusterBytes > 0) {
					var unusedTipClusterBytesI = Tools.Collection.CheckNotImplemented64bitAddressingLength(unusedTipClusterBytes);
					_parent.FastWriteClusterData(_currentCluster, _parent.ClusterSize - unusedTipClusterBytes, _parent.ZeroClusterBytes.Span.Slice(..unusedTipClusterBytesI));
				}
			}

			// Update record if applicable
			if (_clusterDataType == ClusterDataType.Stream) {
				_parent._openScope.Record.Size = length;
				if (_parent._openScope.Record.Size == 0)
					_parent._openScope.Record.StartCluster = -1;
				if (oldTotalClusters == 0 && newTotalClusters > 0)
					_parent._openScope.Record.StartCluster = newClustersL[0];
			} else {
				_parent.AllRecordsSize = length;
			}

			newFragments = newFragmentsL.ToArray();
			deletedFragments = deletedFragmentsL.ToArray();
			return true;
		}

		internal void Reset() {
			ResetClusterPointer();
			if (_enableCache)
				_fragmentCache.Clear();
		}

		internal void ResetClusterPointer() {
			switch (_clusterDataType) {
				case ClusterDataType.Record:
					_currentFragment = _parent._header.RecordsCount > 0 && _parent._header.TotalClusters > 0 ? 0 : -1;
					_currentCluster = _parent._header.RecordsCount > 0 && _parent._header.TotalClusters > 0 ? 0 : -1;
					break;
				case ClusterDataType.Stream:
					_currentFragment = _parent._openScope.Record.StartCluster > 0 ? 0 : -1;
					_currentCluster = _parent._openScope.Record.StartCluster > 0 ? _parent._openScope.Record.StartCluster : -1;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		internal void InvalidateCluster(long cluster) {
			if (cluster == _currentCluster)
				ResetClusterPointer();
			if (_enableCache)
				_fragmentCache.InvalidateCluster(cluster);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsStartCluster(long cluster) => _clusterDataType switch {
			ClusterDataType.Record => cluster == 0,
			ClusterDataType.Stream => cluster == _parent._openScope.Record.StartCluster,
			_ => throw new NotSupportedException($"{_clusterDataType}")
		};

		private void TraverseToStart() {
			// TODO: start moving backward from known smallest fragment 
			var steps = 0;
			while (TryStepBack())
				CheckSteps(steps++);
		}

		private void TraverseToEnd() {
			// TODO: start moving forward from known greatest fragment
			var steps = 0;
			while (TryStepForward())
				CheckSteps(steps++);
		}

		private void TraverseToFragment(long index) {
			if (_currentFragment == index)
				return;

			if (_enableCache && _fragmentCache.TryGetCluster(index, out var cluster)) {
				_currentFragment = index;
				_currentCluster = cluster;
				return;
			}

			var steps = 0;
			if (index < _currentFragment) {
				while (_currentFragment != index) {
					if (!TryStepBack())
						throw new InvalidOperationException($"Unable to seek to fragment {index}");
					CheckSteps(steps++);
				}
			} else if (index > _currentFragment)
				while (_currentFragment != index) {
					if (!TryStepForward())
						throw new InvalidOperationException($"Unable to seek to fragment {index}");
					CheckSteps(steps++);
				}
		}

		private bool TryStepBack() {
			if (_currentFragment <= 0)
				return false;

			_currentFragment--;
			// note: _currentCluster still points to current at this point
			var prevCluster = 0L;
			if (!(_enableCache && _fragmentCache.TryGetCluster(_currentFragment, out prevCluster))) {
				_currentCluster = _parent.FastReadClusterPrev(_currentCluster);
				if (_enableCache)
					_fragmentCache.SetCluster(_currentFragment, _currentCluster);
			} else _currentCluster = prevCluster;

			return true;
		}

		private bool TryStepForward() {
			if (_currentFragment < 0)
				return false;

			if (!(_enableCache && _fragmentCache.TryGetCluster(_currentFragment + 1, out var nextCluster))) {
				nextCluster = _parent.FastReadClusterNext(_currentCluster);
			}

			if (nextCluster == _currentCluster)
				return false;

			if (nextCluster == -1)
				return false;

			_currentFragment++;
			_currentCluster = nextCluster;
			if (_enableCache)
				_fragmentCache.SetCluster(_currentFragment, _currentCluster);
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckSteps(int steps) {
			if (steps > FragmentCount)
				throw new CorruptDataException(_parent._header, $"Unable to traverse the cluster-chain due to cyclic dependency (detected at cluster {_currentCluster})");
		}

	}


	internal class FragmentCache {
		private readonly IDictionary<long, long> _fragmentToClusterMap;
		private readonly IDictionary<long, long> _clusterToFragmentMap;

		public FragmentCache() {
			_fragmentToClusterMap = new Dictionary<long, long>();
			_clusterToFragmentMap = new Dictionary<long, long>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetCluster(long fragment, out long cluster) {
			return _fragmentToClusterMap.TryGetValue(fragment, out cluster);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetCluster(long fragment, long cluster) {
			_fragmentToClusterMap[fragment] = cluster;
			_clusterToFragmentMap[cluster] = fragment;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Invalidate(long fragment, long cluster) {
			_fragmentToClusterMap.Remove(fragment);
			_clusterToFragmentMap.Remove(cluster);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void InvalidateFragment(long fragment) {
			if (_fragmentToClusterMap.TryGetValue(fragment, out var cluster))
				Invalidate(fragment, cluster);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void InvalidateCluster(long cluster) {
			if (_clusterToFragmentMap.TryGetValue(cluster, out var fragment))
				Invalidate(fragment, cluster);
		}

		public void Clear() {
			_clusterToFragmentMap.Clear();
			_fragmentToClusterMap.Clear();
		}

	}

	#endregion

}
