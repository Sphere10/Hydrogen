//-----------------------------------------------------------------------
// <copyright file="ActionEqualityComparer.cs" company="Sphere 10 Software">
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

namespace Hydrogen {
	public class ActionEqualityComparer<T> : IEqualityComparer<T> {
		private readonly Func<T, T, bool> _comparerFunc;

		public ActionEqualityComparer(Func<T, T, bool> comparerFunc) {
			_comparerFunc = comparerFunc;
		}

		public bool Equals(T x, T y) {
			return _comparerFunc(x, y);
		}

		public int GetHashCode(T obj) {
			return obj.GetHashCode();
		}

        public static ActionEqualityComparer<T> From(Func<T, T, bool> comparer) {
            return new ActionEqualityComparer<T>(comparer);
        }

	}
}
