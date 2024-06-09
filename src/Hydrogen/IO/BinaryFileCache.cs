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

public class BinaryFileCache : FileCacheBase<byte[]> {

	public BinaryFileCache(
		CacheReapPolicy reapStrategy = CacheReapPolicy.LeastUsed,
		ExpirationPolicy expirationStrategy = ExpirationPolicy.SinceLastAccessedTime,
		long maxCapacity = int.MaxValue,
		TimeSpan? expirationDuration = null,
		IEqualityComparer<string> fileNameComparer = null,
		ICacheReaper reaper = null
	) : base(reapStrategy, expirationStrategy, maxCapacity, expirationDuration, fileNameComparer, reaper) {
	}

	protected override long EstimateSize(byte[] value)
		=> value.Length * sizeof(byte);

	protected override byte[] Fetch(string key)
		=> File.ReadAllBytes(key);

}
