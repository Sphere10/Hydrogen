using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sphere10.Framework {



	/// <summary>
	/// Base implementation of Clustered stream container. The main consumer-level sub-class is <see cref="ClusteredStorage"/>, which
	/// can be combined with decorators <see cref="MerkleStreamStorage" /> and <see cref="MerkleStreamStorage" />.
	/// <typeparam name="TRecord">Type which maintains the stream record (customizable)</typeparam>
	/// <typeparam name="THeader">Type which maintains the header of the stream storage (customizable)</typeparam>
	/// <remarks>
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
	///  - Header is fixed 256b, and can be expanded to include other data (passwords, merkle roots, etc)
	///  - Clusters are bi-directionally linked, to allow dynamic re-sizing on the fly 
	///  - Records contain the meta-data of all the streams and the entire records stream is also serialized over clusters.
	///  - Cluster traits distinguish record clusters from stream clusters. 
	///  - Cluster 0, when allocated, is always the first record cluster.
	///  - Records always link to the (First | Data) cluster of their stream.
	///  - Clusters with traits (First | Data) re-purpose the Prev field to denote the record.
	/// </remarks>
	public abstract class ClusteredStorageBase<THeader, TRecord> : IStreamStorage<THeader, TRecord>
		where THeader : ClusteredStorageHeader
		where TRecord : IClusteredRecord {

		private readonly StreamPagedList<Cluster> _clusters;
		private readonly FragmentProvider _recordsFragmentProvider;
		private readonly PreAllocatedList<TRecord> _records;
		private int? _openRecord;
		private FragmentProvider _openFragmentProvider;
		private Stream _openStream;
		private readonly object _lock;

		protected ClusteredStorageBase(Stream rootStream, int clusterSize, IItemSerializer<TRecord> recordSerializer, Endianness endianness = Endianness.LittleEndian, ClusteredStorageCachePolicy recordsCachePolicy = ClusteredStorageCachePolicy.None) {
			Guard.ArgumentNotNull(rootStream, nameof(rootStream));
			Guard.ArgumentInRange(clusterSize, 1, int.MaxValue, nameof(clusterSize));
			Guard.ArgumentNotNull(recordSerializer, nameof(recordSerializer));
			Guard.Argument(recordSerializer.IsStaticSize, nameof(recordSerializer), "Records must be constant sized");
			var clusterSerializer = new ClusterSerializer(clusterSize);
			Header = CreateHeader();
			if (rootStream.Length > 0) {
				Header.AttachTo(rootStream, endianness);
				Header.CheckHeaderIntegrity();
				CheckHeaderDataIntegrity((int)rootStream.Length, Header, clusterSerializer, recordSerializer);
			} else {
				Header.CreateIn(1, rootStream, clusterSize, endianness);
			}
			Guard.Argument(ClusterSize == clusterSize, nameof(rootStream), $"Inconsistent cluster sizes (stream header had '{ClusterSize}')");
			AllRecordsSize = Header.RecordsCount * recordSerializer.StaticSize;

			// Clusters are stored in a StreamPagedList (single page, statically sized items)
			_clusters = new StreamPagedList<Cluster>(
				clusterSerializer,
				new NonClosingStream(new BoundedStream(rootStream, ClusteredStorageHeader.ByteLength, long.MaxValue) { UseRelativeOffset = true, AllowResize = true }),
				endianness
			) { IncludeListHeader = false };
			if (_clusters.RequiresLoad)
				_clusters.Load();

			// Records are stored in record 0 as StreamPagedList (single page, statically sized items) which maps over the fragmented stream 
			_recordsFragmentProvider = new FragmentProvider(this, recordsCachePolicy);
			var recordStorage = new StreamPagedList<TRecord>(
				recordSerializer,
				new FragmentedStream(_recordsFragmentProvider, Header.RecordsCount * recordSerializer.StaticSize),
				endianness
			) { IncludeListHeader = false };
			if (recordStorage.RequiresLoad)
				recordStorage.Load();

			// The actual records collection is a PreAllocated list over the StreamPagedList which allows INSERTS in the form of UPDATES.
			_records = new PreAllocatedList<TRecord>(
				recordStorage,
				Header.RecordsCount,
				PreAllocationPolicy.MinimumRequired,
				0,
				NewRecord
			);
			ClusterEnvelopeSize = clusterSerializer.StaticSize - ClusterSize;
			_lock = new object();
			_openFragmentProvider = null;
			_openStream = null;
			_openRecord = null;
			ZeroClusterBytes = Tools.Array.Gen<byte>(clusterSize, 0);
			IntegrityChecks = true;
		}

		public THeader Header { get; }

		public int Count => Header.RecordsCount;

		public ClusteredStorageCachePolicy DefaultStreamPolicy { get; set; } = ClusteredStorageCachePolicy.None;

		public IReadOnlyList<TRecord> Records => _records;

		public bool IntegrityChecks { get; set; }

		internal IReadOnlyList<Cluster> Clusters => _clusters;

		internal int ClusterSize => Header.ClusterSize;

		internal int ClusterEnvelopeSize { get; }

		internal int AllRecordsSize { get; private set; }

		private ReadOnlyMemory<byte> ZeroClusterBytes { get; }

		#region Streams

		public Stream Add() => Add(DefaultStreamPolicy);

		public Stream Add(ClusteredStorageCachePolicy cachePolicy) {
			lock (_lock) {
				CheckNotOpened();
				var record = AddRecord(out var index, CreateRecord());  // the first record add will allocate cluster 0 for the records stream
				return Open(index, cachePolicy);
			}
		}

		public Stream Open(int index) => Open(index, DefaultStreamPolicy);

		public Stream Open(int index, ClusteredStorageCachePolicy cachePolicy) {
			CheckRecordIndex(index);
			lock (_lock) {
				CheckNotOpened();
				_openFragmentProvider = new FragmentProvider(this, index, cachePolicy);
				_openStream = new FragmentedStream(_openFragmentProvider).OnDispose(() => { _openStream = null; _openRecord = null; _openFragmentProvider = null; });
				_openRecord = index;
				return _openStream;
			}
		}

		public void Remove(int index) {
			CheckRecordIndex(index);
			lock (_lock) {
				CheckNotOpened();
				var record = GetRecord(index);
				if (record.StartCluster != -1)
					RemoveClusterChain(record.StartCluster);
				RemoveRecord(index); // record must be removed last, in case it deletes genesis cluster
			}
		}

		public Stream Insert(int index) => Insert(index, DefaultStreamPolicy);

		public Stream Insert(int index, ClusteredStorageCachePolicy cachePolicy) {
			CheckRecordIndex(index, allowEnd: true);
			lock (_lock) {
				CheckNotOpened();
				InsertRecord(index, CreateRecord());
				return Open(index, cachePolicy);
			}
		}

		public void Swap(int first, int second) {
			CheckRecordIndex(first);
			CheckRecordIndex(second);

			if (first == second)
				return;

			lock (_lock) {
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

		public void Clear(int index) {
			CheckRecordIndex(index);
			using (var stream = Open(index)) {
				stream.SetLength(0);
			}
		}

		public void Clear() {
			lock (_lock) {
				CheckNotOpened();
				_records.Clear();
				_clusters.Clear();
				Header.RecordsCount = 0;
				Header.TotalClusters = 0;
				AllRecordsSize = 0;
				_recordsFragmentProvider.Reset();
			}
		}

		public void Optimize() {
			// TODO: 
			//	- Organize clusters in sequential order
			//  - Do not try to organize nested ClusteredStreamStorage (dont know how to activate them)
			throw new NotImplementedException();
		}

		public override string ToString() => Header.ToString();

		internal string ToStringFullContents() {
			var stringBuilder = new FastStringBuilder();
			stringBuilder.AppendLine(this.ToString());
			stringBuilder.AppendLine("Records:");
			for (var i = 0; i < _records.Count; i++) {
				var record = _records[i];
				stringBuilder.AppendLine($"\t{i}: {record}");
			}
			stringBuilder.AppendLine("Clusters:");
			for (var i = 0; i < _clusters.Count; i++) {
				var cluster = _clusters[i];
				stringBuilder.AppendLine($"\t{i}: {cluster}");
			}

			return stringBuilder.ToString();
		}

		protected abstract THeader CreateHeader();

		#endregion

		#region Records

		protected abstract TRecord NewRecord();

		private void UpdateRecord(int index, TRecord record, bool resetTrackingIfOpen = true) {
			if (IntegrityChecks)
				CheckRecordIntegrity(index, record);

			_records.Update(index, record);
			if (resetTrackingIfOpen && _openRecord.HasValue && _openRecord.Value == index)
				_openFragmentProvider.Reset();
		}

		// This is the interface implementation of UpdateRecord (used by friendly classes)
		void IStreamStorage<THeader, TRecord>.UpdateRecord(int index, IStreamRecord record) {
			Guard.ArgumentCast<TRecord>(record, out var recordT, nameof(record));
			UpdateRecord(index, recordT);
		}

		private TRecord CreateRecord() {
			var record = NewRecord();
			record.Size = 0;
			record.StartCluster = -1;
			return record;
		}

		private TRecord GetRecord(int index) {
			var record = _records.Read(index);
			if (IntegrityChecks)
				CheckRecordIntegrity(index, record);
			return record;
		}

		private TRecord AddRecord(out int index, TRecord record) {
			_records.Add(record);
			Header.RecordsCount++;
			index = _records.Count - 1;
			return record;
		}

		private void InsertRecord(int index, TRecord record) {
			Debug.Assert(_openStream == null);
			// Update genesis clusters 
			for (var i = index; i < _records.Count; i++) {
				var shiftedRecord = GetRecord(i);
				if (shiftedRecord.StartCluster != -1)
					FastWriteClusterPrev(shiftedRecord.StartCluster, i + 1);
			}
			_records.Insert(index, record);
			Header.RecordsCount++;
		}

		private void RemoveRecord(int index) {
			for (var i = index + 1; i < _records.Count; i++) {
				var higherRecord = GetRecord(i);
				if (higherRecord.StartCluster != -1)
					FastWriteClusterPrev(higherRecord.StartCluster, i - 1);
			}
			_records.RemoveAt(index);
			Header.RecordsCount--;
		}

		#endregion

		#region Clusters

		private void AllocateNextClusters(ClusterDataType clusterDataType, int recordIndex, int previousCluster, int quantity, bool allocateFirst, out int startClusterIX) {
			if (quantity == 0) {
				startClusterIX = -1;
				return;
			}
			var typeTrait = clusterDataType switch { ClusterDataType.Record => ClusterTraits.Record, ClusterDataType.Stream => ClusterTraits.Data };
			var clusters = new Cluster[quantity];
			startClusterIX = _clusters.Count;
			var zeroData = ZeroClusterBytes.ToArray();
			for(var i = 0; i < quantity; i++) {
				clusters[i] = new Cluster {
					Traits = typeTrait,
					Prev = startClusterIX + i - 1,
					Next = startClusterIX + i + 1,
					Data = zeroData
				};
			}
			// Fix first new cluster
			if (allocateFirst) 
				clusters[0].Traits |= ClusterTraits.First;
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
		private Cluster GetCluster(int clusterIndex) {
			var cluster = _clusters[clusterIndex];
			if (IntegrityChecks) {
				CheckClusterTraits(clusterIndex, cluster.Traits);
				CheckClusterIndex(cluster.Next);
				if (!cluster.Traits.HasFlag(ClusterTraits.First | ClusterTraits.Data))
					CheckClusterIndex(cluster.Prev);
			}
			return cluster;
		}

		private void UpdateCluster(int clusterIndex, Cluster cluster) => _clusters[clusterIndex] = cluster;

		private void RemoveCluster(int clusterIndex) {
			var next = FastReadClusterNext(clusterIndex);
			Guard.Ensure(next == -1, $"Can only remove a cluster from end of cluster-linked chain (clusterIndex = {clusterIndex})");
			var traits = FastReadClusterTraits(clusterIndex);
			var prevPointsToRecord = traits.HasFlag(ClusterTraits.First) && traits.HasFlag(ClusterTraits.Data);
			if (!prevPointsToRecord) {
				var prev = FastReadClusterPrev(clusterIndex);
				if (prev >= Header.TotalClusters)
					throw new CorruptDataException(Header, clusterIndex, $"Prev index pointed to non-existent cluster {clusterIndex}");
				if (prev != -1)
					FastWriteClusterNext(prev, -1);  // prev.next points to deleted cluster, so make it point to nothing
			}
			MigrateTipClusterTo(clusterIndex);
			var tipClusterIX = _clusters.Count - 1;

			InvalidateCachedCluster(clusterIndex);
			InvalidateCachedCluster(tipClusterIX);
			_clusters.RemoveAt(tipClusterIX);
			Header.TotalClusters--;
		}

		private void RemoveClusterChain(int startCluster) {
			var clustersRemoved = 0;
			var cluster = startCluster;
			Guard.Ensure(FastReadClusterTraits(cluster).HasFlag(ClusterTraits.First), "Cluster not a start cluster");

			while (cluster != -1) {
				var nextCluster = FastReadClusterNext(cluster);
				CheckClusterIndex(nextCluster, "Corrupt data");
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
		internal ClusterTraits FastReadClusterTraits(int clusterIndex) {
			_clusters.ReadItemRaw(clusterIndex, 0, 1, out var bytes);
			var traits = (ClusterTraits)bytes[0];
			if (IntegrityChecks)
				CheckClusterTraits(clusterIndex, traits);
			return traits;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void FastWriteClusterTraits(int clusterIndex, ClusterTraits traits) {
			_clusters.WriteItemBytes(clusterIndex, 0, new byte[(byte)traits]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal int FastReadClusterPrev(int clusterIndex) {
			_clusters.ReadItemRaw(clusterIndex, sizeof(byte), 4, out var bytes);
			var prevCluster = _clusters.Reader.BitConverter.ToInt32(bytes);
			if (IntegrityChecks)
				CheckLinkedCluster(clusterIndex, prevCluster);
			return prevCluster;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void FastWriteClusterPrev(int clusterIndex, int prev) {
			var bytes = _clusters.Writer.BitConverter.GetBytes(prev);
			_clusters.WriteItemBytes(clusterIndex, sizeof(byte), bytes);
		}

		private int FastReadClusterNext(int clusterIndex) {
			_clusters.ReadItemRaw(clusterIndex, sizeof(byte) + sizeof(int), 4, out var bytes);
			var nextValue = _clusters.Reader.BitConverter.ToInt32(bytes);
			if (IntegrityChecks)
				CheckLinkedCluster(clusterIndex, nextValue);
			return nextValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void FastWriteClusterNext(int clusterIndex, int next) {
			var bytes = _clusters.Writer.BitConverter.GetBytes(next);
			_clusters.WriteItemBytes(clusterIndex, sizeof(byte) + sizeof(int), bytes);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void FastWriteClusterData(int clusterIndex, int offset, ReadOnlySpan<byte> data) {
			_clusters.WriteItemBytes(clusterIndex, sizeof(byte) + sizeof(int) + sizeof(int) + offset, data);
		}

		#endregion

		#region Aux methods

		private void CheckHeaderDataIntegrity(int rootStreamLength, ClusteredStorageHeader header, IItemSerializer<Cluster> clusterSerializer, IItemSerializer<TRecord> recordSerializer) {
			var clusterEnvelopeSize = clusterSerializer.StaticSize - header.ClusterSize;
			var recordClusters = (int)Math.Ceiling(header.RecordsCount * recordSerializer.StaticSize / (float)header.ClusterSize);
			if (header.TotalClusters < recordClusters)
				throw new CorruptDataException($"Inconsistency in {nameof(ClusteredStorageHeader.TotalClusters)}/{nameof(ClusteredStorageHeader.RecordsCount)}");
			var minStreamSize = header.TotalClusters * (header.ClusterSize + clusterEnvelopeSize) + ClusteredStorageHeader.ByteLength;
			if (rootStreamLength < minStreamSize)
				throw new CorruptDataException($"Stream too small (header gives minimum size {minStreamSize} but was {rootStreamLength})");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckRecordIndex(int index, string msg = null, bool allowEnd = false)
			=> Guard.Argument(0 <= index && allowEnd ? index <= _records.Count : index < _records.Count, nameof(index), msg ?? "Index out of bounds");

		private void CheckRecordIntegrity(int index, TRecord record) {
			if (record.Size == 0) {
				if (record.StartCluster != -1)
					throw new CorruptDataException(Header, $"Empty record {index} should have start cluster -1 but was {record.StartCluster}");
			} else if (!(0 <= record.StartCluster && record.StartCluster < Header.TotalClusters))
				throw new CorruptDataException(Header, $"Record {index} pointed to to non-existent cluster {record.StartCluster}");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckClusterIndex(int index, string msg = null)
			=> Guard.Argument(index == -1 || (0 <= index && index < _clusters.Count), nameof(index), msg ?? "Cluster index out of bounds (or not -1)");

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckLinkedCluster(int sourceCluster, int linkedCluster, string msg = null) {
			Guard.Argument(sourceCluster >= 0, nameof(sourceCluster), "Must be greater than or equal to 0");

			if (linkedCluster == -1)
				return;

			if (sourceCluster == linkedCluster)
				throw new CorruptDataException(Header, sourceCluster, $"Cluster links to itself {sourceCluster}");

			if (!(0 <= linkedCluster && linkedCluster < Header.TotalClusters))
				throw new CorruptDataException(Header, sourceCluster, msg ?? $"Cluster {sourceCluster} pointed to non-existent cluster {linkedCluster}");
		}

		private void CheckClusterTraits(int cluster, ClusterTraits traits) {
			var bTraits = (byte)traits;
			if (bTraits == 0 || bTraits > 7 || ((traits & ClusterTraits.Data) > 0 && (traits & ClusterTraits.Record) > 0))
				throw new CorruptDataException(Header, cluster, "Invalid traits");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckNotOpened() {
			if (_openStream != null)
				throw new InvalidOperationException("A stream is already opened");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void MigrateTipClusterTo(int to) => MigrateTipClusterTo(_clusters.Count - 1, to);

		private void MigrateTipClusterTo(int tipIndex, int to) {
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
					var updatedRecord = _records[recordIndex];
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

		private void InvalidateCachedCluster(int cluster) {
			_recordsFragmentProvider.InvalidateCluster(cluster);
			if (_openFragmentProvider != null)
				_openFragmentProvider.InvalidateCluster(cluster);
		}

		#endregion

		#region Inner Types

		internal class FragmentProvider : IStreamFragmentProvider {
			private readonly ClusteredStorageBase<THeader, TRecord> _parent;
			private readonly FragmentCache _fragmentCache;
			private readonly ClusterDataType _clusterDataType;
			private int _currentFragment;
			private int _currentCluster;
			private readonly int _recordIndex;
			private TRecord _record;

			public FragmentProvider(ClusteredStorageBase<THeader, TRecord> parent, ClusteredStorageCachePolicy cachePolicy)
				: this(parent, ClusterDataType.Record, -1, cachePolicy) {
			}

			public FragmentProvider(ClusteredStorageBase<THeader, TRecord> parent, int recordIndex, ClusteredStorageCachePolicy cachePolicy)
				: this(parent, ClusterDataType.Stream, recordIndex, cachePolicy) {
			}

			private FragmentProvider(ClusteredStorageBase<THeader, TRecord> parent, ClusterDataType clusterDataType, int recordIndex, ClusteredStorageCachePolicy cachePolicy) {
				_parent = parent;
				_clusterDataType = clusterDataType;
				if (_clusterDataType == ClusterDataType.Stream)
					Guard.ArgumentInRange(recordIndex, 0, _parent.Header.RecordsCount, nameof(recordIndex));
				_recordIndex = recordIndex;
				_fragmentCache = new FragmentCache(cachePolicy);
				Reset();
			}

			public long TotalBytes => _clusterDataType switch {
				ClusterDataType.Record => _parent.AllRecordsSize,
				ClusterDataType.Stream => _record.Size,
				_ => throw new ArgumentOutOfRangeException()
			};

			public int FragmentCount => (int)Math.Ceiling(TotalBytes / (float)_parent.ClusterSize);

			public bool EnableCache => _fragmentCache.Policy != ClusteredStorageCachePolicy.None;

			public ReadOnlySpan<byte> GetFragment(int index) {
				Guard.ArgumentInRange(index, 0, FragmentCount - 1, nameof(index));
				TraverseToFragment(index);
				return _parent.GetCluster(_currentCluster).Data;
			}

			public bool TryMapStreamPosition(long position, out int fragmentIndex, out int fragmentPosition) {
				fragmentIndex = (int)(position / _parent.ClusterSize);
				fragmentPosition = (int)(position % _parent.ClusterSize);
				return true;
			}

			public void UpdateFragment(int fragmentIndex, int fragmentPosition, ReadOnlySpan<byte> updateSpan) {
				TraverseToFragment(fragmentIndex);
				var cluster = _parent.GetCluster(_currentCluster);
				updateSpan.CopyTo(cluster.Data.AsSpan(fragmentPosition));
				_parent.UpdateCluster(_currentCluster, cluster);
			}

			public bool TrySetTotalBytes(long length, out int[] newFragments, out int[] deletedFragments) {
				Tools.Debugger.CounterA++;
				var oldLength = TotalBytes;
				var newTotalClusters = (int)Math.Ceiling(length / (float)_parent.ClusterSize);
				var oldTotalClusters = FragmentCount;
				var currentTotalClusters = oldTotalClusters;
				var newFragmentsL = new List<int>();
				var newClustersL = new List<int>();
				var deletedFragmentsL = new List<int>();
				var deletedClustersL = new List<int>();
				TraverseToEnd();
				if (newTotalClusters > currentTotalClusters) {
					// Fast Implementation, adds clusters in range
					var newClusters = newTotalClusters - currentTotalClusters;
					var needsFirstCluster = currentTotalClusters == 0;
					_parent.AllocateNextClusters(_clusterDataType, _recordIndex, _currentCluster, newClusters, needsFirstCluster, out var newStartIX);
					
					for (var i = 0; i < newClusters; i++) {
						var newClusterIX = newStartIX + i;
						var newFragmentIX = _currentFragment + i + 1;
						newClustersL.Add(newClusterIX);
						newFragmentsL.Add(newFragmentIX);
						if (EnableCache)
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
							_currentCluster = steppedBackToTip ? deleteCluster : rememberCurrentCluster;  // this is because we deleted the tip cluster which moved back to where migrated cluster was
							_currentFragment = rememberCurrentFragment;
						}
						currentTotalClusters--;
						deletedFragmentsL.Add(deleteFragment);
						deletedClustersL.Add(deleteCluster);
					}
				}

				// Erase unused portion of tip cluster when shrinking stream
				if (length < oldLength) {
					var unusedTipClusterBytes = (int)(newTotalClusters * _parent.ClusterSize - length);
					if (unusedTipClusterBytes > 0)
						_parent.FastWriteClusterData(_currentCluster, _parent.ClusterSize - unusedTipClusterBytes, _parent.ZeroClusterBytes.Span.Slice(..unusedTipClusterBytes));
				}

				// Update record if applicable
				if (_clusterDataType == ClusterDataType.Stream) {
					_record.Size = (int)length;
					if (_record.Size == 0)
						_record.StartCluster = -1;
					if (oldTotalClusters == 0 && newTotalClusters > 0)
						_record.StartCluster = newClustersL[0];
					_parent.UpdateRecord(_recordIndex, _record, false);
				} else {
					_parent.AllRecordsSize = (int)length;
				}

				newFragments = newFragmentsL.ToArray();
				deletedFragments = deletedFragmentsL.ToArray();
				return true;
			}

			internal void Reset() {
				ResetClusterPointer();
				if (EnableCache)
					_fragmentCache.Clear();
			}

			internal void ResetClusterPointer() {
				switch (_clusterDataType) {
					case ClusterDataType.Record:
						_currentFragment = _parent.Header.RecordsCount > 0 && _parent.Header.TotalClusters > 0 ? 0 : -1;
						_currentCluster = _parent.Header.RecordsCount > 0 && _parent.Header.TotalClusters > 0 ? 0 : -1;
						break;
					case ClusterDataType.Stream:
						_record = _parent.GetRecord(_recordIndex);
						_currentFragment = _record.StartCluster > 0 ? 0 : -1;
						_currentCluster = _record.StartCluster > 0 ? _record.StartCluster : -1;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			internal void InvalidateCluster(int cluster) {
				if (cluster == _currentCluster /*|| (_record != null && _record.StartCluster == cluster)*/)
					ResetClusterPointer();
				if (EnableCache)
					_fragmentCache.InvalidateCluster(cluster);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private bool IsStartCluster(int cluster) => _clusterDataType switch {
				ClusterDataType.Record => cluster == 0,
				ClusterDataType.Stream => cluster == _record.StartCluster,
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

			private void TraverseToFragment(int index) {
				if (_currentFragment == index)
					return;

				if (EnableCache && _fragmentCache.TryGetCluster(index, out var cluster)) {
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
				var prevCluster = 0;
				if (!(EnableCache && _fragmentCache.TryGetCluster(_currentFragment, out prevCluster))) {
					_currentCluster = _parent.FastReadClusterPrev(_currentCluster);
					if (EnableCache)
						_fragmentCache.SetCluster(_currentFragment, _currentCluster);
				} else _currentCluster = prevCluster;

				return true;
			}

			private bool TryStepForward() {
				if (_currentFragment < 0)
					return false;

				if (!(EnableCache && _fragmentCache.TryGetCluster(_currentFragment + 1, out var nextCluster))) {
					nextCluster = _parent.FastReadClusterNext(_currentCluster);
				}

				if (nextCluster == _currentCluster)
					return false;

				if (nextCluster == -1)
					return false;

				_currentFragment++;
				_currentCluster = nextCluster;
				if (EnableCache)
					_fragmentCache.SetCluster(_currentFragment, _currentCluster);
				return true;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private void CheckSteps(int steps) {
				if (steps > FragmentCount)
					throw new CorruptDataException(_parent.Header, $"Unable to traverse the cluster-chain due to cyclic dependency (detected at cluster {_currentCluster})");
			}

		}

		internal class FragmentCache {
			private readonly IDictionary<int, int> _fragmentToClusterMap;
			private readonly IDictionary<int, int> _clusterToFragmentMap;

			public FragmentCache(ClusteredStorageCachePolicy policy) {
				if (policy == ClusteredStorageCachePolicy.Scan)
					throw new NotSupportedException(policy.ToString());

				_fragmentToClusterMap = new Dictionary<int, int>();
				_clusterToFragmentMap = new Dictionary<int, int>();
				Policy = policy;
			}

			public ClusteredStorageCachePolicy Policy { get; }

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool TryGetCluster(int fragment, out int cluster) {
				return _fragmentToClusterMap.TryGetValue(fragment, out cluster);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void SetCluster(int fragment, int cluster) {
				_fragmentToClusterMap[fragment] = cluster;
				_clusterToFragmentMap[cluster] = fragment;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void Invalidate(int fragment, int cluster) {
				_fragmentToClusterMap.Remove(fragment);
				_clusterToFragmentMap.Remove(cluster);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void InvalidateFragment(int fragment) {
				if (_fragmentToClusterMap.TryGetValue(fragment, out var cluster))
					Invalidate(fragment, cluster);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void InvalidateCluster(int cluster) {
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

}
