using System;
using System.Numerics;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Hydrogen;
using Newtonsoft.Json;

namespace Hydrogen.Communications {
	public class ByteArrayHexConverter : JsonConverter<byte[]> {
		public override void WriteJson(JsonWriter writer, byte[] value, JsonSerializer serializer) {
			string hexStr = HexEncoding.Encode(value, false);
			writer.WriteValue(hexStr);
		}

		public override byte[] ReadJson(JsonReader reader, Type objectType, byte[] existingValue, bool hasExistingValue, JsonSerializer serializer) {
			string hexStr = (string)reader.Value;
			return ToByteArray(hexStr);
		}

		static public byte[] ToByteArray(String hexStr) {
			if (hexStr == null)
				return null;

			byte[] result = null;
			HexEncoding.TryDecode(hexStr, out result, false);
			return result;
		}
	}

}
