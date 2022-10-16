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

		protected override bool CheckStaleness(string key, CachedItem<TContent> item) 
			=>  !_lastModified.TryGetValue(key, out var lastKnownModifiedTime) || File.GetLastWriteTime(key) > lastKnownModifiedTime; 

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
