namespace Sphere10.Framework {
	public class IntSerializer : StaticSizeItemSerializer<int> {
		public IntSerializer() : base(4) {
		}

		public override bool TrySerialize(int item, EndianBinaryWriter writer) {
			writer.Write(writer.BitConverter.GetBytes(item));
			return true;
		}

		public override bool TryDeserialize(EndianBinaryReader reader, out int item) {
			item = reader.ReadInt32();
			return true;
		}
	}
}
