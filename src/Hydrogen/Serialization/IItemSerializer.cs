// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Hydrogen;

public interface IItemSerializer : IItemSizer {

	internal void SerializeInternal(object item, EndianBinaryWriter writer);

	internal object DeserializeInternal(long byteSize, EndianBinaryReader reader);
}

public interface IItemSerializer<TItem> : IItemSizer<TItem>, IItemSerializer {

	internal new void SerializeInternal(TItem item, EndianBinaryWriter writer);

	internal new TItem DeserializeInternal(long byteSize, EndianBinaryReader reader);

	void IItemSerializer.SerializeInternal(object item, EndianBinaryWriter writer)
		=> SerializeInternal((TItem)item, writer);

	object IItemSerializer.DeserializeInternal(long byteSize, EndianBinaryReader reader)
		=> DeserializeInternal(byteSize, reader);
}

public static class IItemSerializerExtensions {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long Serialize<TItem>(this IItemSerializer<TItem> serializer, TItem item, EndianBinaryWriter writer) {
		var startPos = writer.BaseStream.Position;
		serializer.SerializeInternal(item, writer);
		return writer.BaseStream.Position - startPos;

		// NOTE: if a malicious serializer writes more than it says, and rewinds stream Position
		// to hide its hidden data, any subsequent serializations will overwrite that hidden data.
		// Thus there is no attack vector of meaningful consequence here. Attempting to write
		// bloated data is responsibility of underlying Stream itself and will not result in security
		// vulnerability.
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TItem Deserialize<TItem>(this IItemSerializer<TItem> serializer, long byteSize, EndianBinaryReader reader) {
		var startPos = reader.BaseStream.Position;
		var item = serializer.DeserializeInternal(byteSize, reader);
		Guard.Ensure(reader.BaseStream.Position - startPos == byteSize, "Read overflow");
		return item;

		// NOTE: if a malicious serializer reads more than it says, and rewinds stream Position
		// to hide its hidden data, any subsequent reads will overwrite that hidden data.
		// At most attacking serializer can read ahead of stream.
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] SerializeLE<TItem>(this IItemSerializer<TItem> serializer, TItem item)
		=> serializer.Serialize(item, Endianness.LittleEndian);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] Serialize<TItem>(this IItemSerializer<TItem> serializer, TItem item, Endianness endianness) {
		using var stream = new MemoryStream();
		using var writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), stream);
		serializer.Serialize(item, writer);
		stream.Flush();
		return stream.ToArray();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TItem DeserializeLE<TItem>(this IItemSerializer<TItem> serializer, ReadOnlySpan<byte> bytes)
		=> serializer.Deserialize(bytes, Endianness.LittleEndian);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TItem DeserializeLE<TItem>(this IItemSerializer<TItem> serializer, byte[] bytes)
		=> serializer.Deserialize(bytes, Endianness.LittleEndian);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TItem Deserialize<TItem>(this IItemSerializer<TItem> serializer, ReadOnlySpan<byte> bytes, Endianness endianness)
		=> serializer.Deserialize(bytes.ToArray(), endianness);  // TODO: need fast way to deal with deserializing spans
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TItem Deserialize<TItem>(this IItemSerializer<TItem> serializer, byte[] bytes, Endianness endianness)  {
		using var stream = new MemoryStream(bytes);
		using var reader = new EndianBinaryReader(EndianBitConverter.For(endianness), stream);
		return serializer.Deserialize(bytes.Length, reader);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TrySerializeLE<TItem>(this IItemSerializer<TItem> serializer, TItem item, out byte[] data, out Exception error) 
		=> TrySerialize(serializer, item, Endianness.LittleEndian, out data, out error);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TrySerialize<TItem>(this IItemSerializer<TItem> serializer, TItem item, Endianness endianness, out byte[] data, out Exception error) {
		try {
			data = serializer.Serialize(item, endianness);
			error = null;
			return true;
		} catch (Exception ex) {
			data = default;
			error = ex;
			return false;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryDeserializeLE<TItem>(this IItemSerializer<TItem> serializer, ReadOnlySpan<byte> bytes, out TItem item, out Exception error)
		=> serializer.TryDeserialize(bytes, out item, Endianness.LittleEndian, out error);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryDeserializeLE<TItem>(this IItemSerializer<TItem> serializer, byte[] bytes, out TItem item, out Exception error)
		=> serializer.TryDeserialize(bytes, out item, Endianness.LittleEndian, out error);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryDeserialize<TItem>(this IItemSerializer<TItem> serializer, ReadOnlySpan<byte> bytes, out TItem item, Endianness endianness, out Exception error)
		=> TryDeserialize(serializer, bytes.ToArray(), out item, endianness, out error);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryDeserialize<TItem>(this IItemSerializer<TItem> serializer, byte[] bytes, out TItem item, Endianness endianness, out Exception error) {
		try {
			item = serializer.Deserialize(bytes, endianness);
			error = null;
			return true;
		} catch (Exception ex) {
			item = default;
			error = ex;
			return false;
		}
	}

	public static IItemSerializer<TBase> AsBaseSerializer<TItem, TBase>(this IItemSerializer<TItem> serializer) where TItem : TBase
		=> new ProjectedSerializer<TItem,TBase>(serializer, x => (TBase)x, x => (TItem)x);

	public static IItemSerializer<TItem> AsConstantSizeSerializer<TItem>(this IItemSerializer<TItem> serializer, long staticSize, SizeDescriptorStrategy sizeDescriptorStrategy)
		=> new PaddedSerializer<TItem>(staticSize, serializer, sizeDescriptorStrategy);

}
