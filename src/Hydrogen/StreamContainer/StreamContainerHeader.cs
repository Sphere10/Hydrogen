// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Runtime.CompilerServices;
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

	private readonly Stream _headerStream;
	private readonly Stream _extensionPropertiesStream;
	private readonly EndianBinaryReader _reader;
	private readonly EndianBinaryWriter _writer;

	private readonly ICriticalObject _lock;

	private readonly StreamMappedProperty<byte> _versionProperty;
	private readonly StreamMappedProperty<StreamContainerPolicy> _policyProperty;
	private readonly StreamMappedProperty<long> _streamCountProperty;
	private readonly StreamMappedProperty<long> _streamDescriptorsClusterProperty;
	private readonly StreamMappedProperty<ushort> _streamDescriptorKeySizeProperty;
	private readonly StreamMappedProperty<long> _reservedStreamsProperty;
	private readonly StreamMappedProperty<int> _clusterSizeProperty;
	private readonly StreamMappedProperty<long> _totalClustersProperty;

	internal StreamContainerHeader(ConcurrentStream rootStream, Endianness endianness) {
		Guard.ArgumentNotNull(rootStream, nameof(rootStream));
		_headerStream = rootStream.AsBounded(0, ByteLength - 1, allowInnerResize: true); 
		_lock = rootStream;
		_reader = new EndianBinaryReader(EndianBitConverter.For(endianness), _headerStream);
		_writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), _headerStream);
		_versionProperty = new StreamMappedProperty<byte>(_headerStream, VersionOffset, VersionLength, PrimitiveSerializer<byte>.Instance, _reader, _writer, @lock: _lock);
		_policyProperty = new StreamMappedProperty<StreamContainerPolicy>(_headerStream, PolicyOffset, PolicyLength, EnumSerializer<StreamContainerPolicy>.Instance, _reader, _writer, @lock: _lock);
		_streamCountProperty = new StreamMappedProperty<long>(_headerStream, StreamCountOffset, StreamCountLength, PrimitiveSerializer<long>.Instance, _reader, _writer, @lock: _lock);
		_streamDescriptorsClusterProperty = new StreamMappedProperty<long>(_headerStream, StreamDescriptorsEndClusterOffset, StreamDescriptorsEndClusterLength, PrimitiveSerializer<long>.Instance, _reader, _writer, @lock: _lock);
		_streamDescriptorKeySizeProperty = new StreamMappedProperty<ushort>(_headerStream, StreamDescriptorKeySizeOffset, StreamDescriptorKeySizeLength, PrimitiveSerializer<ushort>.Instance, _reader, _writer, @lock: _lock);
		_reservedStreamsProperty = new StreamMappedProperty<long>(_headerStream, StreamDescriptorRecordsOffset, ReservedStreamsLength, PrimitiveSerializer<long>.Instance, _reader, _writer, @lock: _lock);
		_clusterSizeProperty = new StreamMappedProperty<int>(_headerStream, ClusterSizeOffset, ClusterSizeLength, PrimitiveSerializer<int>.Instance, _reader, _writer, @lock: _lock);
		_totalClustersProperty = new StreamMappedProperty<long>(_headerStream, TotalClustersOffset, TotalClustersLength, PrimitiveSerializer<long>.Instance, _reader, _writer, @lock: _lock);
		_extensionPropertiesStream = rootStream.AsBounded(TotalClustersOffset + TotalClustersLength, ByteLength - 1, allowInnerResize: false, useRelativeOffset: true);
	}

	public byte Version { get => _versionProperty.Value; internal set => _versionProperty.Value = value; }

	public StreamContainerPolicy Policy { get => _policyProperty.Value; internal set => _policyProperty.Value = value; }

	public long StreamCount { get => _streamCountProperty.Value; internal set => _streamCountProperty.Value = value; }

	public long StreamDescriptorsEndCluster { get => _streamDescriptorsClusterProperty.Value; internal set => _streamDescriptorsClusterProperty.Value = value; }

	public ushort StreamDescriptorKeySize { get => _streamDescriptorKeySizeProperty.Value; internal set => _streamDescriptorKeySizeProperty.Value = value; }

	public long ReservedStreams {
		get => _reservedStreamsProperty.Value;
		internal set {
			if (value == 0 && Policy.HasFlag(StreamContainerPolicy.TrackKey))
				throw new InvalidOperationException($"Cannot set {nameof(ReservedStreams)} to 0 as {nameof(Policy)} has {StreamContainerPolicy.TrackKey} enabled");

			if (StreamCount > _reservedStreamsProperty.Value)
				throw new InvalidOperationException($"Cannot set {nameof(ReservedStreams)} to {value} as records already exist with value");

			_reservedStreamsProperty.Value = value;
		}
	}

	public int ClusterSize {
		get => _clusterSizeProperty.Value;
		set => _clusterSizeProperty.Value = value;
	}

	public long TotalClusters { get => _totalClustersProperty.Value; set => _totalClustersProperty.Value = value; }

	public StreamMappedProperty<T> CreateExtensionProperty<T>(long offset, int length, IItemSerializer<T> serializer) {
		Guard.ArgumentInRange(offset, 0, _extensionPropertiesStream.Length - 1, nameof(offset));
		Guard.ArgumentInRange(length, 1, _extensionPropertiesStream.Length - offset, nameof(length));
		Guard.ArgumentNotNull(serializer, nameof(serializer));
		return new StreamMappedProperty<T>(_extensionPropertiesStream, offset, length, serializer, _reader, _writer, @lock: _lock);
	}

	public void Load() {
		using var accessScope = _lock.EnterAccessScope();
		Guard.Ensure(_headerStream.Length >= ByteLength, "Missing or corrupt header");
	}

	public void Create(byte version, int clusterSize, long recordKeySize, long reservedRecords) {
		Guard.ArgumentGTE(clusterSize, 1, nameof(clusterSize));
		Guard.ArgumentInRange(recordKeySize, 0, ushort.MaxValue, nameof(recordKeySize));
		Guard.ArgumentGTE(reservedRecords, 0, nameof(reservedRecords));
		
		using var accessScope = _lock.EnterAccessScope();
		Guard.Ensure(_headerStream.Length == 0, "Header must be empty");
		_headerStream.SetLength(ByteLength);
		Version = version;
		Policy = 0;
		StreamCount = 0;
		StreamDescriptorsEndCluster = Cluster.Null;
		StreamDescriptorKeySize = (ushort)recordKeySize;
		ReservedStreams = reservedRecords;
		ClusterSize = clusterSize;
		TotalClusters = 0;
		Guard.Ensure(_headerStream.Position == ByteLength - _extensionPropertiesStream.Length);
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


	public override string ToString() {
		using var accessScope = _lock.EnterAccessScope();
		return $"[{nameof(StreamContainerHeader)}] {nameof(Version)}: {Version}, {nameof(ClusterSize)}: {ClusterSize}, {nameof(TotalClusters)}: {TotalClusters}, {nameof(StreamCount)}: {StreamCount}, {nameof(StreamDescriptorsEndCluster)}: {StreamDescriptorsEndCluster}, {nameof(ReservedStreams)}: {ReservedStreams}, {nameof(Policy)}: {Policy}, Extension Data: {_extensionPropertiesStream.ToArray()}";
	}

}
