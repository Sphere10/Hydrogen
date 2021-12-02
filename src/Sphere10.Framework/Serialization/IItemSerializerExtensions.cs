using System;
using System.IO;

namespace Sphere10.Framework {

	public static class IItemSerializerExtensions {

		public static byte[] SerializeLE<TItem>(this IItemSerializer<TItem> serializer, TItem item)
			=> serializer.Serialize(item, Endianness.LittleEndian);

		public static byte[] Serialize<TItem>(this IItemSerializer<TItem> serializer, TItem item, Endianness endianness) {
			if (!serializer.TrySerialize(item, out var bytes, endianness))
				throw new InvalidOperationException($"Unable to little-endian serialize object of type '{item?.GetType().Name ?? "NULL"}'");
			return bytes;
		}
		
		public static int Serialize<TItem>(this IItemSerializer<TItem> serializer, TItem @object, EndianBinaryWriter writer) {
			if (!serializer.TrySerialize(@object, writer, out var bytesWritten))
				throw new InvalidOperationException($"Unable to serialize object of type '{@object?.GetType().Name ?? "NULL"}'");
			return bytesWritten;
		}

		public static bool TrySerializeLE<TItem>(this IItemSerializer<TItem> serializer, TItem @object, out byte[] data)
			=> serializer.TrySerialize(@object, out data, Endianness.LittleEndian);

		public static bool TrySerialize<TItem>(this IItemSerializer<TItem> serializer, TItem @object, out byte[] data, Endianness endianness) {
			using var stream = new MemoryStream();
			using var writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), stream);
			if (!serializer.TrySerialize(@object, writer, out _)) {
				data = null;
				return false;
			}
			stream.Flush();
			data = stream.ToArray();
			return true;
		}
		
		public static bool TryDeserializeLE<TItem>(this IItemSerializer<TItem> serializer, ReadOnlySpan<byte> bytes, out TItem item)
			=> serializer.TryDeserialize(bytes, out item, Endianness.LittleEndian);

		public static bool TryDeserialize<TItem>(this IItemSerializer<TItem> serializer, ReadOnlySpan<byte> bytes, out TItem item, Endianness endianness) {
			using var stream = new MemoryStream(bytes.ToArray());
			using var reader = new EndianBinaryReader(EndianBitConverter.For(endianness), stream);
			return serializer.TryDeserialize(bytes.Length, reader, out item);
		}

		public static TItem Deserialize<TItem>(this IItemSerializer<TItem> serializer, int byteSize, EndianBinaryReader reader) {
			if (!serializer.TryDeserialize(byteSize, reader, out var item))
				throw new InvalidOperationException($"Unable to deserialize object of size {byteSize}b");
			return item;
		}

		public static TItem DeserializeLE<TItem>(this IItemSerializer<TItem> serializer, ReadOnlySpan<byte> bytes)
			=> serializer.Deserialize(bytes, Endianness.LittleEndian);


		public static TItem Deserialize<TItem>(this IItemSerializer<TItem> serializer, ReadOnlySpan<byte> bytes, Endianness endianness) {
			if (!serializer.TryDeserialize(bytes, out var item, endianness))
				throw new InvalidOperationException($"Unable to little-endian deserialize object of type '{item?.GetType().Name ?? "NULL"}'");
			return item;
		}


		public static IItemSerializer<TBase> AsBaseSerializer<TItem, TBase>(this IItemSerializer<TItem> serializer) where TItem : TBase
			=> new CastedSerializer<TBase, TItem>(serializer);
	}
}