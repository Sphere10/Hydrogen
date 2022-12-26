//-----------------------------------------------------------------------
// <copyright file="BulkFetchCacheBase.cs" company="Sphere 10 Software">
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

	public abstract class BulkFetchCacheBase<TKey, TValue> : CacheBase<TKey, TValue> {
		private readonly bool _fetchOnceOnly;
		private uint _fetchCount;

		protected BulkFetchCacheBase(ExpirationPolicy expirationStrategy, TimeSpan? expirationDuration = null, bool fetchOnceOnly = false, NullValuePolicy nullValuePolicy = NullValuePolicy.CacheNormally, StaleValuePolicy staleValuePolicy = StaleValuePolicy.AssumeNeverStale, IEqualityComparer<TKey> keyComparer = null)
			: base(CacheReapPolicy.None, expirationStrategy, uint.MaxValue, expirationDuration, nullValuePolicy, staleValuePolicy, keyComparer, new IsolatedCacheReaper()) {
			_fetchOnceOnly = fetchOnceOnly;
			_fetchCount = 0;
		}

		protected sealed override TValue Fetch(TKey key) {
			TValue result;
			using (EnterWriteScope()) {
				if (!_fetchOnceOnly || _fetchCount == 0) {
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
			if (_fetchCount == 0)
				ForceRefresh();

			return base.Get(key);
		}

		public override bool ContainsCachedItem(object key) {
			if (_fetchCount == 0)
				ForceRefresh();

			return base.ContainsCachedItem(key);
		}

		public void ForceRefresh() {
			Flush();
			BulkLoad(BulkFetch());
			_fetchCount++;
		}

		public override void Flush() {
			base.Flush();
			_fetchCount = 0;
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

}