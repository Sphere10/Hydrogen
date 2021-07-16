using System.Collections.Generic;

namespace Sphere10.Framework {
	public class ByteArraySerializer : IItemSerializer<byte[]> {

		public bool IsFixedSize { get; } = false;
		public int FixedSize { get; } = -1;

		public int CalculateTotalSize(IEnumerable<byte[]> items, bool calculateIndividualItems, out int[] itemSizes) {
			throw new System.NotImplementedException();
		}

		public int CalculateSize(byte[] item) {
			throw new System.NotImplementedException();
		}

		public int Serialize(byte[] @object, EndianBinaryWriter writer) {
			writer.Write(@object.Length);
			writer.Write(@object);

			return sizeof(int) + @object.Length;
		}

		public byte[] Deserialize(int size, EndianBinaryReader reader) {
			int length = reader.ReadInt32();
			return reader.ReadBytes(length);
		}
	}
}
