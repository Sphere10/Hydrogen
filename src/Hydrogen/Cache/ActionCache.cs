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
/// Cache implementation that fetches individual values via delegates and supports optional size estimation and staleness checks.
/// </summary>
public sealed class ActionCache<TKey, TValue> : CacheBase<TKey, TValue> {

	private readonly Func<TKey, TValue> _fetchFunc = null;
	private readonly Func<TValue, long> _estimateSizeFunc = null;
	private readonly Func<TKey, CachedItem<TValue>, bool> _stalenessChecker = null;

	/// <summary>
	/// Creates a cache that resolves values on demand.
	/// </summary>
	/// <param name="valueFetcher">Delegate used to populate a missing or expired entry.</param>
	/// <param name="sizeEstimator">Optional delegate estimating item size for capacity enforcement.</param>
	/// <param name="reapStrategy">Strategy for selecting eviction candidates.</param>
	/// <param name="expirationStrategy">Expiration policy for cached entries.</param>
	/// <param name="maxCapacity">Maximum aggregate size before reaping occurs.</param>
	/// <param name="expirationDuration">Optional expiration window; defaults to <see cref="TimeSpan.MaxValue"/>.</param>
	/// <param name="nullValuePolicy">Defines how fetched <c>null</c> values are treated.</param>
	/// <param name="staleValuePolicy">Determines how stale entries are detected.</param>
	/// <param name="stalenessChecker">Optional predicate to detect stale entries when <paramref name="staleValuePolicy"/> requests it.</param>
	/// <param name="keyComparer">Equality comparer for keys.</param>
	/// <param name="reaper">Optional shared reaper; defaults to <see cref="IsolatedCacheReaper"/>.</param>
	public ActionCache(
		Func<TKey, TValue> valueFetcher,
		Func<TValue, long> sizeEstimator = null,
		CacheReapPolicy reapStrategy = CacheReapPolicy.None,
		ExpirationPolicy expirationStrategy = ExpirationPolicy.None,
		long maxCapacity = long.MaxValue,
		TimeSpan? expirationDuration = null,
		NullValuePolicy nullValuePolicy = NullValuePolicy.CacheNormally,
		StaleValuePolicy staleValuePolicy = StaleValuePolicy.AssumeNeverStale,
		Func<TKey, CachedItem<TValue>, bool> stalenessChecker = null,
		IEqualityComparer<TKey> keyComparer = null,
		ICacheReaper reaper = null)
		: base(reapStrategy, expirationStrategy, maxCapacity, expirationDuration, nullValuePolicy, staleValuePolicy, keyComparer, reaper) {
		Guard.ArgumentNotNull(valueFetcher, nameof(valueFetcher));
		_fetchFunc = valueFetcher;
		_estimateSizeFunc = sizeEstimator;
		_stalenessChecker = stalenessChecker;
	}

	protected override TValue Fetch(TKey key) => _fetchFunc(key);

	protected override bool CheckStaleness(TKey key, CachedItem<TValue> item)
		=> _stalenessChecker?.Invoke(key, item) ?? false;

	protected override long EstimateSize(TValue value)
		=> _estimateSizeFunc?.Invoke(value) ?? 0;
}
