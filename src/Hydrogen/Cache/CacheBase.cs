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

namespace Hydrogen {

	public abstract class CacheBase : SynchronizedObject, ICache, IDisposable {

		public event EventHandlerEx<object> ItemFetching;
		public event EventHandlerEx<object, object> ItemFetched;
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

		public IEnumerable<CachedItem> CachedItems => InternalStorage.Values;

		public int ItemCount => InternalStorage.Count;

		public long CurrentSize { get; internal set; }

		public long MaxCapacity { get; internal set; }

		public DateTime LastUpdateOn { get; internal set; }

		public DateTime LastAccessedOn { get; internal set; }

		public long TotalAccesses { get; internal set; }

		public TimeSpan ExpirationDuration { get; internal set; }

		public CacheReapPolicy ReapPolicy { get; internal set; }

        public NullValuePolicy NullValuePolicy { get; internal set; }

        public StaleValuePolicy StaleValuePolicy { get; internal set; }

        public ExpirationPolicy ExpirationPolicy { get; internal set; }

		protected ICacheReaper Reaper { get; }

		public virtual bool ContainsCachedItem(object key) {
			return InternalStorage.TryGetValue(key, out var item) && !IsExpired(item);
		}

		public virtual object this[object key] {
			get => Get(key).Value;
			set => Set(key, value);
		}

		public void Invalidate(object key) {
			if (InternalStorage.TryGetValue(key, out var item)) {
				item.Traits = item.Traits.CopyAndSetFlags(CachedItemTraits.Invalidated, true);
			}
		}

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

		public virtual void Flush() {
			using (this.EnterWriteScope()) {
				foreach (var key in InternalStorage.Keys.ToArray())
					RemoveItemInternal(key);
				InternalStorage.Clear();
			}
		}

		public virtual void BulkLoad(IEnumerable<KeyValuePair<object, object>> bulkLoadedValues) {
			using (this.EnterWriteScope()) {
				foreach (var kvp in bulkLoadedValues) 
					this[kvp.Key] = kvp.Value;
			}
		}

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

		protected abstract long EstimateSize(object value);

		protected abstract object Fetch(object key);

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
				var liberatedSpace = Reaper.MakeSpace(this, item.Size);  // in a pooled reap situation, space is made in other caches
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
}
