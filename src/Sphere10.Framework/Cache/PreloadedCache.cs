//-----------------------------------------------------------------------
// <copyright file="PreloadedCache.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework {

	public sealed class PreloadedCache<TKey, TValue> : BulkFetchCacheBase<TKey, TValue> {
		private readonly IDictionary<TKey, TValue> _preloadedValues = null;
		public PreloadedCache(
            IDictionary<TKey,TValue> preloadedValues,
            IEqualityComparer<TKey> keyComparer = null
        ) : base (ExpirationPolicy.None, null, true, NullValuePolicy.CacheNormally, keyComparer) {
			_preloadedValues = preloadedValues;
		}

		protected override IDictionary<TKey, TValue> BulkFetch() {
		        return _preloadedValues;
		}
    }
}
