using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static Hydrogen.AMS;

namespace Hydrogen {

	public abstract class FileCacheBase<TContent> : CacheBase<string, TContent>  {
		private readonly IDictionary<string, DateTime> _lastModified;

		protected FileCacheBase(
			CacheReapPolicy reapStrategy = CacheReapPolicy.LeastUsed,
			ExpirationPolicy expirationStrategy = ExpirationPolicy.SinceLastAccessedTime,
			long maxCapacity = int.MaxValue,
			TimeSpan? expirationDuration = null,
			IEqualityComparer<string> fileNameComparer = null,
			ICacheReaper reaper = null
		) : base(reapStrategy, expirationStrategy, maxCapacity, expirationDuration, NullValuePolicy.Throw, StaleValuePolicy.CheckStaleOnDemand, fileNameComparer, reaper){
			_lastModified = new Dictionary<string, DateTime>();
		}

		/// <summary>
		/// Will continue to return the cache result after the file is deleted. This is useful for scenarios where files are regularly updated in the background.
		/// </summary>
		public bool RetainCacheOnDelete { get; init; }


		/// <remarks>
		/// A stale file will result in a re-fetching of that file TRUE. When FALSE, the cache re-uses the cached item.
		/// Scenarios:
		///   - file does not exist, stale = !RetainCacheOnDelete  (this means if Retain is true, it is not stale and value is reused)
		///   - file does exist and time is same since last fetch, stale = false 
		///   - file does exist and time is different to last fetch, stale = true
		/// </remarks>	
		protected override bool CheckStaleness(string key, CachedItem<TContent> item) 
			=> !File.Exists(key)
				? !RetainCacheOnDelete
				: !_lastModified.TryGetValue(key, out var lastKnownModifiedTime) || File.GetLastWriteTime(key) > lastKnownModifiedTime;


		protected override void OnItemFetching(string key) {
			base.OnItemFetching(key);
			_lastModified[key] = File.GetLastWriteTime(key);
		}

		protected override void OnItemRemoved(string key, CachedItem<TContent> val) {
			base.OnItemRemoved(key, val);
			_lastModified.Remove(key);
		}
	}

}
