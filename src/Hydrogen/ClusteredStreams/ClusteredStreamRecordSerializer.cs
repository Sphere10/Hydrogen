// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class ClusteredStreamRecordSerializer : StaticSizeItemSerializerBase<ClusteredStreamRecord> {
	private readonly ClusteredStoragePolicy _policy;
	private readonly long _keySize;

	public ClusteredStreamRecordSerializer(ClusteredStoragePolicy policy, long keySize)
		: base(DetermineSizeBasedOnPolicy(policy, keySize)) {
		_policy = policy;
		_keySize = keySize;
	}

	public override void SerializeInternal(ClusteredStreamRecord item, EndianBinaryWriter writer) {
		writer.Write((byte)item.Traits);
		writer.Write((long)item.StartCluster);
		writer.Write((long)item.EndCluster);
		writer.Write((long)item.Size);
		if (_policy.HasFlag(ClusteredStoragePolicy.TrackChecksums))
			writer.Write((int)item.KeyChecksum);

		if (_policy.HasFlag(ClusteredStoragePolicy.TrackKey)) {
			if (item.Key?.Length != _keySize) 
				throw new InvalidOperationException($"Key size mismatch. Expected: {_keySize} but was {item.Key.Length}");
			writer.Write(item.Key);
		}
	}

	public override ClusteredStreamRecord Deserialize(EndianBinaryReader reader) {
		var item = new ClusteredStreamRecord {
			Traits = (ClusteredStreamTraits)reader.ReadByte(),
			StartCluster = reader.ReadInt64(),
			EndCluster = reader.ReadInt64(),
			Size = reader.ReadInt64()
		};

		if (_policy.HasFlag(ClusteredStoragePolicy.TrackChecksums))
			item.KeyChecksum = reader.ReadInt32();

		if (_policy.HasFlag(ClusteredStoragePolicy.TrackKey))
			item.Key = reader.ReadBytes(_keySize);

		return item;
	}


	static long DetermineSizeBasedOnPolicy(ClusteredStoragePolicy policy, long keySize) {
		long size = ClusteredStreamRecord.TraitsLength + ClusteredStreamRecord.StartClusterLength + ClusteredStreamRecord.EndClusterLength + ClusteredStreamRecord.SizeLength;

		if (policy.HasFlag(ClusteredStoragePolicy.TrackChecksums))
			size += sizeof(int);

		if (policy.HasFlag(ClusteredStoragePolicy.TrackKey))
			size += keySize;

		return size;
	}
}
