//-----------------------------------------------------------------------
// <copyright file="EnumerableSequenceEqualComparer.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Collections {

	public class ArrayEqualityComparer<T> : IEqualityComparer<T[]> {
		private readonly IEqualityComparer<T> _elementComparer;

		public ArrayEqualityComparer() {
			_elementComparer = EqualityComparer<T>.Default;
		}

		public ArrayEqualityComparer(IEqualityComparer<T> comparer) {
			_elementComparer = comparer;
		}

		public bool Equals(T[] x, T[] y) {
            return Object.ReferenceEquals(x, y) || (x != null && y != null && x.SequenceEqual(y, _elementComparer));
        }

        public int GetHashCode(T[] obj) {
            if (obj == null)
                return 0;

            return unchecked(obj.Select(e => e.GetHashCode()).Aggregate(0, (a, b) => (((23 * 37) + a) * 37) + b)); 
        }
    }
}
