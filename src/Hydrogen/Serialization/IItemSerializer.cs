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

	internal void Serialize(object item, EndianBinaryWriter writer);

	internal object Deserialize(EndianBinaryReader reader);
	
}

public interface IItemSerializer<TItem> : IItemSizer<TItem>, IItemSerializer {

	public new void Serialize(TItem item, EndianBinaryWriter writer);

	public new TItem Deserialize(EndianBinaryReader reader);

	void IItemSerializer.Serialize(object item, EndianBinaryWriter writer)
		=> Serialize((TItem)item, writer);

	object IItemSerializer.Deserialize(EndianBinaryReader reader)
		=> Deserialize(reader);
}

public static class IItemSerializerExtensions {

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static long SerializeReturnSize(this IItemSerializer serializer, object item, EndianBinaryWriter writer) {
		var startPos = writer.BaseStream.Position;
		serializer.Serialize(item, writer);
		return writer.BaseStream.Position - startPos;

		// NOTE: if a malicious serializer writes more than it says, and rewinds stream Position
		// to hide its hidden data, any subsequent serializations will overwrite that hidden data.
		// Thus there is no attack vector of meaningful consequence here. Attempting to write
		// bloated data is responsibility of underlying Stream itself and will not result in security
		// vulnerability.
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] SerializeBytesLE<TItem>(this IItemSerializer<TItem> serializer, TItem item)
		=> serializer.SerializeToBytes(item, Endianness.LittleEndian);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static byte[] SerializeToBytes<TItem>(this IItemSerializer<TItem> serializer, TItem item, Endianness endianness) {
		using var stream = new MemoryStream();
		using var writer = new EndianBinaryWriter(EndianBitConverter.For(endianness), stream);
		serializer.Serialize(item, writer);
		stream.Flush();
		return stream.ToArray();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TItem DeserializeBytesLE<TItem>(this IItemSerializer<TItem> serializer, ReadOnlySpan<byte> bytes)
		=> serializer.DeserializeBytes(bytes, Endianness.LittleEndian);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TItem DeserializeBytesLE<TItem>(this IItemSerializer<TItem> serializer, byte[] bytes)
		=> serializer.DeserializeBytes(bytes, Endianness.LittleEndian);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TItem DeserializeBytes<TItem>(this IItemSerializer<TItem> serializer, ReadOnlySpan<byte> bytes, Endianness endianness)
		=> serializer.DeserializeBytes(bytes.ToArray(), endianness);  // TODO: need fast way to deal with deserializing spans
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TItem DeserializeBytes<TItem>(this IItemSerializer<TItem> serializer, byte[] bytes, Endianness endianness)  {
		using var stream = new MemoryStream(bytes);
		using var reader = new EndianBinaryReader(EndianBitConverter.For(endianness), stream);
		return serializer.Deserialize(reader);
	}
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static IItemSerializer<TBase> AsBaseSerializer<TItem, TBase>(this IItemSerializer<TItem> serializer) where TItem : TBase
		=> new CastedSerializer<TBase>(serializer);

}
