using System;
using System.Text;

namespace Sphere10.Framework {

	public class StringSerializer : StringSizer, IItemSerializer<string> {
		public StringSerializer(Encoding encoding)
			: base(encoding) {
		}
		
		public bool TrySerialize(string item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				var stringBytes = Encoding.GetBytes(item);
				writer.Write(stringBytes);
				bytesWritten = stringBytes.Length;
				
				return true;
			} catch (Exception) {
				bytesWritten = 0;
				return false;
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out string item) {
			try {
				var stringBytes = reader.ReadBytes(byteSize);
				item = Encoding.GetString(stringBytes);
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}
	}

}