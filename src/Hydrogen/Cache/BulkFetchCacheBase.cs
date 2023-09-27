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

public abstract class BulkFetchCacheBase<TKey, TValue> : CacheBase<TKey, TValue> {
	private readonly bool _fetchOnceOnly;
	internal uint FetchCount;

	protected BulkFetchCacheBase(ExpirationPolicy expirationStrategy, TimeSpan? expirationDuration = null, bool fetchOnceOnly = false, NullValuePolicy nullValuePolicy = NullValuePolicy.CacheNormally,
	                             StaleValuePolicy staleValuePolicy = StaleValuePolicy.AssumeNeverStale, IEqualityComparer<TKey> keyComparer = null)
		: base(CacheReapPolicy.None, expirationStrategy, uint.MaxValue, expirationDuration, nullValuePolicy, staleValuePolicy, keyComparer, new IsolatedCacheReaper()) {
		_fetchOnceOnly = fetchOnceOnly;
		FetchCount = 0;
	}

	protected sealed override TValue Fetch(TKey key) {
		TValue result;
		using (EnterWriteScope()) {
			if (!_fetchOnceOnly || FetchCount == 0) {
				NotifyItemFetching(key);
				ForceRefresh();
			}

			if (InternalStorage.TryGetValue(key, out var item)) {
				item.AccessedCount++;
				item.LastAccessedOn = DateTime.Now;
				base.TotalAccesses++;
				base.LastAccessedOn = DateTime.Now;
				result = ((CachedItem<TValue>)item).Value;
			} else {
				switch (NullValuePolicy) {
					case NullValuePolicy.Throw:
						throw new SoftwareException("BulkCache does not have key with id {0}", key);
					case NullValuePolicy.ReturnButDontCache:
					case NullValuePolicy.CacheNormally:
					default:
						result = default;
						break;
				}
			}
		}
		NotifyItemFetched(key, result);
		return result;
	}

	public override CachedItem Get(object key) {
		if (FetchCount == 0)
			ForceRefresh();

		return base.Get(key);
	}

	public override bool ContainsCachedItem(object key) {
		if (FetchCount == 0)
			ForceRefresh();

		return base.ContainsCachedItem(key);
	}

	public void ForceRefresh() {
		Flush();
		BulkLoad(BulkFetch());
		FetchCount++;
	}

	public override void Flush() {
		base.Flush();
		FetchCount = 0;
	}

	public override void Remove(object key) {
		throw new NotSupportedException("Items cannot be manually removed from a bulk fetch cache");
	}

	protected sealed override long EstimateSize(TValue value) {
		// Size limiting not applied to bulk-fetched cache
		return 0L;
	}

	protected abstract IDictionary<TKey, TValue> BulkFetch();

}
