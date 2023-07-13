// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class ClusteredStreamRecordSerializer : StaticSizeItemSerializerBase<ClusteredStreamRecord> {
	private readonly ClusteredStoragePolicy _policy;
	private readonly long _keySize;

	public ClusteredStreamRecordSerializer(ClusteredStoragePolicy policy, long keySize)
		: base(DetermineSizeBasedOnPolicy(policy, keySize)) {
		_policy = policy;
		_keySize = keySize;
	}

	public override bool TrySerialize(ClusteredStreamRecord item, EndianBinaryWriter writer) {
		writer.Write((byte)item.Traits);
		writer.Write((long)item.StartCluster);
		writer.Write((long)item.Size);
		if (_policy.HasFlag(ClusteredStoragePolicy.TrackChecksums))
			writer.Write((int)item.KeyChecksum);

		if (_policy.HasFlag(ClusteredStoragePolicy.TrackKey)) {
			if (item.Key?.Length != _keySize)
				return false;
			writer.Write(item.Key);
		}

		return true;
	}

	public override bool TryDeserialize(EndianBinaryReader reader, out ClusteredStreamRecord item) {
		item = new ClusteredStreamRecord {
			Traits = (ClusteredStreamTraits)reader.ReadByte(),
			StartCluster = reader.ReadInt64(),
			Size = reader.ReadInt64()
		};

		if (_policy.HasFlag(ClusteredStoragePolicy.TrackChecksums))
			item.KeyChecksum = reader.ReadInt32();

		if (_policy.HasFlag(ClusteredStoragePolicy.TrackKey))
			item.Key = reader.ReadBytes(_keySize);

		return true;
	}


	static long DetermineSizeBasedOnPolicy(ClusteredStoragePolicy policy, long keySize) {
		long size = sizeof(byte) + sizeof(long) + sizeof(long); // Traits + StartCluster + Size

		if (policy.HasFlag(ClusteredStoragePolicy.TrackChecksums))
			size += sizeof(int);

		if (policy.HasFlag(ClusteredStoragePolicy.TrackKey))
			size += keySize;

		return size;
	}
}
