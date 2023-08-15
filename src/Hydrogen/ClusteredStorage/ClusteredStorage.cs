// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A container of <see cref="Stream"/>'s whose contents are stored across clusters of data over a root <see cref="Stream"/> (similar in principle to how a file-system works).
/// Fundamentally, this class can function as a "virtual file system" allowing an arbitrary number of <see cref="Stream"/>'s to be stored (and changed). This class
/// also serves as the base container for implementations of <see cref="IStreamMappedList{TItem}"/>'s, <see cref="IStreamMappedDictionary{TKey,TValue}"/>'s and <see cref="IStreamMappedHashSet{TItem}"/>'s.
/// <remarks>
/// The structure of the underlying stream is depicted below:
/// [HEADER] Version: 1, Cluster Size: 32, Total ClusterMap: 10, Records: 5
/// [Records]
///   0: [StreamRecord] Size: 60, Start Cluster: 3
///   1: [StreamRecord] Size: 88, Start Cluster: 7
///   2: [StreamRecord] Size: 27, Start Cluster: 2
///   3: [StreamRecord] Size: 43, Start Cluster: 1
///   4: [StreamRecord] Size: 0, Start Cluster: -1
/// [ClusterMap]
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
///  - ClusterMap are bi-directionally linked, to allow dynamic re-sizing on the fly. 
///  - Records contain the meta-data of all the streams and the entire records stream is also serialized over clusters.
///  - Cluster traits distinguish record clusters from stream clusters. 
///  - Cluster 0, when allocated, is always the first record cluster.
///  - Records always link to the (First | Data) cluster of their stream.
///  - ClusterMap with traits (First | Data) re-purpose the Prev field to denote the record.
/// </remarks>
public class ClusteredStorage : SyncLoadableBase, IClusteredStorage {
	internal const long NullCluster = -1L;

	public event EventHandlerEx<ClusteredStreamRecord> RecordCreated;
	public event EventHandlerEx<long, ClusteredStreamRecord> RecordAdded;
	public event EventHandlerEx<long, ClusteredStreamRecord> RecordInserted;
	public event EventHandlerEx<long, ClusteredStreamRecord> RecordUpdated;
	public event EventHandlerEx<(long, ClusteredStreamRecord), (long, ClusteredStreamRecord)> RecordSwapped;
	public event EventHandlerEx<long, long> RecordSizeChanged;
	public event EventHandlerEx<long> RecordRemoved;

	private StreamMappedClusterMap _clusters;
	private UpdateOnlyList<ClusteredStreamRecord, StreamPagedList<ClusteredStreamRecord>> _records;
	private ICache<long, ClusteredStreamRecord> _recordCache;
	private ClusteredStorageHeader _header;
	private readonly HashSet<long> _openScopes;
	

	private bool _initialized;
	private readonly Stream _rootStream;
	private readonly int _clusterSize;
	private readonly long _recordKeySize;
	private readonly long _reservedRecords;
	private readonly bool _integrityChecks;
	private readonly bool _preAllocateOptimization;
	private int _clusterEnvelopeSize;
	private readonly SynchronizedObject _syncRoot;
	private bool _suppressEvents;

