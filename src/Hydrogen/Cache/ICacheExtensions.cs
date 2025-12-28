// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Convenience extensions for working with cache implementations.
/// </summary>
public static class ICacheExtensions {

	/// <summary>
	/// Returns all currently cached values, forcing an initial bulk fetch when applicable.
	/// </summary>
	public static IEnumerable<V> GetAllCachedValues<K, V>(this ICache<K, V> cache) {
		if (cache is BulkFetchCacheBase<K, V> bulkFetchCache && bulkFetchCache.FetchCount == 0) {
			bulkFetchCache.ForceRefresh();
		}
		using (cache.EnterReadScope()) {
			return cache.CachedItems.Select(c => c.Value).ToArray();
		}
	}

}
