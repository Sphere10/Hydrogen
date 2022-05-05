using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hydrogen.FastReflection;

namespace Hydrogen {
	public class DateTimeSerializer : IItemSerializer<DateTime> {

		public bool IsStaticSize => true;

		public int StaticSize => sizeof(ulong);

		public int CalculateTotalSize(IEnumerable<DateTime> items, bool calculateIndividualItems, out int[] itemSizes) {
			itemSizes = Array.Empty<int>();
			var count = items.Count();

			if (calculateIndividualItems)
				itemSizes = Enumerable.Repeat(StaticSize, count).ToArray();

			return StaticSize * count;
		}

		public int CalculateSize(DateTime item) {
			return sizeof(long);
		}

		public bool TrySerialize(DateTime item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				var property = typeof(DateTime).GetProperty("Ticks", BindingFlags.Public | BindingFlags.Instance);
				var dateData = (long)property.FastGetValue(item);
				byte[] bytes = writer.BitConverter.GetBytes(dateData);
				writer.Write(bytes);

				bytesWritten = bytes.Length;
				return true;
			} catch (Exception) {
				bytesWritten = 0;
				return false;
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out DateTime item) {
			try {
				long ticks = reader.ReadInt64();
				item = new DateTime(ticks);
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}
	}


}
