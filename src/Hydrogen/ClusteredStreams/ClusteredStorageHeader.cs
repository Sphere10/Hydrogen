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

public class StreamContainerHeader {

	public const int ByteLength = 256;
	public const int MerkleRootLength = 32;
	public const int MasterKeyLength = 32;

	internal const int VersionLength = sizeof(byte);
	internal const int PolicyLength = sizeof(uint);
	internal const int StreamCountLength = sizeof(long);
	internal const int StreamDescriptorsEndClusterLength = sizeof(long);
	internal const int StreamDescriptorKeySizeLength = sizeof(ushort);
	internal const int ReservedStreamsLength = sizeof(long);
	internal const int ClusterSizeLength = sizeof(int);
	internal const int TotalClustersLength = sizeof(long);

	internal const int VersionOffset = 0;
	internal const int PolicyOffset = VersionOffset + VersionLength;
	internal const int StreamCountOffset = PolicyOffset + PolicyLength;
	internal const int StreamDescriptorsEndClusterOffset = StreamCountOffset + StreamCountLength;
	internal const int StreamDescriptorKeySizeOffset = StreamDescriptorsEndClusterOffset + StreamDescriptorsEndClusterLength;
	internal const int StreamDescriptorRecordsOffset = StreamDescriptorKeySizeOffset + StreamDescriptorKeySizeLength;
	internal const int ClusterSizeOffset = StreamDescriptorRecordsOffset + ReservedStreamsLength;
	internal const int TotalClustersOffset = ClusterSizeOffset + ClusterSizeLength;
	internal const int MerkleRootOffset = TotalClustersOffset + TotalClustersLength;
	internal const int MasterKeyOffset = MerkleRootOffset + MerkleRootLength;

	private readonly Stream _headerStream;
	private readonly EndianBinaryReader _reader;
	private readonly EndianBinaryWriter _writer;

	private byte? _version;
	private StreamContainerPolicy? _policy;
	private long? _streamCount;
	private long? _streamDescriptorsCluster;
	private ushort? _streamDescriptorKeySize;
	private long? _reservedStreams;
	private int? _clusterSize;
	private long? _totalClusters;
	private readonly ICriticalObject _lock;
	private byte[] _merkleRoot;
	private byte[] _masterKey;

	internal StreamContainerHeader(ConcurrentStream rootStream, Endianness endianness) {
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

	public StreamContainerPolicy Policy {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (!_policy.HasValue) {
				using var _ = _headerStream.EnterRestorePositionSeek(PolicyOffset, SeekOrigin.Begin);
				_policy = (StreamContainerPolicy)_reader.ReadInt32();
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

	public long StreamCount {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (!_streamCount.HasValue) {
				using var _ = _headerStream.EnterRestorePositionSeek(StreamCountOffset, SeekOrigin.Begin);
				_streamCount = _reader.ReadInt64();
			}
			return _streamCount.Value;

		}
		internal set {
			using var accessScope = _lock.EnterAccessScope();
			if (_streamCount == value)
				return;
			_streamCount = value;
			using var _ = _headerStream.EnterRestorePositionSeek(StreamCountOffset, SeekOrigin.Begin);
			_writer.Write(_streamCount.Value);

		}
	}

	public long StreamDescriptorsEndCluster {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (!_streamDescriptorsCluster.HasValue) {
				using var _ = _headerStream.EnterRestorePositionSeek(StreamDescriptorsEndClusterOffset, SeekOrigin.Begin);
				_streamDescriptorsCluster = _reader.ReadInt64();
			}
			return _streamDescriptorsCluster.Value;
		}
		internal set {
			using var accessScope = _lock.EnterAccessScope();
			if (_streamDescriptorsCluster == value)
				return;
			_streamDescriptorsCluster = value;
			using var _ = _headerStream.EnterRestorePositionSeek(StreamDescriptorsEndClusterOffset, SeekOrigin.Begin);
			_writer.Write(_streamDescriptorsCluster.Value);
		}
	}

	public ushort StreamDescriptorKeySize {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (!_streamDescriptorKeySize.HasValue) {
				using var _ = _headerStream.EnterRestorePositionSeek(StreamDescriptorKeySizeOffset, SeekOrigin.Begin);
				_streamDescriptorKeySize = _reader.ReadUInt16();
			}
			return _streamDescriptorKeySize.Value;
		}
		internal set {
			using var accessScope = _lock.EnterAccessScope();
			if (_streamDescriptorKeySize == value)
				return;
			_streamDescriptorKeySize = value;
			using var _ = _headerStream.EnterRestorePositionSeek(StreamDescriptorKeySizeOffset, SeekOrigin.Begin);
			_writer.Write((ushort)_streamDescriptorKeySize.Value);

		}
	}

