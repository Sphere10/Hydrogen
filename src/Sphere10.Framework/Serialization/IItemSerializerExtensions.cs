using System.IO;

namespace Sphere10.Framework {


	public static class IItemSerializerExtensions {

		public static byte[] SerializeLE<TItem>(this IItemSerializer<TItem> serializer, TItem @object) {
			var stream = new MemoryStream();
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream);
			serializer.Serialize(@object, writer);
			return stream.GetBuffer();
		}

	}
}