// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Threading;

namespace Hydrogen;

public class ClusteredStorageHeader : ISynchronizedObject {
	private readonly object _lock;
	public const int ByteLength = 256;
	public const int MerkleRootLength = 32;
	public const int MasterKeyLength = 32;

	internal const int VersionLength = sizeof(byte);
	internal const int PolicyLength = sizeof(uint);
	internal const int RecordsLength = sizeof(long);
	internal const int RecordsEndClusterLength = sizeof(long);
	internal const int RecordKeySizeLength = sizeof(ushort);
	internal const int ReservedRecordsLength = sizeof(long);
	internal const int ClusterSizeLength = sizeof(int);
	internal const int TotalClustersLength = sizeof(long);

	internal const int VersionOffset = 0;
	internal const int PolicyOffset = VersionOffset + VersionLength;
	internal const int RecordsOffset = PolicyOffset + PolicyLength;
	internal const int RecordsEndClusterOffset = RecordsOffset + RecordsLength;
	internal const int RecordKeySizeOffset = RecordsEndClusterOffset + RecordsEndClusterLength;
	internal const int ReservedRecordsOffset = RecordKeySizeOffset + RecordKeySizeLength;
	internal const int ClusterSizeOffset = ReservedRecordsOffset + ReservedRecordsLength;
	internal const int TotalClustersOffset = ClusterSizeOffset + ClusterSizeLength;
	internal const int MerkleRootOffset = TotalClustersOffset + TotalClustersLength;
	internal const int MasterKeyOffset = MerkleRootOffset + MerkleRootLength;

	private Stream _headerStream;
	private EndianBinaryReader _reader;
	private EndianBinaryWriter _writer;

	private byte? _version;
	private ClusteredStoragePolicy? _policy;
	private long? _records;
	private long? _recordsEndCluster;
	private ushort? _recordKeySize;
	private long? _reservedRecords;
	private int? _clusterSize;
	private long? _totalClusters;
	private readonly ISynchronizedObject _syncLock;
	private byte[] _merkleRoot;
	private byte[] _masterKey;

	internal ClusteredStorageHeader(ISynchronizedObject syncLock) {
		Guard.ArgumentNotNull(syncLock, nameof(syncLock));
		_syncLock = syncLock;
	}

	public ISynchronizedObject ParentSyncObject { get => _syncLock.ParentSyncObject; set => _syncLock.ParentSyncObject = value; }

	public ReaderWriterLockSlim ThreadLock => _syncLock.ThreadLock;

	public byte Version {
		get {
			using (EnterReadScope()) {
				if (!_version.HasValue) {
					using var _ = _headerStream.EnterRestorePositionSeek(VersionOffset, SeekOrigin.Begin);
					_version = _reader.ReadByte();
				}
				return _version.Value;
			}
		}
		internal set {
			Guard.Argument(value == 1, nameof(value), "Only version 1 supported");
			using (EnterWriteScope()) {
				if (_version == value)
					return;
				_version = value;
				using var _ = _headerStream.EnterRestorePositionSeek(VersionOffset, SeekOrigin.Begin);
				_writer.Write(_version.Value);
			}
		}
	}

	public ClusteredStoragePolicy Policy {
		get {
			using (EnterReadScope()) {
				if (!_policy.HasValue) {
					using var _ = _headerStream.EnterRestorePositionSeek(PolicyOffset, SeekOrigin.Begin);
					_policy = (ClusteredStoragePolicy)_reader.ReadInt32();
				}
				return _policy.Value;
			}
		}
		internal set {
			using (EnterWriteScope()) {
				if (_policy == value)
					return;
				_policy = value;
				using var _ = _headerStream.EnterRestorePositionSeek(PolicyOffset, SeekOrigin.Begin);
				_writer.Write((int)_policy.Value);
			}
		}
	}

	public long RecordsCount {
		get {
			using (EnterReadScope()) {
				if (!_records.HasValue) {
					using var _ = _headerStream.EnterRestorePositionSeek(RecordsOffset, SeekOrigin.Begin);
					_records = _reader.ReadInt64();
				}
				return _records.Value;
			}
		}
		internal set {
			using (EnterWriteScope()) {
				if (_records == value)
					return;
				_records = value;
				using var _ = _headerStream.EnterRestorePositionSeek(RecordsOffset, SeekOrigin.Begin);
				_writer.Write(_records.Value);
			}
		}
	}

	public long RecordsEndCluster {
		get {
			using (EnterReadScope()) {
				if (!_recordsEndCluster.HasValue) {
					using var _ = _headerStream.EnterRestorePositionSeek(RecordsEndClusterOffset, SeekOrigin.Begin);
					_recordsEndCluster = _reader.ReadInt64();
				}
				return _recordsEndCluster.Value;
			}
		}
		internal set {
			using (EnterWriteScope()) {
				if (_recordsEndCluster == value)
					return;
				_recordsEndCluster = value;
				using var _ = _headerStream.EnterRestorePositionSeek(RecordsEndClusterOffset, SeekOrigin.Begin);
				_writer.Write(_recordsEndCluster.Value);
			}
		}
	}

