// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;

namespace Hydrogen;

public abstract class FileCacheBase<TContent> : CacheBase<string, TContent> {
	protected FileCacheBase(
		CacheReapPolicy reapStrategy = CacheReapPolicy.LeastUsed,
		ExpirationPolicy expirationStrategy = ExpirationPolicy.SinceLastAccessedTime,
		long maxCapacity = int.MaxValue,
		TimeSpan? expirationDuration = null,
		IEqualityComparer<string> fileNameComparer = null,
		ICacheReaper reaper = null
	) : base(reapStrategy, expirationStrategy, maxCapacity, expirationDuration, NullValuePolicy.Throw, StaleValuePolicy.CheckStaleOnDemand, fileNameComparer, reaper) {
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
			: item.FetchedOn < File.GetLastWriteTime(key);
}
