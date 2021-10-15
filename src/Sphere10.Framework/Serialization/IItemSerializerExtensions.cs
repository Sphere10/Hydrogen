using System;
using System.IO;

namespace Sphere10.Framework {


	public static class IItemSerializerExtensions {

		public static bool TrySerializeLE<TItem>(this IItemSerializer<TItem> serializer, TItem @object, out byte[] data) {
			using var stream = new MemoryStream();
			using var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream);
			if (!serializer.TrySerialize(@object, writer, out var bytesWritten)) {
				data = null;
				return false;
			}
			stream.Flush();
			data = stream.ToArray();
			return true;
		}

		public static int Serialize<TItem>(this IItemSerializer<TItem> serializer, TItem @object, EndianBinaryWriter writer) {
			if (!serializer.TrySerialize(@object, writer, out var bytesWritten))
				throw new InvalidOperationException($"Unable to serialize object of type '{@object?.GetType().Name ?? "NULL"}'");
			return bytesWritten;
		}

		public static byte[] SerializeLE<TItem>(this IItemSerializer<TItem> serializer, TItem item) {
			if (!serializer.TrySerializeLE(item, out var bytes))
				throw new InvalidOperationException($"Unable to little-endian serialize object of type '{item?.GetType().Name ?? "NULL"}'");
			return bytes;
		}

		public static bool TryDeserializeLE<TItem>(this IItemSerializer<TItem> serializer, ReadOnlySpan<byte> bytes, out TItem item) {
			using var stream = new MemoryStream(bytes.ToArray());
			using var reader = new EndianBinaryReader(EndianBitConverter.Little, stream);
			return serializer.TryDeserialize(bytes.Length, reader, out item);
		}

		public static TItem Deserialize<TItem>(this IItemSerializer<TItem> serializer, int byteSize, EndianBinaryReader reader) {
			if (!serializer.TryDeserialize(byteSize, reader, out var item))
				throw new InvalidOperationException($"Unable to deserialize object of size {byteSize}b");
			return item;
		}

		public static TItem DeserializeLE<TItem>(this IItemSerializer<TItem> serializer, ReadOnlySpan<byte> bytes) {
			if (!serializer.TryDeserializeLE(bytes, out var item))
				throw new InvalidOperationException($"Unable to little-endian deserialize object of type '{item?.GetType().Name ?? "NULL"}'");
			return item;
		}

		public static IItemSerializer<TBase> AsBaseSerializer<TItem, TBase>(this IItemSerializer<TItem> serializer) where TItem : TBase
			=> new CastedSerializer<TBase, TItem>(serializer);
	}
}