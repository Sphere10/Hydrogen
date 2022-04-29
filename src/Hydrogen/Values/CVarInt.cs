using System;
using System.IO;

namespace Hydrogen.Values {
	
	//Compact Variable integer - LEB128 compression based on NBitcoin CompactVarInt. Useful for 
	// serializing small integers in a single byte. Larger integers may also be encoded but will be 
	// less efficient than other formats.
	public readonly struct CVarInt  {
		private readonly ulong _value;
		
		public CVarInt(ulong value) {
			_value = value;
		}

		public CVarInt(byte[] bytes) {
			ulong n = 0;
			
			for (var i = 0; i < bytes.Length; i++) {
				var chData = bytes[i];
				var a = n << 7;
				var b = (byte)(chData & 0x7F);
				n = (a | b);
				if ((chData & 0x80) != 0)
					n++;
				else
					break;
			}
			
			_value = n;
		} 

		/// <summary>
		/// Read <see cref="CVarInt"/> from stream.
		/// </summary>
		/// <param name="size"> value size e.g. sizeof(uint)</param>
		/// <param name="stream"> stream to read value from</param>
		/// <returns> cvarint</returns>
		public static CVarInt Read(int size, Stream stream) {
			ulong n = 0;
			while (true)
			{
				var chData = (byte)stream.ReadByte();
				var a = n << 7;
				var b = (byte)(chData & 0x7F);
				n = (a | b);
				if ((chData & 0x80) != 0)
					n++;
				else
					break;
			}
			return new CVarInt(n);
		}

		/// <summary>
		/// write cvarint value as bytes to stream.
		/// </summary>
		/// <param name="stream"></param>
		public void Write(Stream stream) {
			var n = _value;
			Span<byte> tmp = stackalloc byte[(sizeof(long) * 8 + 6) / 7];

			var len = 0;
			while (true)
			{
				var a = (byte)(n & 0x7F);
				var b = (byte)(len != 0 ? 0x80 : 0x00);
				tmp[len] = (byte)(a | b);
				if (n <= 0x7F)
					break;
				n = (n >> 7) - 1;
				len++;
			}
			do
				stream.WriteByte(tmp[len]);
			while (len-- != 0);
		}

		/// <summary>
		/// encodes the cvarint value as bytes.
		/// </summary>
		/// <returns></returns>
		public byte[] ToBytes() {
			using var stream = new MemoryStream();
			Write(stream);
			return stream.ToArray();
		}
		
		public static implicit operator ulong(CVarInt v) => v._value;

		public static implicit operator CVarInt(ulong v) => new (v);
		
		public ulong ToLong() => _value;

		/// <summary>
		/// Returns the number of bytes the value will require when encoded as <see cref="CVarInt"/>
		/// </summary>
		/// <param name="value"></param>
		/// <returns> size in bytes</returns>
		public static int SizeOf(ulong value) {
			var len = 1;
			var n = value;
			while (n > 0x7F)
			{
				n = (n >> 7) - 1;
				len++;
			}

			return len;
		}
	}
}
