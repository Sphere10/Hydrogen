// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Newtonsoft.Json;

namespace Hydrogen.Communications;

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