	public ushort RecordKeySize {
		get {
			using (EnterReadScope()) {
				if (!_recordKeySize.HasValue) {
					using var _ = _headerStream.EnterRestorePositionSeek(RecordKeySizeOffset, SeekOrigin.Begin);
					_recordKeySize = _reader.ReadUInt16();
				}
				return _recordKeySize.Value;
			}
		}
		internal set {
			using (EnterWriteScope()) {
				if (_recordKeySize == value)
					return;
				_recordKeySize = value;
				using var _ = _headerStream.EnterRestorePositionSeek(RecordKeySizeOffset, SeekOrigin.Begin);
				_writer.Write((ushort)_recordKeySize.Value);
			}
		}
	}

	public long ReservedRecords {
		get {
			using (EnterReadScope()) {
				if (!_reservedRecords.HasValue) {
					using var _ = _headerStream.EnterRestorePositionSeek(ReservedRecordsOffset, SeekOrigin.Begin);
					_reservedRecords = _reader.ReadInt64();
				}
				return _reservedRecords.Value;
			}
		}
		internal set {
			using (EnterWriteScope()) {

				if (_reservedRecords == value)
					return;
				if (value == 0 && Policy.HasFlag(ClusteredStoragePolicy.TrackKey))
					throw new InvalidOperationException($"Cannot set {nameof(ReservedRecords)} to 0 as {nameof(Policy)} has {ClusteredStoragePolicy.TrackKey} enabled");

				if (RecordsCount > _reservedRecords.GetValueOrDefault(0))
					throw new InvalidOperationException($"Cannot set {nameof(ReservedRecords)} to {value} as records already exist with value");

				_reservedRecords = value;
				using var _ = _headerStream.EnterRestorePositionSeek(ReservedRecordsOffset, SeekOrigin.Begin);
				_writer.Write(_reservedRecords.Value);
			}
		}
	}

	// TODO: obviously too large, make it a unshort maybe? 65k clusters, or int?
	public int ClusterSize {
		get {
			using (EnterReadScope()) {
				if (!_clusterSize.HasValue) {
					using var _ = _headerStream.EnterRestorePositionSeek(ClusterSizeOffset, SeekOrigin.Begin);
					_clusterSize = _reader.ReadInt32();
				}
				return _clusterSize.Value;
			}
		}
		internal set {
			using (EnterWriteScope()) {
				if (_clusterSize == value)
					return;
				_clusterSize = value;
				using var _ = _headerStream.EnterRestorePositionSeek(ClusterSizeOffset, SeekOrigin.Begin);
				_writer.Write(_clusterSize.Value);
			}
		}
	}

	public long TotalClusters {
		get {
			using (EnterReadScope()) {
				if (!_totalClusters.HasValue) {
					using var _ = _headerStream.EnterRestorePositionSeek(TotalClustersOffset, SeekOrigin.Begin);
					_totalClusters = _reader.ReadInt64();
				}
				return _totalClusters.Value;
			}
		}
		internal set {
			using (EnterWriteScope()) {
				if (_totalClusters == value)
					return;
				_totalClusters = value;
				using var _ = _headerStream.EnterRestorePositionSeek(TotalClustersOffset, SeekOrigin.Begin);
				_writer.Write(_totalClusters.Value);
			}
		}
	}

	public byte[] MerkleRoot {
		get {
			using (EnterReadScope()) {
				if (_merkleRoot == null) {
					using var _ = _headerStream.EnterRestorePositionSeek(MerkleRootOffset, SeekOrigin.Begin);
					_merkleRoot = _reader.ReadBytes(MerkleRootLength);
				}
				return _merkleRoot;
			}
		}
		internal set {
			using (EnterWriteScope()) {
				if (ByteArrayEqualityComparer.Instance.Equals(_merkleRoot, value))
					return;
				_merkleRoot = value;
				using var _ = _headerStream.EnterRestorePositionSeek(MerkleRootOffset, SeekOrigin.Begin);
				_writer.Write(_merkleRoot);
			}
		}
	}

	public byte[] MasterKey {
		get {
			using (EnterReadScope()) {
				if (_masterKey == null) {
					using var _ = _headerStream.EnterRestorePositionSeek(MasterKeyOffset, SeekOrigin.Begin);
					_masterKey = _reader.ReadBytes(MasterKeyLength);
				}
				return _masterKey;
			}
		}
		internal set {
			using (EnterWriteScope()) {
				if (ByteArrayEqualityComparer.Instance.Equals(_masterKey, value))
					return;
				_masterKey = value;
				using var _ = _headerStream.EnterRestorePositionSeek(MasterKeyOffset, SeekOrigin.Begin);
				_writer.Write(_masterKey);
			}
		}
	}

	public IDisposable EnterReadScope() => _syncLock.EnterReadScope();

	public IDisposable EnterWriteScope() => _syncLock.EnterWriteScope();

