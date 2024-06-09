// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class ByteArraySerializer : ItemSerializerBase<byte[]> {
	private readonly SizeDescriptorSerializer _sizeSerializer;

	public ByteArraySerializer(SizeDescriptorStrategy sizeDescriptorStrategy = SizeDescriptorStrategy.UseCVarInt) {
		_sizeSerializer = new SizeDescriptorSerializer(sizeDescriptorStrategy);
	}

	public static ByteArraySerializer Instance { get; } = new();

	public override long CalculateSize(SerializationContext context, byte[] item) 
		=> _sizeSerializer.CalculateSize(context, item.Length) + item.Length;

	public override void Serialize(byte[] item, EndianBinaryWriter writer, SerializationContext context) {
		_sizeSerializer.Serialize(item.Length, writer, context);
		writer.Write(item);
	}

	public override byte[] Deserialize(EndianBinaryReader reader, SerializationContext context) {
		var byteSize = _sizeSerializer.Deserialize(reader, context);
		 return reader.ReadBytes(byteSize);
	}

}