	public ClusteredStorage(Stream rootStream, int clusterSize = HydrogenDefaults.ClusterSize, ClusteredStoragePolicy policy = ClusteredStoragePolicy.Default, long recordKeySize = 0, long reservedRecords = 0, Endianness endianness = Endianness.LittleEndian, bool autoLoad = false) {
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
		_preAllocateOptimization = Policy.HasFlag(ClusteredStoragePolicy.FastAllocate);
		_integrityChecks = Policy.HasFlag(ClusteredStoragePolicy.IntegrityChecks);
		ZeroClusterBytes = Tools.Array.Gen<byte>(clusterSize, 0);
		_header = null;
		_initialized = false;
		_syncRoot = new SynchronizedObject();
		_openScopes = new HashSet<long>();
		_suppressEvents = false;
		if (autoLoad)
			Load();
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

	public ISynchronizedObject ParentSyncObject { get => _syncRoot.ParentSyncObject; set => _syncRoot.ParentSyncObject = value; }

	public ReaderWriterLockSlim ThreadLock => _syncRoot.ThreadLock;

	public override bool RequiresLoad {
		get => !_initialized || base.RequiresLoad || RootStream is ILoadable { RequiresLoad: true };
		set => base.RequiresLoad = value;
	}

	public Stream RootStream => _rootStream;

	public ClusteredStorageHeader Header {
		get {
			CheckInitialized();
			return _header;
		}
		private set => _header = value;
	}

	public ClusteredStoragePolicy Policy { get; }

	public Endianness Endianness { get; }

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

	public IClusterMap ClusterMap {
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

	private ReadOnlyMemory<byte> ZeroClusterBytes { get; }

	public bool SuppressEvents {
		get => _suppressEvents && _clusters.SuppressEvents;
		set {
			_suppressEvents = value;
			_clusters.SuppressEvents = value;
		}
	}

	#region Initialization & Loading
	
	public IDisposable EnterReadScope() => _syncRoot.EnterReadScope();

	public IDisposable EnterWriteScope() => _syncRoot.EnterWriteScope();

	protected override void LoadInternal() {
			if (_rootStream is ILoadable loadableStream)
				loadableStream.Load();

			if (!_initialized)
				Initialize();

			if (_clusters.RequiresLoad)
				_clusters.Load();
	}

	private void Initialize() {
		var recordSerializer = new ClusteredStreamRecordSerializer(Policy, _recordKeySize);
		var clusterSerializer = new ClusterSerializer(_clusterSize);

		// Header
		_header = new ClusteredStorageHeader(this);
		var wasEmptyStream = _rootStream.Length == 0;
		if (!wasEmptyStream) {
			_header.AttachTo(_rootStream, Endianness);
			_header.CheckHeaderIntegrity();
			CheckHeaderDataIntegrity(_rootStream.Length, _header, clusterSerializer, recordSerializer);
		} else {
			_header.CreateIn(1, _rootStream, _clusterSize, _recordKeySize, _reservedRecords, Endianness);
		}
		Guard.Ensure(_header.ClusterSize == _clusterSize, $"Inconsistent cluster size {_clusterSize} (stream header had '{_header.ClusterSize}')");

		// ClusterMap
		// - stored in a StreamPagedList (single page, statically sized items)
		// - when a start/end cluster is moved, we must update the record that points to it (the record is stored as the terminal values of a cluster chain)
		ClusterEnvelopeSize = checked((int)clusterSerializer.StaticSize) - _header.ClusterSize; // the serializer includes the envelope, the header is the data size
		_clusters = new StreamMappedClusterMap(_rootStream, ClusteredStorageHeader.ByteLength, clusterSerializer, Endianness, autoLoad: true);
		_clusters.ParentSyncObject = this;
		_clusters.ClusterCountChanged += (_, clusterCountDelta, _) => Header.TotalClusters += clusterCountDelta;   // track total clusters in header
		
		// track item stream start/end in record stream
		_clusters.ClusterChainCreated += (_, startCluster, endCluster, _, terminalValue) => {
			if (terminalValue != -1) {
				CheckWriteLocked();
				FastWriteRecordStartCluster(terminalValue, startCluster);
				FastWriteRecordEndCluster(terminalValue, endCluster);
			}
		};
		_clusters.ClusterChainStartChanged += (_, cluster, terminalValue, _) => {
			if (terminalValue != -1) {
				CheckWriteLocked();
				FastWriteRecordStartCluster(terminalValue, cluster);
				_recordCache?.Invalidate(terminalValue);
			}
		};
		_clusters.ClusterChainEndChanged += (_, cluster, terminalValue, _) => {
			if (terminalValue != -1) {
				CheckWriteLocked();
				FastWriteRecordEndCluster(terminalValue, cluster);
				_recordCache?.Invalidate(terminalValue);
			}
		};
		_clusters.ClusterMoved += (_, fromCluster, toCluster, traits, terminalValue) => {
			if (terminalValue.HasValue && terminalValue != -1) {
				CheckWriteLocked();
				if (traits.HasFlag(ClusterTraits.Start))
					FastWriteRecordStartCluster(terminalValue.Value, toCluster);
				if (traits.HasFlag(ClusterTraits.End))
					FastWriteRecordEndCluster(terminalValue.Value, toCluster);
				_recordCache?.Invalidate(terminalValue.Value);
			}
		};
		_clusters.ClusterChainRemoved += (_, terminalValue) => {
			if (terminalValue != -1) {
				CheckWriteLocked();
				FastWriteRecordStartCluster(terminalValue, -1);
				FastWriteRecordEndCluster(terminalValue, -1);
				FastWriteRecordSize(terminalValue, 0);
				_recordCache?.Invalidate(terminalValue);
			}
		};

		// Records
		//  - are stored StreamPagedList or ClusteredStreamRecords (single page, statically sized items) which mapped over the cluster chain starting from 0
		//  - the end cluster of the cluster chain is tracked in the header
		//  - the record count is also tracked in the header
		//  - this list of records maintains all the other lists stored in the cluster container
		var recordsCount = _header.RecordsCount;
		var recordsFragmentProvider = new ClusteredStreamFragmentProvider(
			_clusters,
			-1,
			_header.RecordsCount * recordSerializer.StaticSize,
			() => {
				if (_header.RecordsCount == 0) {
					if (_header.RecordsEndCluster != -1)
							return new ClusterChain{ StartCluster = 0, EndCluster = _header.RecordsEndCluster, TotalClusters = 1 }; // when it writes first time
						else
							return new ClusterChain{ StartCluster = -1, EndCluster = -1, TotalClusters = 0 };
				}
				Guard.Ensure(Header.RecordsCount == _records.Count, "Header state / record collection mismatch");
				return new ClusterChain{ StartCluster = 0, EndCluster = _header.RecordsEndCluster, TotalClusters = _clusters.CalculateClusterChainLength( _records.Count * recordSerializer.StaticSize) };
			}
		);

		// track record stream end cluster in header
		_clusters.ClusterChainCreated += (_, startCluster, endCluster, _, terminalValue) => {
			if (terminalValue == -1) {
				CheckWriteLocked();
				Guard.Ensure(startCluster == 0, $"Record chain created with invalid start cluster {startCluster}.");
				Header.RecordsEndCluster = endCluster;
			}
		};
		_clusters.ClusterChainStartChanged += (_, _, terminalValue, _) => {
			Guard.Against(terminalValue == -1, "Record cluster chain start cannot be changed");
		};
		_clusters.ClusterChainEndChanged += (_, cluster, terminalValue, _) => {
			if (terminalValue == -1) {
				CheckWriteLocked();
				Header.RecordsEndCluster = cluster;
			}
		};
		_clusters.ClusterMoved += (_, fromCluster, toCluster, traits, terminalValue) => {
			if (terminalValue == -1) {
				CheckWriteLocked();
				Guard.Against(traits.HasFlag(ClusterTraits.Start), "Record cluster chain start cannot be moved");
				Guard.Ensure(traits.HasFlag(ClusterTraits.End), "Only end of record cluster chain can be moved");
				Header.RecordsEndCluster = toCluster;
			}
		};
		_clusters.ClusterChainRemoved += (_, terminalValue) => {
			if (terminalValue == -1) {
				CheckWriteLocked();
				Header.RecordsEndCluster = -1;
			}
		};

		// track record stream length in header
		recordsFragmentProvider.StreamLengthChanged += (_, newLength) => {
			if (_suppressEvents) // event generated from fragment provider
				return;

			CheckWriteLocked();
			_header.RecordsCount = newLength / recordSerializer.StaticSize;
		};

		// The actual records collection is stored is update optimized
		_records = 
			new StreamPagedList<ClusteredStreamRecord>(
				recordSerializer,
				new FragmentedStream(recordsFragmentProvider,  _header.RecordsCount * recordSerializer.StaticSize),
				Endianness,
				includeListHeader: false,
				autoLoad: true
			)
			.AsUpdateOnly(
				_header.RecordsCount,
				PreAllocationPolicy.MinimumRequired,
				0,
				NewRecord
			);

		if (Policy.HasFlag(ClusteredStoragePolicy.CacheRecords)) {
			_recordCache = new ActionCache<long, ClusteredStreamRecord>(
				FetchRecord,
				sizeEstimator: _ => recordSerializer.StaticSize,
				reapStrategy: CacheReapPolicy.LeastUsed,
				ExpirationPolicy.SinceLastAccessedTime,
				maxCapacity: HydrogenDefaults.RecordCacheSize
			);
			RecordSwapped += (record1, record2) => {
				_recordCache.Invalidate(record1.Item1);
				_recordCache.Invalidate(record2.Item1);
			};
		} else {
			_recordCache = null;
		}

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
			ListOperationType.Update => OpenWrite(index),
			ListOperationType.Insert => Insert(index),
			_ => throw new ArgumentException($@"List operation type '{operationType}' not supported", nameof(operationType)),
		};
		try {
			using var writer = new EndianBinaryWriter(EndianBitConverter.For(Endianness), scope.Stream);
			if (item != null) {
				scope.Record.Traits = scope.Record.Traits.CopyAndSetFlags(ClusteredStreamTraits.IsNull, false);
				if (_preAllocateOptimization) {
					// pre-setting the stream length before serialization improves performance since it avoids
					// re-allocating fragmented stream on individual properties of the serialized item
					var expectedSize = serializer.CalculateSize(item);
					scope.Stream.SetLength(expectedSize);
					serializer.Serialize(item, writer);
				} else {
					var byteLength = serializer.Serialize(item, writer);
					scope.Stream.SetLength(byteLength);
				}

			} else {
				scope.Record.Traits = scope.Record.Traits.CopyAndSetFlags(ClusteredStreamTraits.IsNull, true);
				scope.Stream.SetLength(0); // open record will save when closed
			}
			return scope;
		} catch {
			// need to dispose explicitly if not returned
			scope.Dispose(); 
			throw;
		}
	}

	public ClusteredStreamScope EnterLoadItemScope<TItem>(long index, IItemSerializer<TItem> serializer, out TItem item) {
		// initialized and reentrancy checks done by Open
		var scope = OpenRead(index);
		try {
			if (!scope.Record.Traits.HasFlag(ClusteredStreamTraits.IsNull)) {
				using var reader = new EndianBinaryReader(EndianBitConverter.For(Endianness), scope.Stream);
				item = serializer.Deserialize(scope.Record.Size, reader);
			} else item = default;
			return scope;
		} catch {
			// need to dispose explicitly if not returned
			scope.Dispose();
			throw;
		}
	}

	public ClusteredStreamScope Add() {
		CheckInitialized();
		using (EnterWriteScope()) {
			AddRecord(out var index, NewRecord()); // the first record add will allocate cluster 0 for the records stream
			return NewRecordScope(index, false);
		}
	}

	public ClusteredStreamScope OpenRead(long index) {
		CheckInitialized();
		CheckRecordIndex(index);
		return NewRecordScope(index, true);
	}

	public ClusteredStreamScope OpenWrite(long index) {
		CheckInitialized();
		CheckRecordIndex(index);
		return NewRecordScope(index, false);
	}

	public void Remove(long index) {
		CheckInitialized();
		using (EnterWriteScope()) {
			CheckNoOpenScopes();
			var countBeforeRemove = Header.RecordsCount;
			CheckRecordIndex(index);
			var record = GetRecord(index);
			Guard.Against(record.Size == 0 && (record.StartCluster != -1 || record.EndCluster != -1), "Invalid empty record");
			if (record.Size > 0) {
				_clusters.RemoveNextClusters(record.StartCluster);
			}
			RemoveRecord(index); // record must be removed last, in case it deletes genesis cluster
			var countAfterRemove = Header.RecordsCount;
			Guard.Ensure(countAfterRemove == countBeforeRemove - 1, $"Failed to remove record {index}");
			#if ENABLE_CLUSTER_DIAGNOSTICS
			ClusterDiagnostics.VerifyClusters(this);
			#endif
		}
	}

	public ClusteredStreamScope Insert(long index) {
		CheckInitialized();
		using (EnterWriteScope()) {
			CheckNoOpenScopes();
			CheckRecordIndex(index, allowEnd: true);
			InsertRecord(index, NewRecord());
			return NewRecordScope(index, false);
		}
	}

	public void Swap(long first, long second) {
		CheckInitialized();
		using (EnterWriteScope()) {
			Guard.Ensure(!_openScopes.Contains(first), $"Cannot swap record {first} it is open");
			Guard.Ensure(!_openScopes.Contains(second), $"Cannot swap record {second} it is open");

			CheckRecordIndex(first);
			CheckRecordIndex(second);

			if (first == second)
				return;

			// Get records
			var firstRecord = GetRecord(first);
			var secondRecord = GetRecord(second);

			// Swap records
			UpdateRecord(first, secondRecord);
			UpdateRecord(second, firstRecord);

			// Update cluster -> record backlinks  (if applicable)
			if (firstRecord.StartCluster != -1)
				_clusters.FastWriteClusterPrev(firstRecord.StartCluster, second);
			if (firstRecord.EndCluster != -1)
				_clusters.FastWriteClusterNext(firstRecord.EndCluster, second);
			if (secondRecord.StartCluster != -1)
				_clusters.FastWriteClusterPrev(secondRecord.StartCluster, first);
			if (secondRecord.EndCluster != -1)
				_clusters.FastWriteClusterNext(secondRecord.EndCluster, first);
			
			NotifyRecordSwapped(first, firstRecord, second, secondRecord);

			#if ENABLE_CLUSTER_DIAGNOSTICS
			ClusterDiagnostics.VerifyClusters(this);
			#endif
		}
	}

	public void Clear(long index) {
		CheckInitialized();
		using (EnterWriteScope()) {
			CheckRecordIndex(index);
			using (var scope = OpenWrite(index)) {
				scope.Stream.SetLength(0);
			}
		}
	}

	public void Clear() {
		CheckInitialized();
		using (EnterWriteScope()) {
			CheckNoOpenScopes();
			SuppressEvents = true;
			try {
				_records.Clear();
				_recordCache?.Flush();
				_clusters.Clear();
				Header.RecordsCount = 0;
				Header.TotalClusters = 0;
				Header.RecordsEndCluster = -1;
				Header.ResetMerkleRoot();
				CreateReservedRecords();
			} finally {
				SuppressEvents = false;
			}
			#if ENABLE_CLUSTER_DIAGNOSTICS
			ClusterDiagnostics.VerifyClusters(this);
			#endif
		}
	}

	public void Optimize() {
		CheckInitialized();
		using (EnterWriteScope()) {
			// TODO: 
			//	- Organize clusters in sequential order
			//  - Do not try to organize nested ClusteredStreamStorage (dont know how to activate them)
			throw new NotImplementedException();
		}
	}

	public override string ToString() {
		CheckInitialized();
		// Reentrancy check not required since Header is statically mapped to stream
		return Header.ToString();
	}

	#endregion

	#region Records

	public ClusteredStreamRecord GetRecord(long index) {
		CheckInitialized();
		return _recordCache is not null ? _recordCache[index] : FetchRecord(index);
	}

	// This is the interface implementation of UpdateRecord (used by friendly classes)
	internal void UpdateRecord(long index, ClusteredStreamRecord record) {
		//Guard.ArgumentCast<TRecord>(record, out var recordT, nameof(record));
		using (EnterWriteScope()) {
			if (_integrityChecks)
				CheckRecordIntegrity(index, record);
			_records.Update(index, record);
			_recordCache?.Set(index, record);
			NotifyRecordUpdated(index, record);
		}
	}

	private ClusteredStreamRecord NewRecord() {
		using (EnterWriteScope()) {
			var record = new ClusteredStreamRecord();
			record.Size = 0;
			record.StartCluster = NullCluster;
			record.EndCluster = NullCluster;
			record.Key = new byte[_recordKeySize];
			NotifyRecordCreated(record);
			return record;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ClusteredStreamRecord FetchRecord(long index) {
		using (EnterReadScope()) {
			var record = _records.Read(index);
			if (_integrityChecks)
				CheckRecordIntegrity(index, record);
			return record;
		}
	}

	private ClusteredStreamRecord AddRecord(out long index, ClusteredStreamRecord record) {
		using (EnterWriteScope()) {
			_records.Add(record);
			index = Header.RecordsCount - 1;
			_recordCache?.Set(index, record);
			NotifyRecordAdded(index, record);
			return record;
		}
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FastWriteRecordStartCluster(long record, long startCluster) {
		if (record == 51 && startCluster == 503) {
			var x= 1;
		}
		var bytes = _records.InternalCollection.Writer.BitConverter.GetBytes(startCluster);
		_records.InternalCollection.WriteItemBytes(record, ClusteredStreamRecord.StartClusterOffset, bytes);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FastWriteRecordEndCluster(long record, long endCluster) {
		var bytes = _records.InternalCollection.Writer.BitConverter.GetBytes(endCluster);
		_records.InternalCollection.WriteItemBytes(record, ClusteredStreamRecord.EndClusterOffset, bytes);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FastWriteRecordSize(long record, long size) {
		var bytes = _records.InternalCollection.Writer.BitConverter.GetBytes(size);
		_records.InternalCollection.WriteItemBytes(record, ClusteredStreamRecord.SizeOffset, bytes);
	}

	/// <remarks>
	/// This has O(N) complexity in worst case (inserting at 0), use with care
	/// </remarks>
	private void InsertRecord(long index, ClusteredStreamRecord record) {
		using (EnterWriteScope()) {
			// Update genesis clusters 
			for (var i = _records.Count - 1; i >= index; i--) {
				var shiftedRecord = GetRecord(i);
				if (shiftedRecord.StartCluster != NullCluster)
					_clusters.FastWriteClusterPrev(shiftedRecord.StartCluster, i + 1);
				if (shiftedRecord.EndCluster != NullCluster)
					_clusters.FastWriteClusterNext(shiftedRecord.EndCluster, i + 1);
				_recordCache?.Invalidate(i);
				_recordCache?.Set(i + 1, shiftedRecord);
			}
			_records.Insert(index, record);
			_recordCache?.Set(index, record);
			//Header.RecordsCount++;
			NotifyRecordInserted(index, record);
		}
	}

	private void RemoveRecord(long index) {
		using (EnterWriteScope()) {
			// Need to update terminals of adjacent records (O(N/2) complexity here)
			for (var i = index + 1; i < _records.Count; i++) {
				var higherRecord = GetRecord(i);
				if (higherRecord.StartCluster != NullCluster) 
					_clusters.FastWriteClusterPrev(higherRecord.StartCluster, i - 1);
				if (higherRecord.EndCluster != NullCluster) 
					_clusters.FastWriteClusterNext(higherRecord.EndCluster, i - 1);
				_recordCache?.Invalidate(i);
				_recordCache?.Set(i - 1, higherRecord);
			}
			
			_records.RemoveAt(index); 
			_recordCache?.Invalidate(index);
			// Note: Header.RecordsCount is adjusted automatically when removing from the collection
			NotifyRecordRemoved(index);
		}
	}

	private ClusteredStreamScope NewRecordScope(long recordIndex, bool readOnly) {
		Guard.Ensure(!_openScopes.Contains(recordIndex), $"Record {recordIndex} is already open");

		if (readOnly)
			ThreadLock.EnterReadLock();
		else
			ThreadLock.EnterWriteLock();
		var scope = new ClusteredStreamScope(this, recordIndex, readOnly, EndScopeCleanup);
		if (!readOnly) {
			scope.RecordSizeChanged += size => {
				CheckWriteLocked();
				FastWriteRecordSize(recordIndex, size); 
				_recordCache?.Invalidate(recordIndex);
			};
		}
		_openScopes.Add(recordIndex);
		return scope;

		void EndScopeCleanup() {
			if (readOnly)
				ThreadLock.ExitReadLock();
			else
				ThreadLock.ExitWriteLock();
			_openScopes.Remove(recordIndex);
		}
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
		if (_suppressEvents)
			return;

		OnRecordCreated(record);
		RecordCreated?.Invoke(record);
	}

	private void NotifyRecordAdded(long index, ClusteredStreamRecord record) {
		if (_suppressEvents)
			return;

		OnRecordAdded(index, record);
		RecordAdded?.Invoke(index, record);
	}

	private void NotifyRecordInserted(long index, ClusteredStreamRecord record) {
		if (_suppressEvents)
			return;

		OnRecordInserted(index, record);
		RecordInserted?.Invoke(index, record);
	}

	private void NotifyRecordUpdated(long index, ClusteredStreamRecord record) {
		if (_suppressEvents)
			return;

		OnRecordUpdated(index, record);
		RecordUpdated?.Invoke(index, record);
	}

	private void NotifyRecordSwapped(long record1Index, ClusteredStreamRecord record1Data, long record2Index, ClusteredStreamRecord record2Data) {
		if (_suppressEvents)
			return;

		RecordSwapped?.Invoke((record1Index, record1Data), (record2Index, record2Data));
	}

	private void NotifyRecordSizeChanged(long index, long newSize) {
		if (_suppressEvents)
			return;

		RecordSizeChanged?.Invoke(index, newSize);
	}

	private void NotifyRecordRemoved(long index) {
		if (_suppressEvents)
			return;

		OnRecordRemoved(index);
		RecordRemoved?.Invoke(index);
	}

	#endregion

	#region Aux methods

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckWriteLocked() => Guard.Ensure(ThreadLock.IsWriteLockHeld, "Write-lock is required for this operation");
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckNoOpenScopes() => Guard.Ensure(_openScopes.Count == 0, "This operation cannot be executed whilst there are open scopes");

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckInitialized() {
		if (!_initialized)
			throw new InvalidOperationException("Clustered Storage not initialized");
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
		for(var i = 0; i < Header.ReservedRecords; i++) {
			AddRecord(out var index, NewRecord());
		}
		Header.RecordsCount = _records.Count; // this has to be done explicitly here since the handler which sets RecordCount may not be called in certain scenarios
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckRecordIndex(long index, string msg = null, bool allowEnd = false)
		=> Guard.CheckIndex(index, 0, _records.Count, allowEnd);

	private void CheckRecordIntegrity(long index, ClusteredStreamRecord record) {
		if (record.Size == 0) {
			if (record.StartCluster != NullCluster)
				throw new CorruptDataException(Header, $"Empty record {index} should have start cluster {NullCluster} but was {record.StartCluster}");
			if (record.EndCluster != NullCluster)
				throw new CorruptDataException(Header, $"Empty record {index} should have end cluster {NullCluster} but was {record.EndCluster}");
		} else if (!(0 <= record.StartCluster && record.StartCluster < Header.TotalClusters))
			throw new CorruptDataException(Header, $"Record {index} pointed to to non-existent cluster {record.StartCluster}");
	}
	
	#endregion

}