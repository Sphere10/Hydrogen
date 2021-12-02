using System;
using System.Collections.Generic;
using System.Linq;
using Sphere10.Framework.Values;

namespace Sphere10.Framework {

	public class ByteArraySerializer : ItemSerializerBase<byte[]> {

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
