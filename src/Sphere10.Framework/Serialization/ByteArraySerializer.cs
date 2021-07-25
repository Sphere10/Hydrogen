using System;
using System.Collections.Generic;

namespace Sphere10.Framework {
	public class ByteArraySerializer : IItemSerializer<byte[]> {

		public bool IsFixedSize => false;
		public int FixedSize => -1;

		public int CalculateTotalSize(IEnumerable<byte[]> items, bool calculateIndividualItems, out int[] itemSizes) {
			throw new System.NotImplementedException();
		}

		public int CalculateSize(byte[] item) {
			return sizeof(int) + item.Length;
		}

		public bool TrySerialize(byte[] item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				writer.Write(item.Length);
				writer.Write(item);
				
				bytesWritten = sizeof(int) + item.Length;
				return true;
			} catch (Exception) {
				bytesWritten = 0;
				return false;
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out byte[] item) {
			try {
				var length = reader.ReadInt32();
				item = reader.ReadBytes(length);
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}
	}
}
