namespace Sphere10.Framework {
	public class ClusteredRecordSerializer : StaticSizeItemSerializer<ClusteredRecord> {

		public ClusteredRecordSerializer()
			: base(sizeof(byte) +sizeof(int) + sizeof(int)) {
		}

		public override bool TrySerialize(ClusteredRecord item, EndianBinaryWriter writer) {
			writer.Write((byte)item.Traits);
			writer.Write(item.StartCluster);
			writer.Write(item.Size);
			return true;
		}

		public override bool TryDeserialize(EndianBinaryReader reader, out ClusteredRecord item) {
			item = new ClusteredRecord {
				Traits = (StreamRecordTraits) reader.ReadByte(),
				StartCluster = reader.ReadInt32(),
				Size = reader.ReadInt32()
			};
			return true;
		}
	}
}
