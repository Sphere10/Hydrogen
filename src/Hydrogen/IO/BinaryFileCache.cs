using System;
using System.Collections.Generic;
using System.IO;

namespace Hydrogen;

public class BinaryFileCache : FileCacheBase<byte[]> {

	public BinaryFileCache(
		CacheReapPolicy reapStrategy = CacheReapPolicy.LeastUsed,
		ExpirationPolicy expirationStrategy = ExpirationPolicy.SinceLastAccessedTime,
		long maxCapacity = int.MaxValue,
		TimeSpan? expirationDuration = null,
		IEqualityComparer<string> fileNameComparer = null,
		ICacheReaper reaper = null
	) : base(reapStrategy, expirationStrategy, maxCapacity, expirationDuration, fileNameComparer, reaper){
	}

	protected override long EstimateSize(byte[] value) 
		=> value.Length * sizeof(byte);

	protected override byte[] Fetch(string key) 
		=> File.ReadAllBytes(key);
		
}
