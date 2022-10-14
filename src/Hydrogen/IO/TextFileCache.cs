using System;
using System.Collections.Generic;
using System.IO;

namespace Hydrogen;

public class TextFileCache : FileCacheBase<string> {

	public TextFileCache(
		CacheReapPolicy reapStrategy = CacheReapPolicy.LeastUsed,
		ExpirationPolicy expirationStrategy = ExpirationPolicy.SinceLastAccessedTime,
		long maxCapacity = int.MaxValue,
		TimeSpan? expirationDuration = null,
		IEqualityComparer<string> fileNameComparer = null,
		ICacheReaper reaper = null
	) : base(reapStrategy, expirationStrategy, maxCapacity, expirationDuration, fileNameComparer, reaper){
	}

	protected override long EstimateSize(string value) 
		=> value.Length * sizeof(char);

	protected override string Fetch(string key) 
		=> File.ReadAllText(key);
		
}
