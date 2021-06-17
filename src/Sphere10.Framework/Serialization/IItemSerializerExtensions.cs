using System.IO;

namespace Sphere10.Framework {


	public static class IItemSerializerExtensions {

		public static byte[] SerializeLE<TItem>(this IItemSerializer<TItem> serializer, TItem @object) {
			using var stream = new MemoryStream();
			using var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream);
			serializer.Serialize(@object, writer);
			return stream.GetBuffer();
		}

		public static TItem DeSerializeLE<TItem>(this IItemSerializer<TItem> serializer, byte[] bytes) {
			using var stream = new MemoryStream(bytes);
			using var reader = new EndianBinaryReader(EndianBitConverter.Little, stream);
			return serializer.Deserialize(bytes.Length, reader);
		}

		public static IItemSerializer<TBase> AsBaseSerializer<TItem, TBase>(this IItemSerializer<TItem> serializer) where TItem : TBase
			=> new CastedSerializer<TBase, TItem>(serializer);


	}
}