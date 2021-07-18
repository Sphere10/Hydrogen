using System.IO;
using System.Linq;

namespace Sphere10.Framework {
	public class VarInt {
		
		public ulong Value { get; }
		
		private static readonly EndianBitConverter BitConverter = EndianBitConverter.Little;
		
		public VarInt(ulong value) {
			Value = value;
		}

		public static VarInt FromStream(Stream stream) {
			var reader = new EndianBinaryReader(BitConverter, stream);
			var prefix = stream.ReadByte();

			if (prefix < 0xFD)
				return new VarInt((byte)prefix);

			if (prefix == 0xFD) {
				return new VarInt(reader.ReadUInt16());
			}

			if (prefix == 0xFE)
				return new VarInt(reader.ReadUInt32());

			return new VarInt(reader.ReadUInt64());
		}

		public byte[] ToBytes() {

			var builder = new ByteArrayBuilder();

			if (Value < 0xFD)
				builder.Append((byte)Value);
			else if (Value <= 0xFFFF) {
				builder.Append(0xFD);
				builder.Append(BitConverter.GetBytes((ushort)Value));
			} 
			else if (Value <= 0xFFFFFFFF) {
				builder.Append(0xFE);
				builder.Append(BitConverter.GetBytes((uint)Value));
			} else {
				builder.Append(0xFF);
				builder.Append(BitConverter.GetBytes(Value));
			}

			return builder.ToArray();
		}

		public static implicit operator ulong(VarInt v) => v.Value;
	}
}
