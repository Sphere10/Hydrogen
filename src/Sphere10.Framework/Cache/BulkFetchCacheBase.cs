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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Sphere10.Framework {

	public abstract class BulkFetchCacheBase<TKey, TValue> : CacheBase<TKey, TValue> {
	    private readonly bool _fetchOnceOnly;
	    private uint _fetchCount;
           
		protected BulkFetchCacheBase(ExpirationPolicy expirationStrategy, TimeSpan? expirationDuration = null, bool fetchOnceOnly = false, NullValuePolicy nullValuePolicy = NullValuePolicy.CacheNormally, IEqualityComparer<TKey> keyComparer = null)
			: base (CacheReapPolicy.None, expirationStrategy, uint.MaxValue, expirationDuration, nullValuePolicy, keyComparer) {
		    _fetchOnceOnly = fetchOnceOnly;
		    _fetchCount = 0;
		}

        protected sealed override TValue Fetch(TKey key) {
            TValue result;
			using (this.EnterWriteScope()) {
			    if (!_fetchOnceOnly || _fetchCount == 0) {
			        ForceRefresh();
			    }
				if (InternalStorage.TryGetValue(key, out var item)) {
                    item.AccessedCount++;
                    item.LastAccessedOn = DateTime.Now;
                    result = item.Value;
                } else { 
				    switch(NullValuePolicy) {
                        case NullValuePolicy.Throw:
                            throw new SoftwareException("BulkCache does not have key with id {0}", key);
				        case NullValuePolicy.ReturnButDontCache:
                        case NullValuePolicy.CacheNormally:
                        default:
				            result = default(TValue);
				            break;
				    }
				}
			}
            NotifyItemFetched(key, result);
			return result;
		}

	    public override void Remove(TKey key) {
	        throw new NotSupportedException("Items cannot be manually removed from a bulk fetch cache");
	    }

	    protected sealed override uint EstimateSize(TValue value) {
            // Size limiting not applied to bulk-fetched cache
			return 0;
		}

		public override IReadOnlyDictionary<TKey, CachedItem<TValue>> GetCachedItems() {
			return base.GetCachedItems();
		}

		protected abstract IDictionary<TKey, TValue> BulkFetch();

		public void ForceRefresh() {
			Flush();
			BulkLoad(BulkFetch());
		    _fetchCount++;
		}

    }
}
