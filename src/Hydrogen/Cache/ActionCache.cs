// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public sealed class ActionCache<TKey, TValue> : CacheBase<TKey, TValue> {

	private readonly Func<TKey, TValue> _fetchFunc = null;
	private readonly Func<TValue, long> _estimateSizeFunc = null;
	private readonly Func<TKey, CachedItem<TValue>, bool> _stalenessChecker = null;

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