	public void AttachTo(Stream rootStream, Endianness endianness) {
		Guard.ArgumentNotNull(rootStream, nameof(rootStream));
		Guard.Argument(rootStream.Length >= ByteLength, nameof(rootStream), "Missing header");
		using (EnterWriteScope()) {
			_headerStream = new BoundedStream(rootStream, 0, 255);
			_reader = new EndianBinaryReader(EndianBitConverter.For(endianness), _headerStream);
			_writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), _headerStream);
			_version = null;
			_policy = null;
			_recordsEndCluster = null;
			_recordKeySize = null;
			_reservedRecords = null;
			_clusterSize = null;
			_totalClusters = null;
			_records = null;
			_merkleRoot = null;
			_masterKey = null;
		}
	}

	public void CreateIn(byte version, Stream rootStream, int clusterSize, long recordKeySize, long reservedRecords, Endianness endianness) {
		Guard.ArgumentNotNull(rootStream, nameof(rootStream));
		Guard.Argument(rootStream.Length == 0, nameof(rootStream), "Must be empty");
		Guard.ArgumentGTE(clusterSize, 1, nameof(clusterSize));
		Guard.ArgumentInRange(recordKeySize, 0, ushort.MaxValue, nameof(recordKeySize));
		Guard.ArgumentGTE(reservedRecords, 0, nameof(reservedRecords));
		using (EnterWriteScope()) {

			rootStream.Seek(0, SeekOrigin.Begin);
			var writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), rootStream);
			Guard.Ensure(rootStream.Position == VersionOffset);
			writer.Write((byte)version); // Version

			Guard.Ensure(rootStream.Position == PolicyOffset);
			writer.Write((uint)0); // Policy

			Guard.Ensure(rootStream.Position == RecordsOffset);
			writer.Write((long)0L); // Records

			Guard.Ensure(rootStream.Position == RecordsEndClusterOffset);
			writer.Write((long)-1L); // RecordsEndCluster

			Guard.Ensure(rootStream.Position == RecordKeySizeOffset);
			writer.Write((ushort)recordKeySize); // RecordKeySize

			Guard.Ensure(rootStream.Position == ReservedRecordsOffset);
			writer.Write((long)reservedRecords); // ReservedRecords

			Guard.Ensure(rootStream.Position == ClusterSizeOffset);
			writer.Write((int)clusterSize); // ClusterSize

			Guard.Ensure(rootStream.Position == TotalClustersOffset);
			writer.Write((long)0L); // TotalClusters 

			Guard.Ensure(rootStream.Position == MerkleRootOffset);
			writer.Write(new byte[MerkleRootLength]); // MerkleRoot 

			Guard.Ensure(rootStream.Position == MasterKeyOffset);
			writer.Write(new byte[MasterKeyLength]); // MasterKey

			writer.Write(new byte[ByteLength - rootStream.Position]); // header padding
			Guard.Ensure(rootStream.Position == ByteLength);
			AttachTo(rootStream, endianness);
		}
	}

	public void CheckHeaderIntegrity() {
		using (EnterReadScope()) {
			if (Version != 1)
				throw new CorruptDataException($"Corrupt header property {nameof(Version)} value was {Version} bytes");

			if (ClusterSize <= 0)
				throw new CorruptDataException($"Corrupt header property {nameof(ClusterSize)} value was {ClusterSize} bytes");

			if (TotalClusters < 0)
				throw new CorruptDataException($"Corrupt header property {nameof(TotalClusters)} value was {TotalClusters}");

			if (ClusterSize < 0)
				throw new CorruptDataException($"Corrupt header property {nameof(ClusterSize)} value was {ClusterSize}");

			if (ReservedRecords < 0)
				throw new CorruptDataException($"Corrupt header property {nameof(ReservedRecords)} value was {ReservedRecords}");

			if (RecordsCount < 0)
				throw new CorruptDataException($"Corrupt header property {nameof(RecordsCount)} value was {RecordsCount}");

			if (RecordsEndCluster < -1)
				throw new CorruptDataException($"Corrupt header property {nameof(RecordsEndCluster)} value was {RecordsEndCluster}");

			if (Policy.HasFlag(ClusteredStoragePolicy.TrackKey) && RecordKeySize <= 0)
				throw new CorruptDataException($"Corrupt header property {nameof(RecordKeySize)} value was {RecordKeySize} but {nameof(Policy)} property value was {RecordKeySize}");
		}
	}

	public void ResetMerkleRoot() => MerkleRoot = new byte[MerkleRootLength];

	public override string ToString() {
		using (EnterReadScope())
			return $"[{nameof(ClusteredStorageHeader)}] {nameof(Version)}: {Version}, {nameof(ClusterSize)}: {ClusterSize}, {nameof(TotalClusters)}: {TotalClusters}, {nameof(RecordsCount)}: {RecordsCount}, {nameof(RecordsEndCluster)}: {RecordsEndCluster}, {nameof(ReservedRecords)}: {ReservedRecords}, {nameof(Policy)}: {Policy}, {nameof(MerkleRoot)}: {MerkleRoot.ToHexString(true)}";
	}


}
