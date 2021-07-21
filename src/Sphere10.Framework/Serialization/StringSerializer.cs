using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sphere10.Framework {

	public class StringSerializer : IItemSerializer<string> {

		public bool TrySerialize(string item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				using var stream = new MemoryStream();
				using var innerWriter = new EndianBinaryWriter(EndianBitConverter.Little, stream);
				innerWriter.Write(item);
				writer.Write(stream.ToArray());

				bytesWritten = (int)stream.Length;
				
				return true;
			} catch (Exception) {
				bytesWritten = 0;
				return false;
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out string item) {
			try {
				item = reader.ReadString();
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}
		public bool IsFixedSize => false;
		public int FixedSize => -1;

		public int CalculateTotalSize(IEnumerable<string> items, bool calculateIndividualItems, out int[] itemSizes) {

			var arr = items as string[] ?? items.ToArray();
			var sizes = new int[arr.Length];

			for (int i = 0; i < sizes.Length; i++) {
				sizes[i] = CalculateSize(arr[i]);
			}

			itemSizes = calculateIndividualItems ? sizes : null;
			
			return sizes.Sum(x => x);
		}

		/// <summary>
			/// Calculate size - serializes the string to calculate the size since we need to determine the 7bit encoded int
			/// for length. 
			/// </summary>
			/// <param name="item"> string to be sized</param>
			/// <returns> size in bytes.</returns>
			public int CalculateSize(string item) {
				using var stream = new MemoryStream();
				using var innerWriter = new EndianBinaryWriter(EndianBitConverter.Little, stream);
				innerWriter.Write(item);

				return (int)stream.Length;
			}
		}
}
