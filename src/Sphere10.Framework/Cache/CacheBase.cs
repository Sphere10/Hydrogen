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
using System.Threading;
using System.Threading.Tasks;

namespace Sphere10.Framework {

    public abstract class CacheBase<TKey, TValue> : ThreadSafeObject, ICache<TKey, TValue>, IDisposable {
        public event EventHandlerEx<TKey, TValue> ItemFetched; 
        public event EventHandlerEx<TKey, CachedItem<TValue>> ItemRemoved;
        protected IDictionary<TKey, CachedItem<TValue>> InternalStorage;
        
        protected CacheBase(
            CacheReapPolicy reapStrategy = CacheReapPolicy.None, 
            ExpirationPolicy expirationStrategy = ExpirationPolicy.None,
            uint maxCapacity = int.MaxValue,
            TimeSpan? expirationDuration = null,
            NullValuePolicy nullValuePolicy = NullValuePolicy.CacheNormally,
            IEqualityComparer<TKey> keyComparer = null
        ) {
            if (!expirationDuration.HasValue) {
                expirationDuration = TimeSpan.MaxValue;
            }
            NullValuePolicy = nullValuePolicy;
            InternalStorage = keyComparer == null ? new Dictionary<TKey, CachedItem<TValue>>() : new Dictionary<TKey, CachedItem<TValue>>(keyComparer);
            CurrentSize = 0;
            ReapPolicy = reapStrategy;
            ExpirationPolicy = expirationStrategy;
            MaxCapacity = maxCapacity;
            ExpirationDuration = expirationDuration.Value;
        }

		public int ItemCount => InternalStorage.Count;

		public long CurrentSize { get; private set; }

        public long MaxCapacity { get; set; }
	    
        public TimeSpan ExpirationDuration { get; set; }

        public CacheReapPolicy ReapPolicy { get; set; }

        public NullValuePolicy NullValuePolicy { get; set; }

        public ExpirationPolicy ExpirationPolicy { get; set; }

	    public virtual bool ContainsCachedItem(TKey key) {
			return InternalStorage.TryGetValue(key, out var item) && !IsExpired(item);
	    }

        public virtual TValue this[TKey key] => Get(key);

	    public virtual IDictionary<TKey, CachedItem<TValue>> GetCachedItems() {
            return new Dictionary<TKey, CachedItem<TValue>>(InternalStorage).AsReadOnly();
        }

		public void Invalidate(TKey key) {
			if (InternalStorage.TryGetValue(key, out var item)) {
                item.Invalidated = true;
            }
		}

        public virtual TValue Get(TKey key) {
			if (!InternalStorage.TryGetValue(key, out var item)) {
                using (EnterWriteScope()) {
                    if (!InternalStorage.TryGetValue(key, out item)) {
                        item = AddItemInternal(key, Fetch(key));
                    }
                }
            } else if (IsExpired(item)) {
                using (EnterWriteScope()) {
                    RemoveItemInternal(key);
                    item = AddItemInternal(key, Fetch(key));
                }
            }
            item.AccessedCount++;
            item.LastAccessedOn = DateTime.Now;
            var value = item.Value;
			if (value != null)
				return value;

			switch (NullValuePolicy) {
				case NullValuePolicy.Throw:
					throw new SoftwareException("Cache fetched a null value and this cache NullValuePolicy prohibits null values.");
				case NullValuePolicy.ReturnButDontCache:
					Invalidate(key);
					break;
				case NullValuePolicy.CacheNormally:
				default:
					break;
			}
			return default;
        }

        public virtual void Remove(TKey key) {
            using (this.EnterWriteScope()) {
                RemoveItemInternal(key);
            }
        }

