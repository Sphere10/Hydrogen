//-----------------------------------------------------------------------
// <copyright file="EnumerableContainsSameComparer.cs" company="Sphere 10 Software">
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

	public class EnumerableContainsSameComparer<T> : IEqualityComparer<IEnumerable<T>> {
        public bool Equals(IEnumerable<T> x, IEnumerable<T> y) {
            var xArray = x as T[] ?? x.ToArray();
            var yArray = y as T[] ?? y.ToArray();
            return Object.ReferenceEquals(x, y) || (x != null && y != null && xArray.ContainsAll(yArray) && yArray.ContainsAll(xArray));
        }

        public int GetHashCode(IEnumerable<T> obj) {
            if (obj == null)
                return 0;

            return unchecked(obj.Select(e => e.GetHashCode()).Aggregate(0, (a, b) => (((23 * 37) + a) * 37) + b)); 
        }
    }
}
