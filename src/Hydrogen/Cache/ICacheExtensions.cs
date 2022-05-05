//-----------------------------------------------------------------------
// <copyright file="ICacheExtensions.cs" company="Sphere 10 Software">
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

using System.Collections.Generic;
using System.Linq;

namespace Hydrogen {
	public static class ICacheExtensions {

		public static IEnumerable<V> GetAllCachedValues<K, V>(this ICache<K, V> cache) {
			using (cache.EnterReadScope()) {
				return cache.CachedItems.Select(c => c.Value).ToArray();
			}
		}

    }
}
