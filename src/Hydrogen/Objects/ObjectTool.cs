// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

#define USE_FAST_REFLECTION
#if __IOS__
#undef USE_FAST_REFLECTION
#endif

using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Hydrogen;

#if USE_FAST_REFLECTION
#endif

// ReSharper disable CheckNamespace
namespace Tools {

	public static class Object {

		public static object Create(string typeName, params object[] args) {
			return TypeActivator.Activate(typeName, args);
		}

		public static object Create(Type targetType, params object[] args) {
			return TypeActivator.Activate(targetType, args);
		}

		public static Type ResolveType(string fullName) {
			return TypeResolver.Resolve(fullName);
		}

		public static object ChangeType(object value, Type targetType) {
			return TypeChanger.ChangeType(value, targetType);
		}

		public static T ChangeType<T>(object value) {
			return TypeChanger.ChangeType<T>(value);
		}

		public static object SanitizeObject(object obj) {
			return TypeChanger.SanitizeObject(obj);
		}

		public static T Clone<T>(T obj, bool deepClone = false, IEnumerable<Type> dontClone = null) {
			return (T)CloneObject(obj, deepClone, dontClone);
		}

		public static object CloneObject(object obj, bool deepClone = false, IEnumerable<Type> dontClone = null) {
			IObjectCloner cloner;
			cloner = deepClone ? (IObjectCloner)new DeepObjectCloner(dontClone) : new ShallowObjectCloner();
			return cloner.Clone(obj);
		}

		public static void CopyMembers(object source, object dest, bool deepCopy = false) {
			IObjectCloner cloner;
			cloner = deepCopy ? (IObjectCloner)new DeepObjectCloner() : new ShallowObjectCloner();
			cloner.Copy(source, dest);
		}

		public static bool Compare(object obj1, object obj2) {
			var comparer = new DeepObjectComparer();
			return comparer.Equals(obj1, obj2);
		}

		public static void DecryptMembers(object obj) {
			ObjectEncryptor.DecryptMembers(obj);
		}

		public static void EncryptMembers(object obj) {
			ObjectEncryptor.EncryptMembers(obj);
		}

		//public static int CombineHashCodes(int hashCode1, int hashCode2) {
		//    unchecked {
		//        var hash = 17;
		//        hash = hash * 31 + hashCode1;
		//        hash = hash * 31 + hashCode2;
		//        return hash;
		//    }
		//}

		public static void SetDefaultValues(object obj) {
			ObjectWithDefaultValues.SetDefaults(obj);
		}

		/// <summary>
		/// Converts an object to a byte array
		/// </summary>
		/// <param name="obj">The object to be converted</param>
		/// <returns>A byte array that contains the converted object</returns>
		public static byte[] SerializeToByteArray(object obj) {
			if (obj == null) {
				return new byte[0];
			}

			using (var stream = new MemoryStream()) {
				var formatter = new BinarySerializer();
				formatter.Serialize(stream, obj);
				var bytes = stream.ToArray();
				stream.Flush();
				stream.Close();
				return bytes;
			}
		}

		/// <summary>
		/// Converts a byte array to an object
		/// </summary>
		/// <param name="bytes">The array of bytes to be converted</param>
		/// <returns>An object that represents the byte array</returns>
		public static object DeserializeFromByteArray(byte[] bytes) {
			if (bytes.Length == 0) {
				return null;
			}

			using (var stream = new MemoryStream(bytes)) {
				stream.Position = 0;
				var formatter = new BinarySerializer();
				var obj = formatter.Deserialize(stream);
				stream.Close();
				return obj;
			}
		}

		public static string ToSQLString(object obj) {
			if (obj == null || obj == DBNull.Value)
				return "NULL";

			var value = string.Empty;
			TypeSwitch.For(obj,
				TypeSwitch.Case<string>(s => value = string.Format("'{0}'", s.EscapeSQL())),
				TypeSwitch.Case<Guid>(g => value = "'" + g.ToString().ToUpper() + "'"),
				TypeSwitch.Case<char[]>(s => value = string.Format("'{0}'", s.ToString())),
				TypeSwitch.Case<DateTime>(d => value = string.Format("'{0:yyyy-MM-dd HH:mm:ss.fff}'", d)),
				TypeSwitch.Case<byte[]>(b => value = string.Format("'{0}'", b.ToHexString())),
				TypeSwitch.Case<char>(x => value = string.Format("'{0}'", x)),
				TypeSwitch.Case<bool>(b => value = string.Format("{0}", b ? 1 : 0)),
				TypeSwitch.Case<byte>(x => value = string.Format("{0}", x)),
				TypeSwitch.Case<sbyte>(x => value = string.Format("{0}", x)),
				TypeSwitch.Case<short>(x => value = string.Format("{0}", x)),
				TypeSwitch.Case<ushort>(x => value = string.Format("{0}", x)),
				TypeSwitch.Case<int>(x => value = string.Format("{0}", x)),
				TypeSwitch.Case<uint>(x => value = string.Format("{0}", x)),
				TypeSwitch.Case<long>(x => value = string.Format("{0}", x)),
				TypeSwitch.Case<ulong>(x => value = string.Format("{0}", x)),
				TypeSwitch.Case<float>(x => value = string.Format("{0}", x)),
				TypeSwitch.Case<double>(x => value = string.Format("{0}", x)),
				TypeSwitch.Case<decimal>(x => value = string.Format("{0}", x)),
				TypeSwitch.Default(() => value = string.Format("'{0}'", value))
			);
			return value;
		}

	}
}