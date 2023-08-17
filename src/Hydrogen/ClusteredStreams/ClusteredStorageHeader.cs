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

public class ClusteredStorageHeader {

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

	private readonly Stream _headerStream;
	private readonly EndianBinaryReader _reader;
	private readonly EndianBinaryWriter _writer;

	private byte? _version;
	private ClusteredStoragePolicy? _policy;
	private long? _records;
	private long? _recordsEndCluster;
	private ushort? _recordKeySize;
	private long? _reservedRecords;
	private int? _clusterSize;
	private long? _totalClusters;
	private readonly ICriticalObject _lock;
	private byte[] _merkleRoot;
	private byte[] _masterKey;

	internal ClusteredStorageHeader(ConcurrentStream rootStream, Endianness endianness) {
		Guard.ArgumentNotNull(rootStream, nameof(rootStream));
		_headerStream = rootStream.AsBounded(0, 255); 
		_lock = rootStream;
		_reader = new EndianBinaryReader(EndianBitConverter.For(endianness), _headerStream);
		_writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), _headerStream);
	}


	public byte Version {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (!_version.HasValue) {
				using var _ = _headerStream.EnterRestorePositionSeek(VersionOffset, SeekOrigin.Begin);
				_version = _reader.ReadByte();
			}
			return _version.Value;

		}
		internal set {
			Guard.Argument(value == 1, nameof(value), "Only version 1 supported");
			using var accessScope = _lock.EnterAccessScope();
			if (_version == value)
				return;
			_version = value;
			using var _ = _headerStream.EnterRestorePositionSeek(VersionOffset, SeekOrigin.Begin);
			_writer.Write(_version.Value);

		}
	}

	public ClusteredStoragePolicy Policy {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (!_policy.HasValue) {
				using var _ = _headerStream.EnterRestorePositionSeek(PolicyOffset, SeekOrigin.Begin);
				_policy = (ClusteredStoragePolicy)_reader.ReadInt32();
			}
			return _policy.Value;

		}
		internal set {
			using var accessScope = _lock.EnterAccessScope();
			if (_policy == value)
				return;
			_policy = value;
			using var _ = _headerStream.EnterRestorePositionSeek(PolicyOffset, SeekOrigin.Begin);
			_writer.Write((int)_policy.Value);

		}
	}

	public long RecordsCount {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (!_records.HasValue) {
				using var _ = _headerStream.EnterRestorePositionSeek(RecordsOffset, SeekOrigin.Begin);
				_records = _reader.ReadInt64();
			}
			return _records.Value;

		}
		internal set {
			using var accessScope = _lock.EnterAccessScope();
			if (_records == value)
				return;
			_records = value;
			using var _ = _headerStream.EnterRestorePositionSeek(RecordsOffset, SeekOrigin.Begin);
			_writer.Write(_records.Value);

		}
	}

	public long RecordsEndCluster {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (!_recordsEndCluster.HasValue) {
				using var _ = _headerStream.EnterRestorePositionSeek(RecordsEndClusterOffset, SeekOrigin.Begin);
				_recordsEndCluster = _reader.ReadInt64();
			}
			return _recordsEndCluster.Value;
		}
		internal set {
			using var accessScope = _lock.EnterAccessScope();
			if (_recordsEndCluster == value)
				return;
			_recordsEndCluster = value;
			using var _ = _headerStream.EnterRestorePositionSeek(RecordsEndClusterOffset, SeekOrigin.Begin);
			_writer.Write(_recordsEndCluster.Value);
		}
	}

	public ushort RecordKeySize {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (!_recordKeySize.HasValue) {
				using var _ = _headerStream.EnterRestorePositionSeek(RecordKeySizeOffset, SeekOrigin.Begin);
				_recordKeySize = _reader.ReadUInt16();
			}
			return _recordKeySize.Value;
		}
		internal set {
			using var accessScope = _lock.EnterAccessScope();
			if (_recordKeySize == value)
				return;
			_recordKeySize = value;
			using var _ = _headerStream.EnterRestorePositionSeek(RecordKeySizeOffset, SeekOrigin.Begin);
			_writer.Write((ushort)_recordKeySize.Value);

		}
	}

	public long ReservedRecords {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (!_reservedRecords.HasValue) {
				using var _ = _headerStream.EnterRestorePositionSeek(ReservedRecordsOffset, SeekOrigin.Begin);
				_reservedRecords = _reader.ReadInt64();
			}
			return _reservedRecords.Value;
		}
		internal set {
			using var accessScope = _lock.EnterAccessScope();

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

	// TODO: obviously too large, make it a unshort maybe? 65k clusters, or int?
	public int ClusterSize {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (!_clusterSize.HasValue) {
				using var _ = _headerStream.EnterRestorePositionSeek(ClusterSizeOffset, SeekOrigin.Begin);
				_clusterSize = _reader.ReadInt32();
			}
			return _clusterSize.Value;
		}
		internal set {
			using var accessScope = _lock.EnterAccessScope();
			if (_clusterSize == value)
				return;
			_clusterSize = value;
			using var _ = _headerStream.EnterRestorePositionSeek(ClusterSizeOffset, SeekOrigin.Begin);
			_writer.Write(_clusterSize.Value);
		}
	}

	public long TotalClusters {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (!_totalClusters.HasValue) {
				using var _ = _headerStream.EnterRestorePositionSeek(TotalClustersOffset, SeekOrigin.Begin);
				_totalClusters = _reader.ReadInt64();
			}
			return _totalClusters.Value;
		}
		internal set {
			using var accessScope = _lock.EnterAccessScope();
			if (_totalClusters == value)
				return;
			_totalClusters = value;
			using var _ = _headerStream.EnterRestorePositionSeek(TotalClustersOffset, SeekOrigin.Begin);
			_writer.Write(_totalClusters.Value);
		}
	}

	public byte[] MerkleRoot {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (_merkleRoot == null) {
				using var _ = _headerStream.EnterRestorePositionSeek(MerkleRootOffset, SeekOrigin.Begin);
				_merkleRoot = _reader.ReadBytes(MerkleRootLength);
			}
			return _merkleRoot;
		}
		internal set {
			using var accessScope = _lock.EnterAccessScope();
			if (ByteArrayEqualityComparer.Instance.Equals(_merkleRoot, value))
				return;
			_merkleRoot = value;
			using var _ = _headerStream.EnterRestorePositionSeek(MerkleRootOffset, SeekOrigin.Begin);
			_writer.Write(_merkleRoot);
		}
	}

	public byte[] MasterKey {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (_masterKey == null) {
				using var _ = _headerStream.EnterRestorePositionSeek(MasterKeyOffset, SeekOrigin.Begin);
				_masterKey = _reader.ReadBytes(MasterKeyLength);
			}
			return _masterKey;

		}
		internal set {
			using var accessScope = _lock.EnterAccessScope();
			if (ByteArrayEqualityComparer.Instance.Equals(_masterKey, value))
				return;
			_masterKey = value;
			using var _ = _headerStream.EnterRestorePositionSeek(MasterKeyOffset, SeekOrigin.Begin);
			_writer.Write(_masterKey);

		}
	}


	public void Load() {
		using var accessScope = _lock.EnterAccessScope();
		Guard.Ensure(_headerStream.Length >= ByteLength, "Missing or corrupt header");
		// Below are lazily loaded
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

	public void Create(byte version, int clusterSize, long recordKeySize, long reservedRecords) {
		Guard.ArgumentGTE(clusterSize, 1, nameof(clusterSize));
		Guard.ArgumentInRange(recordKeySize, 0, ushort.MaxValue, nameof(recordKeySize));
		Guard.ArgumentGTE(reservedRecords, 0, nameof(reservedRecords));
		
		using var accessScope = _lock.EnterAccessScope();
		Guard.Ensure(_headerStream.Length == 0, "Header must be empty");

		_headerStream.Seek(0, SeekOrigin.Begin);
		
		Guard.Ensure(_headerStream.Position == VersionOffset);
		_writer.Write((byte)version); // Version

		Guard.Ensure(_headerStream.Position == PolicyOffset);
		_writer.Write((uint)0); // Policy

		Guard.Ensure(_headerStream.Position == RecordsOffset);
		_writer.Write((long)0L); // Records

		Guard.Ensure(_headerStream.Position == RecordsEndClusterOffset);
		_writer.Write((long)Cluster.Null); // RecordsEndCluster

		Guard.Ensure(_headerStream.Position == RecordKeySizeOffset);
		_writer.Write((ushort)recordKeySize); // RecordKeySize

		Guard.Ensure(_headerStream.Position == ReservedRecordsOffset);
		_writer.Write((long)reservedRecords); // ReservedRecords

		Guard.Ensure(_headerStream.Position == ClusterSizeOffset);
		_writer.Write((int)clusterSize); // ClusterSize

		Guard.Ensure(_headerStream.Position == TotalClustersOffset);
		_writer.Write((long)0L); // TotalClusters 

		Guard.Ensure(_headerStream.Position == MerkleRootOffset);
		_writer.Write(new byte[MerkleRootLength]); // MerkleRoot 

		Guard.Ensure(_headerStream.Position == MasterKeyOffset);
		_writer.Write(new byte[MasterKeyLength]); // MasterKey

		_writer.Write(new byte[ByteLength - _headerStream.Position]); // header padding
		Guard.Ensure(_headerStream.Position == ByteLength);
		Load();
	}

	public void CheckHeaderIntegrity() {
		using var accessScope = _lock.EnterAccessScope();
		Guard.Ensure(Version == 1, $"Corrupt header property {nameof(Version)} value was {Version} bytes");
		Guard.Ensure(ClusterSize > 0, $"Corrupt header property {nameof(ClusterSize)} value was {ClusterSize} bytes");
		Guard.Ensure(TotalClusters >= 0, $"Corrupt header property {nameof(TotalClusters)} value was {TotalClusters} bytes");
		Guard.Ensure(ReservedRecords >= 0, $"Corrupt header property {nameof(ReservedRecords)} value was {ReservedRecords} bytes");
		Guard.Ensure(RecordsCount >= 0, $"Corrupt header property {nameof(RecordsCount)} value was {RecordsCount} bytes");
		Guard.Ensure(RecordsEndCluster >= Cluster.Null, $"Corrupt header property {nameof(RecordsEndCluster)} value was {RecordsEndCluster} bytes");
		Guard.Against(Policy.HasFlag(ClusteredStoragePolicy.TrackKey) && RecordKeySize <= 0, $"Corrupt header property {nameof(RecordKeySize)} value was {RecordKeySize} but {nameof(Policy)} property value was {RecordKeySize}");
	}

	public void ResetMerkleRoot() => MerkleRoot = new byte[MerkleRootLength];

	public override string ToString() {
		using var accessScope = _lock.EnterAccessScope();
		return $"[{nameof(ClusteredStorageHeader)}] {nameof(Version)}: {Version}, {nameof(ClusterSize)}: {ClusterSize}, {nameof(TotalClusters)}: {TotalClusters}, {nameof(RecordsCount)}: {RecordsCount}, {nameof(RecordsEndCluster)}: {RecordsEndCluster}, {nameof(ReservedRecords)}: {ReservedRecords}, {nameof(Policy)}: {Policy}, {nameof(MerkleRoot)}: {MerkleRoot.ToHexString(true)}";
	}



}
