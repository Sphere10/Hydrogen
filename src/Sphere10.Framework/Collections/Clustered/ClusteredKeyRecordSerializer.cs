namespace Sphere10.Framework {
	public class ClusteredKeyRecordSerializer : StaticSizeObjectSerializer<ClusteredKeyRecord> {

		public ClusteredKeyRecordSerializer()
			: base(+sizeof(byte) + sizeof(int) + sizeof(int) + sizeof(int)) {
		}

		public override bool TrySerialize(ClusteredKeyRecord item, EndianBinaryWriter writer) {
			writer.Write((byte)item.Traits);
			writer.Write(item.Size);
			writer.Write(item.StartCluster);
			writer.Write(item.KeyChecksum);
			return true;
		}

		public override bool TryDeserialize(EndianBinaryReader reader, out ClusteredKeyRecord item) {
			item = new ClusteredKeyRecord {
				Traits = (StreamRecordTraits)reader.ReadByte(),
				Size = reader.ReadInt32(),
				StartCluster = reader.ReadInt32(),
				KeyChecksum = reader.ReadInt32(),
			};
			return true;
		}
	}
}
