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

namespace Hydrogen {

	/// <summary>
	/// 1 invalidated item, invalidates all. 1 fetched item fetches all
	/// </summary>
	public sealed class BulkFetchActionCache<TKey, TValue> : BulkFetchCacheBase<TKey, TValue> {
		private readonly Func<IDictionary<TKey, TValue>> _bulkFetcher = null;
		private readonly Func<TKey, CachedItem<TValue>, bool> _stalenessChecker = null;

		public BulkFetchActionCache(
			Func<IDictionary<TKey, TValue>> bulkFetcher,
			ExpirationPolicy expirationStrategy = ExpirationPolicy.None,
			TimeSpan? expirationDuration = null,
			bool fetchOnceOnly = false,
			NullValuePolicy nullValuePolicy = NullValuePolicy.CacheNormally,
			StaleValuePolicy staleValuePolicy = StaleValuePolicy.AssumeNeverStale,
			Func<TKey, CachedItem<TValue>, bool> stalenessChecker = null,
			IEqualityComparer<TKey> keyComparer = null
		) : base(expirationStrategy, expirationDuration, fetchOnceOnly, nullValuePolicy, staleValuePolicy, keyComparer) {
			_bulkFetcher = bulkFetcher;
			_stalenessChecker = stalenessChecker;
		}

		protected override IDictionary<TKey, TValue> BulkFetch() 
			=> _bulkFetcher();

		protected override bool CheckStaleness(TKey key, CachedItem<TValue> item) 
			=> _stalenessChecker?.Invoke(key, item) ?? false;


	}
}
