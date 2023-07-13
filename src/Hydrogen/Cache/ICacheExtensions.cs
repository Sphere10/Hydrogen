// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public static class ICacheExtensions {

	public static IEnumerable<V> GetAllCachedValues<K, V>(this ICache<K, V> cache) {
		if (cache is BulkFetchCacheBase<K, V> bulkFetchCache && bulkFetchCache.FetchCount == 0) {
			bulkFetchCache.ForceRefresh();
		}
		using (cache.EnterReadScope()) {
			return cache.CachedItems.Select(c => c.Value).ToArray();
		}
	}

}
