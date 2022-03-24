namespace Sphere10.Framework;

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
