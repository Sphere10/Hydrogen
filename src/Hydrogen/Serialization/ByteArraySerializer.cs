using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen {

	public class ByteArraySerializer : ItemSerializer<byte[]> {

		public override int CalculateSize(byte[] item) => item.Length;

		public override bool TrySerialize(byte[] item, EndianBinaryWriter writer, out int bytesWritten) {
			writer.Write(item);
			bytesWritten = item.Length;
			return true;
		}

		public override bool TryDeserialize(int byteSize, EndianBinaryReader reader, out byte[] item) {
			item = reader.ReadBytes(byteSize);
			return true;
		}
	}
}
