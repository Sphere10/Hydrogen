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
/// Reaper that manages capacity for a single cache instance without coordination with others.
/// </summary>
public class IsolatedCacheReaper : CacheReaperBase {

	private CacheBase _cache;

	public override void Register(ICache cache) {
		Guard.ArgumentNotNull(cache, nameof(cache));
		Guard.ArgumentCast<CacheBase>(cache, out var cacheBase, nameof(cache), $"Needs to be an instance of {nameof(CacheBase)}");
		CheckNotRegistered();
		_cache = cacheBase;
	}

	public override void Deregister(ICache cache) {
		Guard.ArgumentNotNull(cache, nameof(cache));
		Guard.ArgumentCast<CacheBase>(cache, out var cacheBase, nameof(cache), $"Needs to be an instance of {nameof(CacheBase)}");
		CheckRegistered();
		Guard.Ensure(cache == cacheBase, "Attempting to de-register an unregistered cache");
		_cache = null;
	}

	public override long AvailableSpace() {
		CheckRegistered();
		return _cache.MaxCapacity - _cache.CurrentSize;
	}

	public override long MakeSpace(ICache requestingCache, long requestedSpace) {
		CheckRegistered();
		Guard.Argument(requestingCache == _cache, nameof(requestingCache), "Not a registered cache");
		return MakeSpaceAlgorithm(_cache, requestedSpace);
	}

	internal static long MakeSpaceAlgorithm(CacheBase cache, long requestedSpace) {

		using (cache.EnterWriteScope()) {
			if (requestedSpace > cache.MaxCapacity) {
				throw new InvalidOperationException($"Cache capacity insufficient for requested space {requestedSpace}");
			}

			if (cache.ReapPolicy == CacheReapPolicy.ASAP) {
				// This mode attempts to free-space quickly
				// as used in high-demand caches
				return MakeSpaceFastAlgorithm(cache, requestedSpace);
			}

			// get the elements order with the expired first
			var deathRow =
				from keyItem in (
					from key in cache.InternalStorage.Keys
					let cachedItem = cache.InternalStorage[key]
					where cachedItem.Traits.HasFlag(CachedItemTraits.CanPurge)
					select new {
						Key = key,
						Item = cachedItem,
						IsExpired = cache.IsExpired(cachedItem)
					}
				)
				orderby keyItem.IsExpired
				select keyItem;

			// the order by the reap strategy
			var reapAll = false;
			var now = DateTime.Now;
			switch (cache.ReapPolicy) {
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
						cache.ExpirationPolicy != ExpirationPolicy.None
							? deathRow.Where(x => x.IsExpired).OrderBy(x => x.Key)
							: // just take the expired elements only
							deathRow.Take(0).OrderBy(x => x.Key); // reap nothing
					reapAll = true;
					break;
				case CacheReapPolicy.ASAP:
					throw new InternalErrorException();
				default:
					throw new NotSupportedException(cache.ReapPolicy.ToString());
			}

			var releasedSpace = 0L;
			foreach (var x in deathRow
				         .TakeWhile(item => reapAll || releasedSpace < requestedSpace)
				         .Apply(x => releasedSpace += x.Item.Size))
				cache.RemoveItemInternal(x.Key);

			return releasedSpace;

		}
	}

	internal static long MakeSpaceFastAlgorithm(CacheBase cache, long requestedSpace) {
		var savedSpace = 0L;
		var deathRow = new HashSet<object>();

		foreach (var item in cache.InternalStorage) {
			if (!cache.IsExpired(item.Value))
				continue;
			deathRow.Add(item.Key);
			savedSpace += item.Value.Size;
			if (requestedSpace >= savedSpace)
				break;
		}
		if (savedSpace < requestedSpace) {
			foreach (var item in cache.InternalStorage) {
				if (cache.IsExpired(item.Value) || deathRow.Contains(item.Key))
					continue;
				deathRow.Add(item.Key);
				savedSpace += item.Value.Size;
				if (requestedSpace >= savedSpace)
					break;
			}
		}
		foreach (var item in deathRow)
			cache.RemoveItemInternal(item);
		return savedSpace;
	}

	private void CheckRegistered() {
		Guard.Ensure(_cache != null, "No cache was registered");
	}

	private void CheckNotRegistered() {
		Guard.Ensure(_cache == null, "A cache was already registered");
	}

}
