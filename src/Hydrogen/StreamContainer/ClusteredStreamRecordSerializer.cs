// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class ClusteredStreamDescriptorSerializer : StaticSizeItemSerializerBase<ClusteredStreamDescriptor> {

	internal const int TraitsLength = sizeof(byte);
	internal const int StartClusterLength = sizeof(long);
	internal const int EndClusterLength = sizeof(long);
	internal const int SizeLength = sizeof(long);
	internal const int KeyChecksumLength = sizeof(int);

	internal const long TraitsOffset = 0;
	internal const long StartClusterOffset = TraitsOffset + TraitsLength;
	internal const long EndClusterOffset = StartClusterOffset + StartClusterLength;
	internal const long SizeOffset = EndClusterOffset + EndClusterLength;
	internal const long KeyChecksumOffset = SizeOffset + SizeLength;
	internal const long KeyOffset = KeyChecksumOffset + KeyChecksumLength;

	private readonly StreamContainerPolicy _policy;
	private readonly long _keySize;

	public ClusteredStreamDescriptorSerializer(StreamContainerPolicy policy, long keySize)
		: base(DetermineSizeBasedOnPolicy(policy, keySize)) {
		_policy = policy;
		_keySize = keySize;
	}

	public override void SerializeInternal(ClusteredStreamDescriptor item, EndianBinaryWriter writer) {
		writer.Write((byte)item.Traits);
		writer.Write((long)item.StartCluster);
		writer.Write((long)item.EndCluster);
		writer.Write((long)item.Size);
		if (_policy.HasFlag(StreamContainerPolicy.TrackChecksums))
			writer.Write((int)item.KeyChecksum);

		if (_policy.HasFlag(StreamContainerPolicy.TrackKey)) {
			if (item.Key?.Length != _keySize) 
				throw new InvalidOperationException($"Key size mismatch. Expected: {_keySize} but was {item.Key.Length}");
			writer.Write(item.Key);
		}
	}

	public override ClusteredStreamDescriptor Deserialize(EndianBinaryReader reader) {
		var item = new ClusteredStreamDescriptor {
			Traits = (ClusteredStreamTraits)reader.ReadByte(),
			StartCluster = reader.ReadInt64(),
			EndCluster = reader.ReadInt64(),
			Size = reader.ReadInt64()
		};

		if (_policy.HasFlag(StreamContainerPolicy.TrackChecksums))
			item.KeyChecksum = reader.ReadInt32();

		if (_policy.HasFlag(StreamContainerPolicy.TrackKey))
			item.Key = reader.ReadBytes(_keySize);

		return item;
	}


	static long DetermineSizeBasedOnPolicy(StreamContainerPolicy policy, long keySize) {
		long size = TraitsLength + StartClusterLength + EndClusterLength + SizeLength;

		if (policy.HasFlag(StreamContainerPolicy.TrackChecksums))
			size += sizeof(int);

		if (policy.HasFlag(StreamContainerPolicy.TrackKey))
			size += keySize;

		return size;
	}
}
