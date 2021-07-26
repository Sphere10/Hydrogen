using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sphere10.Framework.Values;
using Tools;

namespace Sphere10.Framework {

	public class StringSerializer : IItemSerializer<string> {
		public Encoding TextEncoding { get; }

		public StringSerializer(Encoding textEncoding) {
			Guard.ArgumentNotNull(textEncoding, nameof(textEncoding));
			TextEncoding = textEncoding;
		}
		
		public bool TrySerialize(string item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				var bytes = TextEncoding.GetBytes(item);
				var lengthBytes = new CVarInt((ulong)bytes.Length).ToBytes();
				writer.Write(lengthBytes);
				writer.Write(bytes);

				bytesWritten = bytes.Length + lengthBytes.Length;
				return true;
			} catch (Exception) {
				bytesWritten = 0;
				return false;
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out string item) {
			try {
				var count = CVarInt.Read(sizeof(int), reader.BaseStream);
				item = TextEncoding.GetString(reader.ReadBytes((int)count.ToLong()));
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

			for (var i = 0; i < sizes.Length; i++) {
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
			int count = new CVarInt((ulong)item.Length).ToBytes().Length;
			int charBytes = TextEncoding.GetByteCount(item);
			
			return count + charBytes;
		}
	}
}
