// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class ClusteredStorageTrackSerializer : StaticSizeItemSerializerBase<ClusteredStorageTrack> {
	public ClusteredStorageTrackSerializer() : base(8) {
	}

	public override bool TrySerialize(ClusteredStorageTrack item, EndianBinaryWriter writer) {
		writer.Write(item.StartCluster);
		writer.Write(item.RecordCount);
		return true;
	}

	public override bool TryDeserialize(EndianBinaryReader reader, out ClusteredStorageTrack item) {
		item = new ClusteredStorageTrack {
			StartCluster = reader.ReadInt32(),
			RecordCount = reader.ReadInt32()
		};
		return true;
	}
}
