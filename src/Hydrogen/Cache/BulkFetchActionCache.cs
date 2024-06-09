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
