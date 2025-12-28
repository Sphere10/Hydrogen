// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Provides core synchronization, expiration, and reaping behavior for cache implementations.
/// </summary>
public abstract class CacheBase : SynchronizedObject, ICache, IDisposable {

	/// <inheritdoc />
	public event EventHandlerEx<object> ItemFetching;
	/// <inheritdoc />
	public event EventHandlerEx<object, object> ItemFetched;
	/// <inheritdoc />
	public event EventHandlerEx<object, CachedItem> ItemRemoved;

	internal IDictionary<object, CachedItem> InternalStorage;

	protected CacheBase(
		IEqualityComparer<object> keyComparer,
		CacheReapPolicy reapStrategy = CacheReapPolicy.None,
		ExpirationPolicy expirationStrategy = ExpirationPolicy.None,
		long maxCapacity = int.MaxValue,
		TimeSpan? expirationDuration = null,
		NullValuePolicy nullValuePolicy = NullValuePolicy.CacheNormally,
		StaleValuePolicy staleValuePolicy = StaleValuePolicy.AssumeNeverStale,
		ICacheReaper reaper = null
	) {
		Guard.ArgumentNotNull(keyComparer, nameof(keyComparer));
		expirationDuration ??= TimeSpan.MaxValue;
		expirationDuration ??= TimeSpan.MaxValue;
		InternalStorage = new Dictionary<object, CachedItem>(keyComparer);
		NullValuePolicy = nullValuePolicy;
		StaleValuePolicy = staleValuePolicy;
		ReapPolicy = reapStrategy;
		ExpirationPolicy = expirationStrategy;
		MaxCapacity = maxCapacity;
		ExpirationDuration = expirationDuration.Value;
		LastUpdateOn = LastAccessedOn = DateTime.Now;
		TotalAccesses = 0;
		Reaper = reaper ?? new IsolatedCacheReaper();
		Reaper.Register(this);
	}

	/// <inheritdoc />
	public IEnumerable<CachedItem> CachedItems => InternalStorage.Values;

	/// <inheritdoc />
	public int ItemCount => InternalStorage.Count;

	/// <inheritdoc />
	public long CurrentSize { get; internal set; }

	/// <inheritdoc />
	public long MaxCapacity { get; internal set; }

	/// <inheritdoc />
	public DateTime LastUpdateOn { get; internal set; }

	/// <inheritdoc />
	public DateTime LastAccessedOn { get; internal set; }

	/// <inheritdoc />
	public long TotalAccesses { get; internal set; }

	/// <inheritdoc />
	public TimeSpan ExpirationDuration { get; internal set; }

	/// <inheritdoc />
	public CacheReapPolicy ReapPolicy { get; internal set; }

	/// <inheritdoc />
	public NullValuePolicy NullValuePolicy { get; internal set; }

	/// <summary>
	/// Indicates how staleness checks are performed.
	/// </summary>
	public StaleValuePolicy StaleValuePolicy { get; internal set; }

	/// <inheritdoc />
	public ExpirationPolicy ExpirationPolicy { get; internal set; }

	protected ICacheReaper Reaper { get; }

	/// <inheritdoc />
	public virtual bool ContainsCachedItem(object key) {
		return InternalStorage.TryGetValue(key, out var item) && !IsExpired(item);
	}

	/// <inheritdoc />
	public virtual object this[object key] {
		get => Get(key).Value;
		set => Set(key, value);
	}

	/// <inheritdoc />
	public void Invalidate(object key) {
		if (InternalStorage.TryGetValue(key, out var item)) {
			item.Traits = item.Traits.CopyAndSetFlags(CachedItemTraits.Invalidated, true);
		}
	}

	/// <inheritdoc />
	public virtual CachedItem Get(object key) {
		Guard.ArgumentNotNull(key, nameof(key));
		if (!InternalStorage.TryGetValue(key, out var item)) {
			using (EnterWriteScope()) {
				if (!InternalStorage.TryGetValue(key, out item)) {
					NotifyItemFetching(key);
					var fetchedVal = Fetch(key);
					item = AddItemInternal(key, fetchedVal);
					NotifyItemFetched(key, fetchedVal);
				}
			}
		} else if (IsExpired(item) || IsStale(key, item)) {
			using (EnterWriteScope()) {
				RemoveItemInternal(key);
				NotifyItemFetching(key);
				var fetchedVal = Fetch(key);
				item = AddItemInternal(key, fetchedVal);
				NotifyItemFetched(key, fetchedVal);
			}
		}
		TotalAccesses++;
		LastAccessedOn = DateTime.Now;
		item.AccessedCount++;
		item.LastAccessedOn = DateTime.Now;
		if (item.Value == null) {
			switch (NullValuePolicy) {
				case NullValuePolicy.CacheNormally:
					break;
				case NullValuePolicy.ReturnButDontCache:
					Invalidate(key);
					break;
				case NullValuePolicy.Throw:
				default:
					throw new InvalidOperationException("Cache fetched a null value and this cache NullValuePolicy prohibits null values.");
			}
		}
		return item;
	}

