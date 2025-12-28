// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// Bulk cache that refreshes all entries in a single fetch operation when any entry is missing, expired, or invalidated.
/// </summary>
public sealed class BulkFetchActionCache<TKey, TValue> : BulkFetchCacheBase<TKey, TValue> {
	private readonly Func<IDictionary<TKey, TValue>> _bulkFetcher = null;
	private readonly Func<TKey, CachedItem<TValue>, bool> _stalenessChecker = null;

	/// <summary>
	/// Creates a bulk-fetch cache backed by the supplied delegate.
	/// </summary>
	/// <param name="bulkFetcher">Delegate that returns the complete key/value set.</param>
	/// <param name="expirationStrategy">Expiration policy applied to fetched entries.</param>
	/// <param name="expirationDuration">Duration before entries expire.</param>
	/// <param name="fetchOnceOnly">If true, fetches only once and then serves results until manually refreshed.</param>
	/// <param name="nullValuePolicy">Determines how <c>null</c> values are handled.</param>
	/// <param name="staleValuePolicy">Defines how staleness is evaluated.</param>
	/// <param name="stalenessChecker">Optional predicate used when on-demand staleness checks are enabled.</param>
	/// <param name="keyComparer">Key comparer.</param>
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
