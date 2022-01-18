namespace Sphere10.Framework {
	public class ClusteredStreamRecordSerializer : StaticSizeObjectSerializer<ClusteredStreamRecord> {

		public ClusteredStreamRecordSerializer()
			: base(sizeof(byte) +sizeof(int) + sizeof(int)) {
		}

		public override bool TrySerialize(ClusteredStreamRecord item, EndianBinaryWriter writer) {
			writer.Write((byte)item.Traits);
			writer.Write(item.StartCluster);
			writer.Write(item.Size);
			return true;
		}

		public override bool TryDeserialize(EndianBinaryReader reader, out ClusteredStreamRecord item) {
			item = new ClusteredStreamRecord {
				Traits = (StreamRecordTraits) reader.ReadByte(),
				StartCluster = reader.ReadInt32(),
				Size = reader.ReadInt32()
			};
			return true;
		}
	}
}
