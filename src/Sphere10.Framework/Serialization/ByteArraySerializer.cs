using System;
using System.Collections.Generic;
using System.Linq;
using Sphere10.Framework.Values;

namespace Sphere10.Framework {
	public class ByteArraySerializer : IItemSerializer<byte[]> {

		public bool IsFixedSize => false;
		public int FixedSize => -1;

		public int CalculateTotalSize(IEnumerable<byte[]> items, bool calculateIndividualItems, out int[] itemSizes) {
			var itemsArray = items as byte[][] ?? items.ToArray();
			var sizes = new int[itemsArray.Length];
			for (var i = 0; i < itemsArray.Length; i++) {
				sizes[i] = CalculateSize(itemsArray[i]);
			}

			itemSizes = calculateIndividualItems ? sizes : default;
			return sizes.Sum(x => x);
		}

		public int CalculateSize(byte[] item) {
			int lengthByteCount = new CVarInt((ulong)item.Length, sizeof(int)).ToBytes().Length;
			return lengthByteCount + item.Length;
		}

		public bool TrySerialize(byte[] item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				byte[] lengthBytes = new CVarInt((ulong)item.Length, sizeof(int)).ToBytes();
					
				writer.Write(lengthBytes);
				writer.Write(item);
				
				bytesWritten = lengthBytes.Length + item.Length;
				return true;
			} catch (Exception) {
				bytesWritten = 0;
				return false;
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out byte[] item) {
			try {
				var length = CVarInt.Read(sizeof(int), reader.BaseStream);
				item = reader.ReadBytes((int)length.ToLong());
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}
	}
}
