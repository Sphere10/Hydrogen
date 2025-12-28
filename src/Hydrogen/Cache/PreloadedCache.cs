// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// Bulk cache seeded with a fixed set of values, optionally keyed by a custom comparer.
/// </summary>
public sealed class PreloadedCache<TKey, TValue> : BulkFetchCacheBase<TKey, TValue> {
	private readonly IDictionary<TKey, TValue> _preloadedValues = null;

	/// <summary>
	/// Creates a cache that serves a predefined dictionary of values.
	/// </summary>
	/// <param name="preloadedValues">Key/value pairs to expose through the cache.</param>
	/// <param name="keyComparer">Optional key comparer.</param>
	public PreloadedCache(
		IDictionary<TKey, TValue> preloadedValues,
		IEqualityComparer<TKey> keyComparer = null
	) : base(ExpirationPolicy.None, null, true, NullValuePolicy.CacheNormally, StaleValuePolicy.AssumeNeverStale, keyComparer) {
		_preloadedValues = preloadedValues;
	}

	protected override IDictionary<TKey, TValue> BulkFetch() {
		return _preloadedValues;
	}

	protected override bool CheckStaleness(TKey key, CachedItem<TValue> item) => false;
}