        public bool IsExpired(CachedItem<TValue> item) {
            if (item.Invalidated)
                return true;

            DateTime from;
            switch(ExpirationPolicy) {
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

        public virtual void Flush() {
            using (this.EnterWriteScope()) {
                InternalStorage.Keys.ToArray().ForEach(RemoveItemInternal);
                InternalStorage.Clear();
            }
        }

	    public async Task<TValue> GetAsync(TKey key) {
		    using (EnterReadScope()) {
			    if (ContainsCachedItem(key))
				    return Get(key);
		    }
			// leave scope as new thread will deadlock
		    return await Task.Run(() => Get(key));
		    
	    }

	    public virtual void BulkLoad(IEnumerable<KeyValuePair<TKey, TValue>> bulkLoadedValues) {
            using (this.EnterWriteScope()) {
                foreach (var kvp in bulkLoadedValues) {
                    if (InternalStorage.ContainsKey(kvp.Key)) {
                        InternalStorage[kvp.Key].Value = kvp.Value;
                        InternalStorage[kvp.Key].LastAccessedOn = DateTime.Now;
                    } else {
                        AddItemInternal(kvp.Key, kvp.Value);
                    }
                }
            }
        }

        public virtual void Dispose() {
            using (this.EnterWriteScope()) {
				InternalStorage
                    .ToArray()
                    .ForEach(x => RemoveItemInternal(x.Key));
                InternalStorage.Clear();
            }
        }

        protected virtual void OnItemFetched(TKey key, TValue val) {            
        }

        protected virtual void OnItemRemoved(TKey key, CachedItem<TValue> val) {
        }

        protected abstract uint EstimateSize(TValue value);

        protected abstract TValue Fetch(TKey key);

        protected virtual void MakeSpace(uint requestedSpace) {
            using (SystemLog.Logger.LogDuration("CacheBase::MakeSpace"))
            using (this.EnterWriteScope()) {
                if (requestedSpace > MaxCapacity) {
                    throw new SoftwareException("Cache capacity insufficient for requested space {0}", requestedSpace);
                }

                if (ReapPolicy == CacheReapPolicy.ASAP) {
                    MakeSpaceFast(requestedSpace);
                    return;
                }

                // get the elements order with the expired first
                var deathRow =
                    from keyItem in (
                        from key in InternalStorage.Keys
                        let cachedItem = InternalStorage[key]
                        select new {
                            Key = key,
                            Item = cachedItem,
                            IsExpired = IsExpired(cachedItem)
                        }
                    )
                    orderby keyItem.IsExpired
                    select keyItem;

                // the order by the reap strategy
                var reapAll = false;
				var now = DateTime.Now;
				switch (ReapPolicy) {
                    case CacheReapPolicy.LeastUsed:
                        deathRow = deathRow.ThenBy(c => c.Item.AccessedCount).ThenByDescending(c => c.Item.Size);
                        break;
					case CacheReapPolicy.Oldest:						
						deathRow = deathRow.ThenBy(c => c.Item.FetchedOn);
						break;
					case CacheReapPolicy.LongestIdle:
                        deathRow = deathRow.ThenByDescending(c => now.Subtract(c.Item.LastAccessedOn));
                        break;
                    case CacheReapPolicy.Largest:
                        deathRow = deathRow.ThenByDescending(c => c.Item.Size);
                        break;
                    case CacheReapPolicy.Smallest:
                        deathRow = deathRow.ThenBy(c => c.Item.Size);
                        break;
                    case CacheReapPolicy.None:
                        deathRow =
                            ExpirationPolicy != ExpirationPolicy.None ?
                                deathRow.Where(x => x.IsExpired).OrderBy(x => x.Key) : // just take the expired elements only
                                deathRow.Take(0).OrderBy(x => x.Key); // reap nothing
                        reapAll = true;
                        break;
					case CacheReapPolicy.ASAP:
                        throw new InternalErrorException();
					default:
                        throw new NotSupportedException(ReapPolicy.ToString());
                }

                deathRow
                    .TakeWhile(item => reapAll || (MaxCapacity - CurrentSize) < requestedSpace)
                    .ForEach(x => RemoveItemInternal(x.Key));

                if ((MaxCapacity - CurrentSize) < requestedSpace) {
                    throw new SoftwareException("After cache reap, cache capacity still insufficient for requested space {0}", requestedSpace);
                }

            }
        }

        protected virtual void MakeSpaceFast(uint requestedSpace) {
            uint savedSpace = 0;
            var deathRow = new HashSet<TKey>();

            foreach (var item in InternalStorage) {
                if (!IsExpired(item.Value))
                    continue;
                deathRow.Add(item.Key);
                savedSpace += item.Value.Size;
                if (requestedSpace >= savedSpace)
                    break;
            }
            if (savedSpace < requestedSpace) {
                foreach (var item in InternalStorage) {
                    if (IsExpired(item.Value) || deathRow.Contains(item.Key))
                        continue;
                    deathRow.Add(item.Key);
                    savedSpace += item.Value.Size;
                    if (requestedSpace >= savedSpace)
                        break;
                }
            }
            foreach (var item in deathRow)
                RemoveItemInternal(item);
        }

        protected void NotifyItemRemoved(TKey key, CachedItem<TValue> val) {
			OnItemRemoved(key, val);
			ItemRemoved?.Invoke(key, val);
		}

        protected void NotifyItemFetched(TKey key, TValue val) {
            OnItemFetched(key, val);
			ItemFetched?.Invoke(key, val);
		}

        #region Auxillary Methods

        private CachedItem<TValue> AddItemInternal(TKey key, TValue val) {
            var item = new CachedItem<TValue> {
                AccessedCount = 0,
                LastAccessedOn = DateTime.Now,
                FetchedOn = DateTime.Now,
                Value = val,
                Size = EstimateSize(val)
            };
            if (MaxCapacity - CurrentSize < item.Size) {
                MakeSpace(item.Size);
            }
            InternalStorage[key] = item;
            CurrentSize += item.Size;
            return item;
        }

        private void RemoveItemInternal(TKey key) {
            var item = InternalStorage[key];
            if (item != null) {
                InternalStorage.Remove(key);
                CurrentSize -= item.Size;
                NotifyItemRemoved(key, item);
                //item.Dispose();
            }
        }

        #endregion
    }
}
