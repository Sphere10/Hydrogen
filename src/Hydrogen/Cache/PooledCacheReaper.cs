// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Allows caches to pool their consumption together, so as to maintain a constant amount in memory.
/// </summary>
/// <remarks>
/// Pooled Cache Algorithm:
/// - reap requesting cache
/// - reap all other caches in least-accessed order using 2 passes (50% of remaining)
///
/// The individual caches still maintain their CurrentSize properties, but their MaxSize are all equal
/// </remarks>
public class PooledCacheReaper : CacheReaperBase {

	private readonly SynchronizedSet<CacheBase> _pooledCaches;

	public PooledCacheReaper(int memoryPoolSize) {
		MaxCapacity = memoryPoolSize;
		_pooledCaches = new SynchronizedSet<CacheBase>();
	}

	public long MaxCapacity { get; }

	public long CurrentSize {
		get {
			using (_pooledCaches.EnterReadScope())
				return _pooledCaches.Sum(x => x.CurrentSize);
		}
	}


	public long TotalItemCount {
		get {
			using (_pooledCaches.EnterReadScope())
				return _pooledCaches.Sum(x => x.ItemCount);
		}
	}

	public override long AvailableSpace() => MaxCapacity - CurrentSize;

	public override void Register(ICache cache) {
		Guard.ArgumentNotNull(cache, nameof(cache));
		Guard.ArgumentCast<CacheBase>(cache, out var cacheBase, nameof(cache), $"Needs to be an instance of {nameof(CacheBase)}");
		Guard.Argument(cache.MaxCapacity == MaxCapacity, nameof(cache), "Cache must have same capacity as pool");
		CheckNotRegistered(cacheBase);
		cacheBase.ParentSyncObject = _pooledCaches;
		_pooledCaches.Add(cacheBase);
	}

	public override void Deregister(ICache cache) {
		Guard.ArgumentNotNull(cache, nameof(cache));
		Guard.ArgumentCast<CacheBase>(cache, out var cacheBase, nameof(cache), $"Needs to be an instance of {nameof(CacheBase)}");
		CheckRegistered(cacheBase);
		Guard.Ensure(cache == cacheBase, "Attempting to de-register an unregistered cache");
		cacheBase.ParentSyncObject = null;
		_pooledCaches.Remove(cacheBase);
	}


	public override long MakeSpace(ICache requestingCache, long requestedSpace) {
		Guard.ArgumentNotNull(requestingCache, nameof(requestingCache));
		Guard.ArgumentCast<CacheBase>(requestingCache, out var requestingCacheB, nameof(requestingCache), $"Needs to be an instance of {nameof(CacheBase)}");

		using (_pooledCaches.EnterWriteScope()) {
			// note: locking _caches will lock all the member caches since they share the same sync object 
			if (requestedSpace > MaxCapacity) {
				throw new InvalidOperationException($"Cache capacity insufficient for requested space {requestedSpace}");
			}

			// First try to reap as much space from the requesting cache as possible
			var liberated = IsolatedCacheReaper.MakeSpaceAlgorithm(requestingCacheB, requestedSpace);

			if (liberated < requestedSpace && _pooledCaches.Count > 1) {
				// Still need more space, so try to reap equal portions from other caches
				var portionedCull = (long)Math.Ceiling((requestedSpace - liberated) / (double)(_pooledCaches.Count - 1));
				foreach (var cache in _pooledCaches.Except(requestingCacheB))
					liberated += IsolatedCacheReaper.MakeSpaceAlgorithm(cache, portionedCull);

				if (liberated < requestedSpace) {
					// Request the remaining for all other caches in least-accessed order
					var remaining = requestedSpace - liberated;
					foreach (var cache in _pooledCaches.Except(requestingCacheB).OrderBy(c => c.LastAccessedOn)) {
						liberated += IsolatedCacheReaper.MakeSpaceAlgorithm(cache, remaining);
						remaining = requestedSpace - liberated;
						if (remaining <= 0)
							break;
					}
				}
			}
			return liberated;
		}
	}


	private void CheckRegistered(CacheBase cache) {
		if (!_pooledCaches.Contains(cache))
			throw new InvalidOperationException("Cache was not registered");
	}

	private void CheckNotRegistered(CacheBase cache) {
		if (_pooledCaches.Contains(cache))
			throw new InvalidOperationException("Cache was already registered");
	}
}
