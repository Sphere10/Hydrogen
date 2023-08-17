// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Diagnostics;

namespace Hydrogen;

public class ClusterSerializer : StaticSizeItemSerializerBase<Cluster> {

	public ClusterSerializer(int clusterDataSize) 
		: base(Cluster.TraitsLength + Cluster.PrevLength + Cluster.NextLength + clusterDataSize) {
		// cluster has an envelope of 9 bytes
		Guard.ArgumentInRange(clusterDataSize, 1, int.MaxValue, nameof(clusterDataSize));
		ClusterDataSize = clusterDataSize;
	}

	public int ClusterDataSize { get; }

	public override void SerializeInternal(Cluster item, EndianBinaryWriter writer) {
		Guard.ArgumentNotNull(item, nameof(item));
		Guard.ArgumentNotNull(writer, nameof(writer));
		Guard.Argument(item.Data.Length == ClusterDataSize, nameof(item), "Unexpected cluster data size");
		writer.Write((byte)item.Traits);
		writer.Write(item.Prev);
		writer.Write(item.Next);
		Debug.Assert(item.Data.Length == ClusterDataSize);
		writer.Write(item.Data);
	}

	public override Cluster Deserialize(EndianBinaryReader reader) {
		Guard.ArgumentNotNull(reader, nameof(reader));
		return new Cluster {
			Traits = (ClusterTraits)reader.ReadByte(),
			Prev = reader.ReadInt64(),
			Next = reader.ReadInt64(),
			Data = reader.ReadBytes(ClusterDataSize),
		};
	}
}
