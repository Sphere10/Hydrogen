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

namespace Hydrogen {

    /// <summary>
    /// Extension methods for all types.
    /// </summary>
    /// <remarks></remarks>
    public static class UniversalExtensions {
        public static void Swap<T>(ref T fromX, ref T fromY) {
            T temp = default(T);
            temp = fromX;
            fromX = fromY;
            fromY = temp;
        }

        public static bool IsIn<T>(this T @object, params T[] collection) {
            return collection.Contains(@object);
        }

        public static bool IsIn<T>(this T @object, IEnumerable<T> collection) {
            return collection.Contains(@object);
        }

		public static string ToStringSafe<T>(this T @object) => @object?.ToString() ?? "<null>";


		public static IEnumerable<T> ConcatWith<T>(this T head, IEnumerable<T> tail) => new [] { head }.Concat(tail);

		public static IEnumerable<T> UnionWith<T>(this T head, IEnumerable<T> tail) => new [] { head }.Union(tail);

		public static IEnumerable<T> UnionWith<T>(this T head, T tail) => UnionWith(head, new [] { tail });

    }
}
