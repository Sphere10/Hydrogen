using System.IO;
using System.Linq;

namespace Sphere10.Framework {
	public class VarInt {

		private readonly ulong _value;
		
		private static readonly EndianBitConverter BitConverter = EndianBitConverter.Little;
		
		public VarInt(ulong value) {
			_value = value;
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

			if (_value < 0xFD)
				builder.Append((byte)_value);
			else if (_value <= 0xFFFF) {
				builder.Append(0xFD);
				builder.Append(BitConverter.GetBytes((ushort)_value));
			} 
			else if (_value <= 0xFFFFFFFF) {
				builder.Append(0xFE);
				builder.Append(BitConverter.GetBytes((uint)_value));
			} else {
				builder.Append(0xFF);
				builder.Append(BitConverter.GetBytes(_value));
			}

			return builder.ToArray();
		}

		public static implicit operator ulong(VarInt v) => v._value;
		
		public ulong ToLong() => _value;
	}
}
