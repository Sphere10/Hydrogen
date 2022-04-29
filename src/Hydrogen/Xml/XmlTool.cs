//-----------------------------------------------------------------------
// <copyright file="Tools.Xml.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Xml.Serialization;
using System.Text;
using Hydrogen;

namespace Tools {
	/// <summary>
	/// Provides XML object-to-xml and xml-to-object serialization services.
	/// This class simplifies already existing .NET functionality and uses
	/// caching for performance.
	/// </summary>
	/// <remarks>
	/// 		Alternative to deep serialization is to use DataContractSerializer
	///	 public static class SerializationExtensions {
	///    public static string Serialize<T>(this T obj) {
	///        var serializer = new DataContractSerializer(obj.GetType());
	///        using (var writer = new StringWriter())
	///        using (var stm = new XmlTextWriter(writer)) {
	///            serializer.WriteObject(stm, obj);
	///            return writer.ToString();
	///        }
	///    }
	///    public static T Deserialize<T>(this string serialized) {
	///        var serializer = new DataContractSerializer(typeof(T));
	///        using (var reader = new StringReader(serialized))
	///        using (var stm = new XmlTextReader(reader)) {
	///            return (T)serializer.ReadObject(stm);
	///        }
	///    }
	///}
	/// </remarks>
	public class Xml {
		public static readonly ICache<Type, XmlSerializer> CachedSerializers;


		static Xml() {
			CachedSerializers = new ActionCache<Type, XmlSerializer>(
				valueFetcher:
					(type) => new XmlSerializer(type)
			);
		}

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

		/// <summary>
		/// Reads the specified type from an XML file.
		/// </summary>
		/// <typeparam name="T">The object type to serialize.</typeparam>
		/// <param name="filename"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static T ReadFromFile<T>(string filename, Encoding encoding) {
			using (var stream = new FileStream(filename, FileMode.Open)) {
				using (var reader = new StreamReader(stream, encoding)) {
					return Read<T>(reader);
				}
			}
		}


		/// <summary>
		/// Reads the specified type from an XML file.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static object ReadFromFile(Type type, string filename) {
			return ReadFromFile(type, filename, Encoding.Unicode);
		}

		/// <summary>
		/// Reads the specified type from an XML file.
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static object ReadFromFile(Type type, string filename, Encoding encoding) {
			using (var stream = new FileStream(filename, FileMode.Open)) {
				using (var reader = new StreamReader(stream, encoding)) {
					return Read(type, reader);
				}
			}
		}

		/// <summary>
		/// Reads the specified type in XML form from a text reader.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static T Read<T>(TextReader reader) {
			//return (T)GetXmlSerializer(typeof(T)).Deserialize(reader);
			return (T)Read(typeof(T), reader);
		}

		public static object Read(Type type, TextReader reader) {
			return CachedSerializers[type].Deserialize(reader);
		}

		public static string WriteToString<T>(T obj) {
			string retval = string.Empty;
			using (TextWriter writer = new StringWriter()) {
				Write(obj, Encoding.Unicode, writer);
				retval = writer.ToString();
			}
			return retval;
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
			CachedSerializers[obj.GetType()].Serialize(writer, obj);
		}


		public static string DeepWriteToString<T>(T obj) {
			var deepSerializer = new XmlDeepSerializer();
			var stringBuilder = new StringBuilder();
			deepSerializer.Serialize(obj, new StringWriter(stringBuilder));
			return stringBuilder.ToString();
		}


		public static void DeepWriteToFile<T>(string filename, T obj) {
			var deepSerializer = new XmlDeepSerializer();
			deepSerializer.Serialize(obj, filename);
		}

		public static T DeepReadFromFile<T>(string filename) {
			var deepDeserializer = new XmlDeepDeserializer();
			return (T)deepDeserializer.Deserialize(filename);
		}

	}
}
