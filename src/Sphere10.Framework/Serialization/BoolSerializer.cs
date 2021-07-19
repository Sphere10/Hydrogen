using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	public class BoolSerializer : IItemSerializer<bool> {
		public bool IsFixedSize => true;
		public int FixedSize => sizeof(bool);
		
		public int CalculateTotalSize(IEnumerable<bool> items, bool calculateIndividualItems, out int[] itemSizes) {
			var enumerable = items as bool[] ?? items.ToArray();
			int sum = enumerable.Length * FixedSize;

			itemSizes = Enumerable.Repeat(FixedSize, enumerable.Length).ToArray();
			return sum;
		}

		public int CalculateSize(bool item) => FixedSize;

		public bool TrySerialize(bool item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				writer.Write(item);
				bytesWritten = FixedSize;
				return true;
			} catch (Exception) {
				bytesWritten = 0;
				return false;
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out bool item) {
			try {
				item = reader.ReadBoolean();
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}
	}

}
