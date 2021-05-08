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

namespace Sphere10.Framework {
	public static class ICacheExtensions {

        public static TVal Get<TKey, TVal>(this ICache<TKey, TVal> cache, TKey key) {
            return cache[key];
        }

        public static IEnumerable<V> GetAllCachedValues<K, V>(this ICache<K, V> cache) {
	        using (cache.EnterReadScope()) {
		        return cache.GetCachedItems().Values.Select(c => c.Value).ToArray();
	        }
        }

        public static void Set<K, V>(this ICache<K, V> cache, K key, V value) {
            cache.BulkLoad(new[] {new KeyValuePair<K, V>(key, value)});
        }
    }
}
