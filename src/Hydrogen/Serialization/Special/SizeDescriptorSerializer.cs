// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class SizeDescriptorSerializer : SizeDescriptorSizer, IItemSerializer<long> {

	public SizeDescriptorSerializer() : this(SizeDescriptorStrategy.UseCVarInt) {
	}

	public SizeDescriptorSerializer(SizeDescriptorStrategy sizeDescriptorStrategy) : base(sizeDescriptorStrategy) {
	}

	public bool SupportsNull => false;

	public void Serialize(long item, EndianBinaryWriter writer, SerializationContext context) {
		var startPos = writer.BaseStream.Position;
		switch (SizeDescriptorStrategy) {
			case SizeDescriptorStrategy.UseVarInt:
				VarInt.Write(unchecked((ulong)item), writer.BaseStream);
				break;
			case SizeDescriptorStrategy.UseCVarInt:
				CVarInt.Write(unchecked((ulong)item), writer.BaseStream);
				break;
			case SizeDescriptorStrategy.UseULong:
				writer.Write(item);
				break;
			case SizeDescriptorStrategy.UseUInt32:
				if (item > uint.MaxValue)
					throw new ArgumentOutOfRangeException(nameof(item), $"Size descriptor for {item} is too large to be serialized as a UInt32");
				writer.Write((uint)item);
				break;
			case SizeDescriptorStrategy.UseUInt16:
				if (item > ushort.MaxValue)
					throw new ArgumentOutOfRangeException(nameof(item), $"Size descriptor for {item} is too large to be serialized as a UInt16");
				writer.Write((ushort)item);
				break;
			case SizeDescriptorStrategy.UseByte:
				if (item > byte.MaxValue)
					throw new ArgumentOutOfRangeException(nameof(item), $"Size descriptor for {item} is too large to be serialized as a Byte");
				writer.Write((byte)item);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}
	public long Deserialize(EndianBinaryReader reader, SerializationContext context)
		=> SizeDescriptorStrategy switch {
			SizeDescriptorStrategy.UseVarInt => unchecked((long)VarInt.Read(reader.BaseStream)),
			SizeDescriptorStrategy.UseCVarInt => unchecked((long)CVarInt.Read(reader.BaseStream)),
			SizeDescriptorStrategy.UseULong => reader.ReadInt64(),
			SizeDescriptorStrategy.UseUInt32 => reader.ReadUInt32(),
			SizeDescriptorStrategy.UseUInt16 => reader.ReadUInt16(),
			SizeDescriptorStrategy.UseByte => reader.ReadByte(),
			_ => throw new ArgumentOutOfRangeException()
		};

}
