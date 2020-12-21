using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Sphere10.Framework {

	public interface IObjectSerializer<TItem> : IObjectSizer<TItem> {

		int Serialize(TItem @object, EndianBinaryWriter writer);

		TItem Deserialize(int size, EndianBinaryReader reader);
	}


	public static class IObjectSerializerExtensions {

		public static byte[] SerializeLE<TItem>(this IObjectSerializer<TItem> serializer, TItem @object) {
			var stream = new MemoryStream();
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream);
			serializer.Serialize(@object, writer);
			return stream.GetBuffer();
		}

	}
}