using System;
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
				int length = reader.ReadInt32();
				item = reader.ReadBytes(length);
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}
	}
}
