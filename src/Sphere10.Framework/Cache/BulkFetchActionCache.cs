//-----------------------------------------------------------------------
// <copyright file="BulkFetchActionCache.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework {

	/// <summary>
	/// 1 invalidated item, invalidates all. 1 fetched item fetches all
	/// </summary>
	public sealed class BulkFetchActionCache<TKey, TValue> : BulkFetchCacheBase<TKey, TValue> {
		private readonly Func<IDictionary<TKey, TValue>> _bulkFetcher = null;
		public BulkFetchActionCache(
			Func<IDictionary<TKey, TValue>> bulkFetcher,
			ExpirationPolicy expirationStrategy = ExpirationPolicy.None,
			TimeSpan? expirationDuration = null,
			bool fetchOnceOnly = false,
			NullValuePolicy nullValuePolicy = NullValuePolicy.CacheNormally,
			IEqualityComparer<TKey> keyComparer = null
		) : base(expirationStrategy, expirationDuration, fetchOnceOnly, nullValuePolicy, keyComparer) {
			_bulkFetcher = bulkFetcher;
		}

		protected override IDictionary<TKey, TValue> BulkFetch() {
			return _bulkFetcher();
		}

	}
}
