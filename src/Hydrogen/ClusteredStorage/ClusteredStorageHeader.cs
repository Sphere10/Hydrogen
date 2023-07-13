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
	private readonly object _lock;
	public const int ByteLength = 256;
	public const int MerkleRootLength = 32;
	public const int MasterKeyLength = 32;

	internal const int VersionOffset = 0;
	internal const int PolicyOffset = VersionOffset + sizeof(byte);
	internal const int RecordsOffset = PolicyOffset + sizeof(uint);
	internal const int RecordKeySizeOffset = RecordsOffset + sizeof(int);
	internal const int ReservedRecordsOffset = RecordKeySizeOffset + sizeof(ushort);
	internal const int ClusterSizeOffset = ReservedRecordsOffset + sizeof(int);
	internal const int TotalClustersOffset = ClusterSizeOffset + sizeof(int);
	internal const int MerkleRootOffset = TotalClustersOffset + sizeof(int);
	internal const int MasterKeyOffset = MerkleRootOffset + MerkleRootLength;

	private Stream _headerStream;
	private EndianBinaryReader _reader;
	private EndianBinaryWriter _writer;

	private byte? _version;
	private ClusteredStoragePolicy? _policy;
	private int? _records;
	private ushort? _recordKeySize;
	private int? _reservedRecords;
	private int? _clusterSize;
	private int? _totalClusters;

	private byte[] _merkleRoot;
	private byte[] _masterKey;

	internal ClusteredStorageHeader(object _lock) {
		this._lock = _lock;

	}

	public byte Version {
		get {
			if (!_version.HasValue) {
				_headerStream.Seek(VersionOffset, SeekOrigin.Begin);
				_version = _reader.ReadByte();
			}
			return _version.Value;
		}
		internal set {
			Guard.Argument(value == 1, nameof(value), "Only version 1 supported");
			Guard.Ensure(Monitor.IsEntered(_lock), "Clustered storage must be locked when updating header");
			if (_version == value)
				return;
			_version = value;
			_headerStream.Seek(VersionOffset, SeekOrigin.Begin);
			_writer.Write(_version.Value);
		}
	}

	public ClusteredStoragePolicy Policy {
		get {
			if (!_policy.HasValue) {
				_headerStream.Seek(PolicyOffset, SeekOrigin.Begin);
				_policy = (ClusteredStoragePolicy)_reader.ReadInt32();
			}
			return _policy.Value;
		}
		internal set {
			Guard.Ensure(Monitor.IsEntered(_lock), "Clustered storage must be locked when updating header");
			if (_policy == value)
				return;
			_policy = value;
			_headerStream.Seek(PolicyOffset, SeekOrigin.Begin);
			_writer.Write((int)_policy.Value);
		}
	}

	public int RecordsCount {
		get {
			if (!_records.HasValue) {
				_headerStream.Seek(RecordsOffset, SeekOrigin.Begin);
				_records = _reader.ReadInt32();
			}
			return _records.Value;
		}
		internal set {
			Guard.Ensure(Monitor.IsEntered(_lock), "Clustered storage must be locked when updating header");
			if (_records == value)
				return;
			_records = value;
			_headerStream.Seek(RecordsOffset, SeekOrigin.Begin);
			_writer.Write(_records.Value);
		}
	}

	public ushort RecordKeySize {
		get {
			if (!_recordKeySize.HasValue) {
				_headerStream.Seek(RecordKeySizeOffset, SeekOrigin.Begin);
				_recordKeySize = _reader.ReadUInt16();
			}
			return _recordKeySize.Value;
		}
		internal set {
			Guard.Ensure(Monitor.IsEntered(_lock), "Clustered storage must be locked when updating header");
			if (_recordKeySize == value)
				return;
			_recordKeySize = value;
			_headerStream.Seek(RecordKeySizeOffset, SeekOrigin.Begin);
			_writer.Write((ushort)_recordKeySize.Value);
		}
	}

	public int ReservedRecords {
		get {
			if (!_reservedRecords.HasValue) {
				_headerStream.Seek(ReservedRecordsOffset, SeekOrigin.Begin);
				_reservedRecords = _reader.ReadUInt16();
			}
			return _reservedRecords.Value;
		}
		internal set {
			Guard.Ensure(Monitor.IsEntered(_lock), "Clustered storage must be locked when updating header");

			if (_reservedRecords == value)
				return;
			if (value == 0 && Policy.HasFlag(ClusteredStoragePolicy.TrackKey))
				throw new InvalidOperationException($"Cannot set {nameof(ReservedRecords)} to 0 as {nameof(Policy)} has {ClusteredStoragePolicy.TrackKey} enabled");

			if (RecordsCount > _reservedRecords.GetValueOrDefault(0))
				throw new InvalidOperationException($"Cannot set {nameof(ReservedRecords)} to {value} as records already exist with value");

			_reservedRecords = value;
			_headerStream.Seek(ReservedRecordsOffset, SeekOrigin.Begin);
			_writer.Write(_reservedRecords.Value);
		}
	}

	public int ClusterSize {
		get {
			if (!_clusterSize.HasValue) {
				_headerStream.Seek(ClusterSizeOffset, SeekOrigin.Begin);
				_clusterSize = _reader.ReadInt32();
			}
			return _clusterSize.Value;
		}
		internal set {
			Guard.Ensure(Monitor.IsEntered(_lock), "Clustered storage must be locked when updating header");
			if (_clusterSize == value)
				return;
			_clusterSize = value;
			_headerStream.Seek(ClusterSizeOffset, SeekOrigin.Begin);
			_writer.Write(_clusterSize.Value);
		}
	}

	public int TotalClusters {
		get {
			if (!_totalClusters.HasValue) {
				_headerStream.Seek(TotalClustersOffset, SeekOrigin.Begin);
				_totalClusters = _reader.ReadInt32();
			}
			return _totalClusters.Value;
		}
		internal set {
			Guard.Ensure(Monitor.IsEntered(_lock), "Clustered storage must be locked when updating header");
			if (_totalClusters == value)
				return;
			_totalClusters = value;
			_headerStream.Seek(TotalClustersOffset, SeekOrigin.Begin);
			_writer.Write(_totalClusters.Value);
		}
	}

	public byte[] MerkleRoot {
		get {
			if (_merkleRoot == null) {
				_headerStream.Seek(MerkleRootOffset, SeekOrigin.Begin);
				_merkleRoot = _reader.ReadBytes(MerkleRootLength);
			}
			return _merkleRoot;
		}
		internal set {
			Guard.Ensure(Monitor.IsEntered(_lock), "Clustered storage must be locked when updating header");
			if (ByteArrayEqualityComparer.Instance.Equals(_merkleRoot, value))
				return;
			_merkleRoot = value;
			_headerStream.Seek(MerkleRootOffset, SeekOrigin.Begin);
			_writer.Write(_merkleRoot);
		}
	}

	public byte[] MasterKey {
		get {
			if (_masterKey == null) {
				_headerStream.Seek(MasterKeyOffset, SeekOrigin.Begin);
				_masterKey = _reader.ReadBytes(MasterKeyLength);
			}
			return _masterKey;
		}
		internal set {
			Guard.Ensure(Monitor.IsEntered(_lock), "Clustered storage must be locked when updating header");
			if (ByteArrayEqualityComparer.Instance.Equals(_masterKey, value))
				return;
			_masterKey = value;
			_headerStream.Seek(MasterKeyOffset, SeekOrigin.Begin);
			_writer.Write(_masterKey);
		}
	}

	public void AttachTo(Stream rootStream, Endianness endianness) {
		Guard.ArgumentNotNull(rootStream, nameof(rootStream));
		Guard.Argument(rootStream.Length >= ByteLength, nameof(rootStream), "Missing header");
		_headerStream = new BoundedStream(rootStream, 0, 255);
		_reader = new EndianBinaryReader(EndianBitConverter.For(endianness), _headerStream);
		_writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), _headerStream);
		_version = null;
		_policy = null;
		_recordKeySize = null;
		_reservedRecords = null;
		_clusterSize = null;
		_totalClusters = null;
		_records = null;
		_merkleRoot = null;
		_masterKey = null;
	}

	public void CreateIn(byte version, Stream rootStream, int clusterSize, int recordKeySize, int reservedRecords, Endianness endianness) {
		Guard.ArgumentNotNull(rootStream, nameof(rootStream));
		Guard.Argument(rootStream.Length == 0, nameof(rootStream), "Must be empty");
		Guard.ArgumentGTE(clusterSize, 1, nameof(clusterSize));
		Guard.ArgumentInRange(recordKeySize, 0, ushort.MaxValue, nameof(recordKeySize));
		Guard.ArgumentGTE(reservedRecords, 0, nameof(reservedRecords));
		rootStream.Seek(0, SeekOrigin.Begin);
		var writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), rootStream);
		writer.Write(version); // Version
		writer.Write((int)0); // Policy
		writer.Write((int)0); // Records
		writer.Write((ushort)recordKeySize); // RecordKeySize
		writer.Write((int)reservedRecords); // ReservedRecords
		writer.Write(clusterSize); // ClusterSize
		writer.Write((int)0); // TotalClusters 
		writer.Write(new byte[MerkleRootLength]); // MerkleRoot 
		writer.Write(new byte[MasterKeyLength]); // MasterKey

		writer.Write(Tools.Array.Gen<byte>(ByteLength - (int)rootStream.Position, 0)); // header padding
		Guard.Ensure(rootStream.Position == ByteLength);
		AttachTo(rootStream, endianness);
	}

	public void CheckHeaderIntegrity() {
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

		if (ReservedRecords < 0)
			throw new CorruptDataException($"Corrupt header property {nameof(ReservedRecords)} value was {ReservedRecords}");

		if (RecordsCount < 0)
			throw new CorruptDataException($"Corrupt header property {nameof(RecordsCount)} value was {RecordsCount}");

		if (Policy.HasFlag(ClusteredStoragePolicy.TrackKey) && RecordKeySize <= 0)
			throw new CorruptDataException($"Corrupt header property {nameof(RecordKeySize)} value was {RecordKeySize} but {nameof(Policy)} property value was {RecordKeySize}");

	}

	public override string ToString() =>
		$"[{nameof(ClusteredStorageHeader)}] {nameof(Version)}: {Version}, {nameof(ClusterSize)}: {ClusterSize}, {nameof(TotalClusters)}: {TotalClusters}, {nameof(RecordsCount)}: {RecordsCount}, {nameof(Policy)}: {Policy},  {nameof(MerkleRoot)}: {MerkleRoot.ToHexString(true)}";

}
