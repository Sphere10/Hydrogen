// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen {

	public sealed class PreloadedCache<TKey, TValue> : BulkFetchCacheBase<TKey, TValue> {
		private readonly IDictionary<TKey, TValue> _preloadedValues = null;
		public PreloadedCache(
            IDictionary<TKey,TValue> preloadedValues,
            IEqualityComparer<TKey> keyComparer = null
        ) : base (ExpirationPolicy.None, null, true, NullValuePolicy.CacheNormally, StaleValuePolicy.AssumeNeverStale, keyComparer) {
			_preloadedValues = preloadedValues;
		}

		protected override IDictionary<TKey, TValue> BulkFetch() {
		        return _preloadedValues;
		}

		protected override bool CheckStaleness(TKey key, CachedItem<TValue> item) => false;
	}
}
