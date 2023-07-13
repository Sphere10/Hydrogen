using System;

namespace Hydrogen;

public class SizeDescriptorSerializer : SizeDescriptorSizer, IItemSerializer<long> {

	public SizeDescriptorSerializer() : this(SizeDescriptorStrategy.UseVarInt) {
	}

	public SizeDescriptorSerializer(SizeDescriptorStrategy sizeDescriptorStrategy) : base(sizeDescriptorStrategy) {
	}


	public bool TrySerialize(long item, EndianBinaryWriter writer, out long bytesWritten) {
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
		;
		bytesWritten = writer.BaseStream.Position - startPos;
		return true;
	}

	public bool TryDeserialize(long byteSize, EndianBinaryReader reader, out long item) {
		var startPos = reader.BaseStream.Position;

		if (!TryDeserialize(reader, out item))
			return false;

		var bytesRead = reader.BaseStream.Position - startPos;
		Guard.Ensure(bytesRead == byteSize, $"Inconsistent read. Expected to read {byteSize} bytes, actual {bytesRead}");
		return true;
	}

	public bool TryDeserialize(EndianBinaryReader reader, out long item) {
		item = SizeDescriptorStrategy switch {
			SizeDescriptorStrategy.UseVarInt => unchecked((long)VarInt.Read(reader.BaseStream)),
			SizeDescriptorStrategy.UseCVarInt => unchecked((long)CVarInt.Read(reader.BaseStream)),
			SizeDescriptorStrategy.UseULong => reader.ReadInt64(),
			SizeDescriptorStrategy.UseUInt32 => reader.ReadUInt32(),
			SizeDescriptorStrategy.UseUInt16 => reader.ReadUInt16(),
			SizeDescriptorStrategy.UseByte => reader.ReadByte(),
			_ => throw new ArgumentOutOfRangeException()
		};
		return true;
	}
}
