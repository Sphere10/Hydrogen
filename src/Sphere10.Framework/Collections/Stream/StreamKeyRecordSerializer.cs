namespace Sphere10.Framework {
	public class StreamKeyRecordSerializer : StaticSizeObjectSerializer<StreamKeyRecord> {

		public StreamKeyRecordSerializer()
			: base(+sizeof(byte) + sizeof(int) + sizeof(int) + sizeof(int)) {
		}

		public override bool TrySerialize(StreamKeyRecord item, EndianBinaryWriter writer) {
			writer.Write((byte)item.Traits);
			writer.Write(item.Size);
			writer.Write(item.StartCluster);
			writer.Write(item.KeyChecksum);
			return true;
		}

		public override bool TryDeserialize(EndianBinaryReader reader, out StreamKeyRecord item) {
			item = new StreamKeyRecord {
				Traits = (StreamRecordTraits)reader.ReadByte(),
				Size = reader.ReadInt32(),
				StartCluster = reader.ReadInt32(),
				KeyChecksum = reader.ReadInt32(),
			};
			return true;
		}
	}
}
