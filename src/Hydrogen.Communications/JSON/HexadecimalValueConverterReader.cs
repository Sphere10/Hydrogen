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

//Write hexadecimal values. Read hex values, string contaning hex values, integer values, string containing integer values
public sealed class HexadecimalValueConverterReader : JsonConverter {
	public override bool CanConvert(Type objectType) {
		return typeof(uint).Equals(objectType) || typeof(UInt64).Equals(objectType) || typeof(int).Equals(objectType) || typeof(Int64).Equals(objectType);
	}


	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
		//NOTE: Hex writing is commented until C++ support reading hex int (ref: jsoncpp does'nt)
		//var str = $"0x{value:x}";
		var str = $"{value}";
		writer.WriteRawValue(str);
	}
	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
		string str;
		if (reader.Value is not string)
			str = reader.Value.ToString();
		else
			str = (string)reader.Value;

		if (str == null)
			throw new JsonSerializationException();
		var baseSpace = 16;
		if (!str.StartsWith("0x"))
			baseSpace = 10;
		if (objectType == typeof(UInt64))
			return Convert.ToUInt64(str, baseSpace);
		else if (objectType == typeof(Int64))
			return Convert.ToInt64(str, baseSpace);
		if (objectType == typeof(uint) || objectType == typeof(UInt32))
			return Convert.ToUInt32(str, baseSpace);
		return Convert.ToInt32(str, baseSpace);
	}
}
