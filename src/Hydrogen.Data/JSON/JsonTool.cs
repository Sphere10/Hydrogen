// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Tools;

public static class Json {
	public static T ReadFromString<T>(string stringValue) {
		using (TextReader reader = new StringReader(stringValue)) {
			return Read<T>(reader);
		}
	}

	public static object ReadFromString(Type type, string stringValue) {
		using (TextReader reader = new StringReader(stringValue)) {
			return Read(type, reader);
		}
	}

	public static T ReadFromFile<T>(string filename) {
		return ReadFromFile<T>(filename, Encoding.Unicode);
	}

	public static T ReadFromFile<T>(string filename, Encoding encoding) {
		using (var stream = new FileStream(filename, FileMode.Open)) {
			using (var reader = new StreamReader(stream, encoding)) {
				return Read<T>(reader);
			}
		}
	}

	public static object ReadFromFile(Type type, string filename) {
		return ReadFromFile(type, filename, Encoding.Unicode);
	}

	public static object ReadFromFile(Type type, string filename, Encoding encoding) {
		using (var stream = new FileStream(filename, FileMode.Open)) {
			using (var reader = new StreamReader(stream, encoding)) {
				return Read(type, reader);
			}
		}
	}

	public static T Read<T>(TextReader reader) {
		return (T)Read(typeof(T), reader);
	}

	public static object Read(Type type, TextReader reader) {
		return new JsonSerializer().Deserialize(reader, type);
	}

	public static string WriteToString<T>(T obj) {
		using TextWriter writer = new StringWriter();
		Write(obj, Encoding.Unicode, writer);
		return writer.ToString();
	}

	public static void WriteToFile<T>(string filename, T obj, FileMode fileMode = FileMode.Create) {
		WriteToFile<T>(filename, obj, Encoding.UTF8, fileMode);
	}

	public static void WriteToFile<T>(string filename, T obj, Encoding encoding, FileMode fileMode = FileMode.Create) {
		using (var stream = new FileStream(filename, fileMode)) {
			using (var writer = new StreamWriter(stream, encoding)) {
				Write(obj, encoding, writer);
				writer.Close();
			}
		}
	}

	public static void Write<T>(T obj, Encoding encoding, TextWriter writer) {
		var jsonSerializer = new JsonSerializer();
		using var jsonWriter = new JsonTextWriter(writer);
		jsonWriter.Formatting = Formatting.Indented;
		jsonSerializer.Serialize(jsonWriter, obj);
	}

}
