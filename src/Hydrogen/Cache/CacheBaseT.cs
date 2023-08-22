// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;


public abstract class CacheBase<TKey, TValue> : CacheBase, ICache<TKey, TValue> {

	public new event EventHandlerEx<TKey> ItemFetching {
		add => ((CacheBase)this).ItemFetching += Tools.Object.ProjectionMemoizer.RememberProjection<EventHandlerEx<TKey>, EventHandlerEx<object>>(value, handler => (k) => handler((TKey)k));
		remove => ((CacheBase)this).ItemFetching -= Tools.Object.ProjectionMemoizer.ForgetProjection<EventHandlerEx<TKey>, EventHandlerEx<object>>(value);
	}

	public new event EventHandlerEx<TKey, TValue> ItemFetched {
		add => ((CacheBase)this).ItemFetched += Tools.Object.ProjectionMemoizer.RememberProjection<EventHandlerEx<TKey, TValue>, EventHandlerEx<object, object>>(value, handler => (k, v) => handler((TKey)k, (TValue)v));
		remove => ((CacheBase)this).ItemFetched -= Tools.Object.ProjectionMemoizer.ForgetProjection<EventHandlerEx<TKey, TValue>, EventHandlerEx<object, object>>(value);
	}

	public new event EventHandlerEx<TKey, CachedItem<TValue>> ItemRemoved {
		add => ((CacheBase)this).ItemRemoved += Tools.Object.ProjectionMemoizer.RememberProjection<EventHandlerEx<TKey, CachedItem<TValue>>, EventHandlerEx<object, CachedItem>>(value, handler => (k, v) => handler((TKey)k, (CachedItem<TValue>)v));
		remove => ((CacheBase)this).ItemRemoved -= Tools.Object.ProjectionMemoizer.ForgetProjection<EventHandlerEx<TKey, CachedItem<TValue>>, EventHandlerEx<object, CachedItem>>(value);
	}

	protected CacheBase(
		CacheReapPolicy reapStrategy = CacheReapPolicy.None,
		ExpirationPolicy expirationStrategy = ExpirationPolicy.None,
		long maxCapacity = int.MaxValue,
		TimeSpan? expirationDuration = null,
		NullValuePolicy nullValuePolicy = NullValuePolicy.CacheNormally,
		StaleValuePolicy staleValuePolicy = StaleValuePolicy.AssumeNeverStale,
		IEqualityComparer<TKey> keyComparer = null,
		ICacheReaper reaper = null
	) : base((keyComparer ?? EqualityComparer<TKey>.Default).AsProjection(x => (object)x, x => (TKey)x), reapStrategy, expirationStrategy, maxCapacity, expirationDuration, nullValuePolicy, staleValuePolicy, reaper) {
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

	protected sealed override object Fetch(object key)
		=> Fetch((TKey)key);

	protected sealed override bool CheckStaleness(object key, CachedItem item)
		=> CheckStaleness((TKey)key, (CachedItem<TValue>)item);

	protected abstract long EstimateSize(TValue value);

	protected abstract TValue Fetch(TKey key);

	protected abstract bool CheckStaleness(TKey key, CachedItem<TValue> item);

	protected sealed override void OnItemFetching(object key)
		=> OnItemFetching((TKey)key);

	protected sealed override void OnItemFetched(object key, object val)
		=> OnItemFetched((TKey)key, (TValue)val);

	protected sealed override void OnItemRemoved(object key, CachedItem val)
		=> OnItemRemoved((TKey)key, (CachedItem<TValue>)val);


	protected virtual void OnItemFetching(TKey key) {
	}

	protected virtual void OnItemFetched(TKey key, TValue val) {
	}

	protected virtual void OnItemRemoved(TKey key, CachedItem<TValue> val) {
	}

}


