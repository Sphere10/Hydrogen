using System;
using System.Collections.Generic;
using System.Linq;
using Sphere10.Framework.Values;

namespace Sphere10.Framework {

	public class StaticSizeByteArraySerializer : StaticSizeItemSerializerBase<byte[]> {

		public StaticSizeByteArraySerializer(int size) : base(size) {
		}

		public override bool TrySerialize(byte[] item, EndianBinaryWriter writer) {
			Guard.Ensure(item.Length == StaticSize, "Incorrectly sized");
			writer.Write(item);
			return true;
		}

		public override bool TryDeserialize(EndianBinaryReader reader, out byte[] item) {
			item = reader.ReadBytes(StaticSize);
			return true;
		}
	}
}
