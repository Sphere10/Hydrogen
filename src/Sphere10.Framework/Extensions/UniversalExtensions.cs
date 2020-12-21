//-----------------------------------------------------------------------
// <copyright file="UniversalExtensions.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

	/// <summary>
	/// Extension methods for all types.
	/// </summary>
	/// <remarks></remarks>
	public static class UniversalExtensions
	{
		public static void Swap<T>(ref T fromX, ref T fromY) {
			T temp = default(T);
			temp = fromX;
			fromX = fromY;
			fromY = temp;
		}

		public static string ToXml<T>(this T source) {
			return Tools.Xml.WriteToString(source);
		}

		public static T FromXml<T>(this string source) {
			return Tools.Xml.ReadFromString<T>(source);
		}

		public static bool IsIn<T>(this T source, params T[] collection) {
			return collection.Contains(source);
		}
		public static bool IsIn<T>(this T source, IEnumerable<T> collection) {
			return collection.Contains(source);
		}

        public static TRet SafeOrDefault<T, TRet>(this T obj, Func<T, TRet> getter) where T : class {
            return obj != null ? getter(obj) : default(TRet);
        }

        public static TRet SafeOrNull<T, TRet>(this T obj, Func<T, TRet> getter) where T : class where TRet : class {
            return obj != null ? getter(obj) : null;
        }

        public static TRet? SafeOrNullValue<T, TRet>(this T obj, Func<T, TRet> getter)
            where T : class
            where TRet : struct {
            return obj != null ? getter(obj) : new TRet?();
        }

        public static void SafeCall<T>(this T obj, Action<T> action) where T : class {
            if (obj != null)
                action(obj);
        }

		public static T[] Repeat<T>(this T source, int occurances) where T : ICloneable {
			var list = new List<T>();
			for (int i = 0; i < occurances; i++) {
				list.Add((T)source.Clone());
			}
			return list.ToArray();
		}

		}
}
