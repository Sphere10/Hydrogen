//-----------------------------------------------------------------------
// <copyright file="ActionCache.cs" company="Sphere 10 Software">
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

	public sealed class ActionCache<TKey, TValue> : CacheBase<TKey, TValue> {

        private readonly Func<TKey, TValue> _fetchFunc = null;
        private readonly Func<TValue, uint> _estimateSizeFunc = null;

        public ActionCache(
            Func<TKey, TValue> valueFetcher,
            Func<TValue, uint> sizeEstimator = null,
            CacheReapPolicy reapStrategy = CacheReapPolicy.None,
            ExpirationPolicy expirationStrategy = ExpirationPolicy.None,
            uint maxCapacity = int.MaxValue,
            TimeSpan? expirationDuration = null,
            NullValuePolicy nullValuePolicy = NullValuePolicy.CacheNormally,
            IEqualityComparer<TKey> keyComparer = null)
        : base(reapStrategy, expirationStrategy, maxCapacity, expirationDuration, nullValuePolicy, keyComparer) {
            _fetchFunc = valueFetcher;
            _estimateSizeFunc = sizeEstimator;
        }

        protected override TValue Fetch(TKey key) {
            var val = _fetchFunc(key);
            NotifyItemFetched(key, val);
            return val;
        }

        protected override uint EstimateSize(TValue value) {
            if (_estimateSizeFunc != null) {
                return _estimateSizeFunc(value);
            }
            return 0;
        }
    }
}
