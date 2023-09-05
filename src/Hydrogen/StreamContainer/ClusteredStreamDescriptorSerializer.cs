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

	internal const long TraitsOffset = 0;
	internal const long StartClusterOffset = TraitsOffset + TraitsLength;
	internal const long EndClusterOffset = StartClusterOffset + StartClusterLength;
	internal const long SizeOffset = EndClusterOffset + EndClusterLength;

	public ClusteredStreamDescriptorSerializer()
		: base(TraitsLength + StartClusterLength + EndClusterLength + SizeLength) {
	}

	public override void SerializeInternal(ClusteredStreamDescriptor item, EndianBinaryWriter writer) {
		writer.Write((byte)item.Traits);
		writer.Write((long)item.StartCluster);
		writer.Write((long)item.EndCluster);
		writer.Write((long)item.Size);
	}

	public override ClusteredStreamDescriptor Deserialize(EndianBinaryReader reader) {
		var item = new ClusteredStreamDescriptor {
			Traits = (ClusteredStreamTraits)reader.ReadByte(),
			StartCluster = reader.ReadInt64(),
			EndCluster = reader.ReadInt64(),
			Size = reader.ReadInt64()
		};
		return item;
	}
}
