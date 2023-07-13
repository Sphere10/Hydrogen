// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

internal class ClusterSerializer : StaticSizeItemSerializerBase<Cluster> {
	private readonly int _clusterDataSize;

	public ClusterSerializer(int clusterDataSize) : base(sizeof(byte) + sizeof(int) + sizeof(int) + clusterDataSize) {
		// cluster has an envelope of 9 bytes
		Guard.ArgumentInRange(clusterDataSize, 1, int.MaxValue, nameof(clusterDataSize));
		_clusterDataSize = clusterDataSize;
	}

	public override bool TrySerialize(Cluster item, EndianBinaryWriter writer) {
		Guard.ArgumentNotNull(item, nameof(item));
		Guard.ArgumentNotNull(writer, nameof(writer));
		Guard.Argument(item.Data.Length == _clusterDataSize, nameof(item), "Unexpected cluster data size");
		writer.Write((byte)item.Traits);
		writer.Write(item.Prev);
		writer.Write(item.Next);
		writer.Write(item.Data);
		return true;
	}

	public override bool TryDeserialize(EndianBinaryReader reader, out Cluster item) {
		Guard.ArgumentNotNull(reader, nameof(reader));
		item = new Cluster {
			Traits = (ClusterTraits)reader.ReadByte(),
			Prev = reader.ReadInt32(),
			Next = reader.ReadInt32(),
			Data = reader.ReadBytes(_clusterDataSize),
		};
		return true;
	}
}
