//-----------------------------------------------------------------------
// <copyright file="CacheBase.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Threading.Tasks;

namespace Hydrogen {

	public abstract  class CacheBase<TKey, TValue> : CacheBase, ICache<TKey, TValue> {
        public new event EventHandlerEx<TKey, TValue> ItemFetched {
	        add => ((CacheBase)this).ItemFetched += ToBaseListener(value);
	        remove => ((CacheBase)this).ItemFetched -= ToBaseListener(value);
        } 

        public new event EventHandlerEx<TKey, CachedItem<TValue>> ItemRemoved {
	        add => ((CacheBase)this).ItemRemoved += ToBaseListener(value);
	        remove => ((CacheBase)this).ItemRemoved -= ToBaseListener(value);
        }

		protected CacheBase(
	        CacheReapPolicy reapStrategy = CacheReapPolicy.None,
	        ExpirationPolicy expirationStrategy = ExpirationPolicy.None,
	        long maxCapacity = int.MaxValue,
	        TimeSpan? expirationDuration = null,
	        NullValuePolicy nullValuePolicy = NullValuePolicy.CacheNormally,
	        IEqualityComparer<TKey> keyComparer = null,
	        ICacheReaper reaper = null
        ) : base(new CastedEqualityComparer<TKey, object>(keyComparer ?? EqualityComparer<TKey>.Default), reapStrategy, expirationStrategy, maxCapacity, expirationDuration, nullValuePolicy, reaper) {
        }

        public new IEnumerable<CachedItem<TValue>> CachedItems => ((CacheBase)this).CachedItems.Cast<CachedItem<TValue>>(); 

        public bool ContainsCachedItem(TKey key) => ((CacheBase)this).ContainsCachedItem(key);

        public void Invalidate(TKey key) => ((CacheBase)this).Invalidate(key); 

		public CachedItem<TValue> Get(TKey key) => (CachedItem<TValue>)((CacheBase)this).Get(key); 

		public void Set(TKey key, TValue value) => ((CacheBase)this).Set(key, value);

		public TValue this[TKey index] { 
			get => (TValue)((CacheBase)this)[index];
			set => ((CacheBase)this)[index] = value;
		}

        public void BulkLoad(IEnumerable<KeyValuePair<TKey, TValue>> bulkLoadedValues)
	        => ((CacheBase)this).BulkLoad(bulkLoadedValues.Select(x => new KeyValuePair<object, object>(x.Key, x.Value))); 

        public void Remove(TKey key)
	        => ((CacheBase)this).Remove(key);

        protected override CachedItem NewCachedItem(object key, object value) {
			Guard.ArgumentCast<TKey>(key, out _, nameof(key));
			if (value != null)
				Guard.ArgumentCast<TValue>(value, out _, nameof(value));
			return new CachedItem<TValue> {
				Value = value != null ? (TValue)value : default
			};
		}

        protected sealed override long EstimateSize(object value)
	        => EstimateSize((TValue)value);
		
        protected override object Fetch(object key)
	        => Fetch((TKey)key);

        protected abstract long EstimateSize(TValue value);

        protected abstract TValue Fetch(TKey key);

        protected sealed override void OnItemFetched(object key, object val)
	        => OnItemFetched((TKey)key, (TValue)val);

        protected sealed override void OnItemRemoved(object key, CachedItem val)
	        => OnItemRemoved((TKey)key, (CachedItem<TValue>)val);

		protected virtual void OnItemFetched(TKey key, TValue val) {
        }

        protected virtual void OnItemRemoved(TKey key, CachedItem<TValue> val) {
        }

		private static EventHandlerEx<object, object> ToBaseListener(EventHandlerEx<TKey, TValue> listener) {
	        return (k, v) => listener((TKey)k, (TValue)v);
        }

        private static EventHandlerEx<object, CachedItem> ToBaseListener(EventHandlerEx<TKey, CachedItem<TValue>> listener) {
	        return (k, v) => listener((TKey)k, (CachedItem<TValue>)v);
        }

	}
}