	/// <inheritdoc />
	public virtual void Set(object key, object value) {
		using (this.EnterWriteScope()) {
			if (InternalStorage.ContainsKey(key)) {
				InternalStorage[key].Value = value;
				InternalStorage[key].LastAccessedOn = DateTime.Now;
			} else {
				AddItemInternal(key, value);
			}
			this.LastUpdateOn = DateTime.Now;
		}
	}

	/// <inheritdoc />
	public virtual void Remove(object key) {
		using (this.EnterWriteScope()) {
			RemoveItemInternal(key);
		}
	}

	public bool IsExpired(CachedItem item) {
		if (item.Traits.HasFlag(CachedItemTraits.Invalidated))
			return true;

		DateTime from;
		switch (ExpirationPolicy) {
			case ExpirationPolicy.SinceFetchedTime:
				from = item.FetchedOn;
				break;
			case ExpirationPolicy.SinceLastAccessedTime:
				from = item.LastAccessedOn;
				break;
			default:
				from = DateTime.Now;
				break;
		}
		return ExpirationPolicy != ExpirationPolicy.None && DateTime.Now.Subtract(from) > ExpirationDuration;
	}

	public bool IsStale(object key, CachedItem item) {
		switch (StaleValuePolicy) {
			case StaleValuePolicy.AssumeNeverStale:
				return false;
			case StaleValuePolicy.CheckStaleOnDemand:
				return CheckStaleness(key, item);
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	/// <inheritdoc />
	public virtual void Purge() {
		using (this.EnterWriteScope()) {
			foreach (var key in InternalStorage.Keys.ToArray())
				RemoveItemInternal(key);
			InternalStorage.Clear();
		}
	}

	/// <inheritdoc />
	public virtual void BulkLoad(IEnumerable<KeyValuePair<object, object>> bulkLoadedValues) {
		using (this.EnterWriteScope()) {
			foreach (var kvp in bulkLoadedValues)
				this[kvp.Key] = kvp.Value;
		}
	}

	/// <inheritdoc />
	public virtual void Dispose() {
		using (this.EnterWriteScope()) {
			foreach (var key in InternalStorage.ToArray())
				RemoveItemInternal(key.Key);
			InternalStorage.Clear();
		}
		Reaper.Deregister(this);
	}

	protected virtual void OnItemFetching(object key) {
	}

	protected virtual void OnItemFetched(object key, object val) {
	}

	protected virtual void OnItemRemoved(object key, CachedItem val) {
	}

	/// <summary>
	/// Estimates the size contribution of a value for capacity enforcement.
	/// </summary>
	protected abstract long EstimateSize(object value);

	/// <summary>
	/// Resolves a value for the supplied key when a cache miss occurs.
	/// </summary>
	protected abstract object Fetch(object key);

	/// <summary>
	/// Determines whether an item should be considered stale and refetched.
	/// </summary>
	protected abstract bool CheckStaleness(object key, CachedItem item);

	protected void NotifyItemFetching(object key) {
		OnItemFetching(key);
		ItemFetching?.Invoke(key);
	}


	protected void NotifyItemFetched(object key, object val) {
		OnItemFetched(key, val);
		ItemFetched?.Invoke(key, val);
	}

	protected void NotifyItemRemoved(object key, CachedItem val) {
		OnItemRemoved(key, val);
		ItemRemoved?.Invoke(key, val);
	}

	protected abstract CachedItem NewCachedItem(object key, object value);

	#region Auxillary Methods

	internal CachedItem AddItemInternal(object key, object val) {
		var item = NewCachedItem(key, val);
		item.AccessedCount = 0;
		item.LastAccessedOn = DateTime.Now;
		item.FetchedOn = DateTime.Now;
		item.Value = val;
		item.Size = EstimateSize(val);

		var availableSpace = Reaper.AvailableSpace();
		if (availableSpace < item.Size) {
			var liberatedSpace = Reaper.MakeSpace(this, item.Size); // in a pooled reap situation, space is made in other caches
			if (availableSpace + liberatedSpace < item.Size) {
				throw new InvalidOperationException($"After cache reap, cache capacity still insufficient for requested space {item.Size}");
			}
		}
		InternalStorage[key] = item;
		CurrentSize += item.Size;
		return item;
	}

	internal void RemoveItemInternal(object key) {
		var item = InternalStorage[key];
		if (item != null) {
			InternalStorage.Remove(key);
			CurrentSize -= item.Size;
			NotifyItemRemoved(key, item);
			item.Traits = item.Traits.CopyAndSetFlags(CachedItemTraits.Purged, true);
			item.Traits = item.Traits.CopyAndSetFlags(CachedItemTraits.CanPurge, false);
			//item.Dispose();
		}
	}

	#endregion

}
