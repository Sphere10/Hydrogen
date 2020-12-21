using System.Text;

namespace Sphere10.Framework {

	public class StringSerializer : StringSizer, IObjectSerializer<string> {
		public StringSerializer(Encoding encoding) 
			: base(encoding) {
		}

		public int Serialize(string @object, EndianBinaryWriter writer) {
			var stringBytes = Encoding.GetBytes(@object);
			writer.Write(stringBytes);
			return stringBytes.Length;
		}

		public string Deserialize(int size, EndianBinaryReader reader) {
			var stringBytes = reader.ReadBytes(size);
			return Encoding.GetString(stringBytes);
		}
	}

}