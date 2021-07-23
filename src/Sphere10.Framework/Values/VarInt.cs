using System.IO;

namespace Sphere10.Framework {
	
	/// <summary>
	/// Variable-sized integer. A given ulong when converted to bytes, will take only the required
	/// number of bytes plus a header. Values less than 0xFC will require only a single byte and no header. Use when
	/// serializing variable ulong values.
	/// </summary>
	public readonly struct VarInt {

		private readonly ulong _value;
		
		private static readonly EndianBitConverter BitConverter = EndianBitConverter.Little;
		
		public VarInt(ulong value) {
			_value = value;
		}
		public VarInt(byte[] bytes) {
			Guard.ArgumentNotNull(bytes, nameof(bytes));
			
			if (bytes.Length == 1)
				_value = bytes[0];
			else {
				var prefix = bytes[0];
				if (prefix == 0xFD)
					_value = BitConverter.ToUInt16(bytes[1..]);
				else if (prefix == 0xFE)
					_value = BitConverter.ToUInt32(bytes[1..]);
				else {
					_value = BitConverter.ToUInt64(bytes[1..]);
				}
			}
		}

		/// <summary>
		/// Read a <see cref="VarInt"/> value from a stream.
		/// </summary>
		/// <param name="stream"> a stream</param>
		/// <returns> new var int with value from the stream</returns>
		public static VarInt FromStream(Stream stream) {
			Guard.ArgumentNotNull(stream, nameof(stream));
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

		/// <summary>
		/// Encodes the current value into a byte array. 
		/// </summary>
		/// <returns> varint as bytes</returns>
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
		
		/// <summary>
		/// Returns varint as ulong.
		/// </summary>
		/// <returns></returns>
		public ulong ToLong() => _value;
	}
}