	public long ReservedStreams {
		get {
			using var accessScope = _lock.EnterAccessScope();
			if (!_reservedStreams.HasValue) {
				using var _ = _headerStream.EnterRestorePositionSeek(StreamDescriptorRecordsOffset, SeekOrigin.Begin);
				_reservedStreams = _reader.ReadInt64();
			}
			return _reservedStreams.Value;
		}
		internal set {
			using var accessScope = _lock.EnterAccessScope();

			if (_reservedStreams == value)
				return;
			if (value == 0 && Policy.HasFlag(StreamContainerPolicy.TrackKey))
				throw new InvalidOperationException($"Cannot set {nameof(ReservedStreams)} to 0 as {nameof(Policy)} has {StreamContainerPolicy.TrackKey} enabled");

			if (StreamCount > _reservedStreams.GetValueOrDefault(0))
				throw new InvalidOperationException($"Cannot set {nameof(ReservedStreams)} to {value} as records already exist with value");

			_reservedStreams = value;
			using var _ = _headerStream.EnterRestorePositionSeek(StreamDescriptorRecordsOffset, SeekOrigin.Begin);
			_writer.Write(_reservedStreams.Value);
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
		_streamDescriptorsCluster = null;
		_streamDescriptorKeySize = null;
		_reservedStreams = null;
		_clusterSize = null;
		_totalClusters = null;
		_streamCount = null;
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

		Guard.Ensure(_headerStream.Position == StreamCountOffset);
		_writer.Write((long)0L); // Records

		Guard.Ensure(_headerStream.Position == StreamDescriptorsEndClusterOffset);
		_writer.Write((long)Cluster.Null); // StreamDescriptorsEndCluster

		Guard.Ensure(_headerStream.Position == StreamDescriptorKeySizeOffset);
		_writer.Write((ushort)recordKeySize); // StreamDescriptorKeySize

		Guard.Ensure(_headerStream.Position == StreamDescriptorRecordsOffset);
		_writer.Write((long)reservedRecords); // ReservedStreams

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
		Guard.Ensure(ReservedStreams >= 0, $"Corrupt header property {nameof(ReservedStreams)} value was {ReservedStreams} bytes");
		Guard.Ensure(StreamCount >= 0, $"Corrupt header property {nameof(StreamCount)} value was {StreamCount} bytes");
		Guard.Ensure(StreamDescriptorsEndCluster >= Cluster.Null, $"Corrupt header property {nameof(StreamDescriptorsEndCluster)} value was {StreamDescriptorsEndCluster} bytes");
		Guard.Against(Policy.HasFlag(StreamContainerPolicy.TrackKey) && StreamDescriptorKeySize <= 0, $"Corrupt header property {nameof(StreamDescriptorKeySize)} value was {StreamDescriptorKeySize} but {nameof(Policy)} property value was {StreamDescriptorKeySize}");
	}

	public void ResetMerkleRoot() => MerkleRoot = new byte[MerkleRootLength];

	public override string ToString() {
		using var accessScope = _lock.EnterAccessScope();
		return $"[{nameof(StreamContainerHeader)}] {nameof(Version)}: {Version}, {nameof(ClusterSize)}: {ClusterSize}, {nameof(TotalClusters)}: {TotalClusters}, {nameof(StreamCount)}: {StreamCount}, {nameof(StreamDescriptorsEndCluster)}: {StreamDescriptorsEndCluster}, {nameof(ReservedStreams)}: {ReservedStreams}, {nameof(Policy)}: {Policy}, {nameof(MerkleRoot)}: {MerkleRoot.ToHexString(true)}";
	}



}
