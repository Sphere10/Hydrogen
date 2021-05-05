using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	public class BoolSerializer : IItemSerializer<bool> {
		public bool IsFixedSize { get; } = true;
		public int FixedSize { get; } = sizeof(bool);
		public int CalculateTotalSize(IEnumerable<bool> items, bool calculateIndividualItems, out int[] itemSizes) {
			var enumerable = items as bool[] ?? items.ToArray();
			int sum = enumerable.Length * FixedSize;

			itemSizes = Enumerable.Repeat(FixedSize, enumerable.Length).ToArray();
			return sum;
		}

		public int CalculateSize(bool item) => FixedSize;

		public int Serialize(bool @object, EndianBinaryWriter writer) {
			writer.Write(@object);

			return sizeof(bool);
		}

		public bool Deserialize(int size, EndianBinaryReader reader) {
			return reader.ReadBoolean();
		}
	}

}