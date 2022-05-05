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
	        add => base.ItemFetched += ToBaseListener(value);
	        remove => base.ItemFetched -= ToBaseListener(value);
        } 

        public new event EventHandlerEx<TKey, CachedItem<TValue>> ItemRemoved {
	        add => base.ItemRemoved += ToBaseListener(value);
	        remove => base.ItemRemoved -= ToBaseListener(value);
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

        public new IEnumerable<CachedItem<TValue>> CachedItems => base.CachedItems.Cast<CachedItem<TValue>>();

        public bool ContainsCachedItem(TKey key)
	        => base.ContainsCachedItem(key);

        public virtual void Invalidate(TKey key)
	        => base.Invalidate(key);

		public CachedItem<TValue> Get(TKey key)
	        => (CachedItem<TValue>)base.Get(key);

		public void Set(TKey key, TValue value)
			=> base.Set(key, value);

		public TValue this[TKey index] {
			get => (TValue)base[index];
			set => base[index] = value;
		}

        public virtual void BulkLoad(IEnumerable<KeyValuePair<TKey, TValue>> bulkLoadedValues)
	        => base.BulkLoad(bulkLoadedValues.Select(x => new KeyValuePair<object, object>(x.Key, x.Value)));

        public virtual void Remove(TKey key)
	        => base.Remove(key);

        protected override CachedItem NewCachedItem(object key, object value) {
			Guard.ArgumentCast<TKey>(key, out var typedKey, nameof(key));
			Guard.ArgumentCast<TValue>(value, out var typedValue, nameof(value));
			return new CachedItem<TValue> {
				Value = typedValue
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
