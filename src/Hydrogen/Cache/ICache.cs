// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

/// <summary>
/// Defines a synchronized, size-aware cache of key/value pairs with expiration and reaping semantics.
/// </summary>
public interface ICache : ISynchronizedObject {

	/// <summary>
	/// Raised immediately before a cache miss triggers a fetch.
	/// </summary>
	event EventHandlerEx<object> ItemFetching;

	/// <summary>
	/// Raised after an item has been fetched and stored.
	/// </summary>
	event EventHandlerEx<object, object> ItemFetched;

	/// <summary>
	/// Raised when an item is removed from the cache (explicitly or through reaping).
	/// </summary>
	event EventHandlerEx<object, CachedItem> ItemRemoved;

	/// <summary>
	/// All cached entries regardless of expiration state.
	/// </summary>
	IEnumerable<CachedItem> CachedItems { get; }

	/// <summary>
	/// Number of items currently cached.
	/// </summary>
	int ItemCount { get; }

	/// <summary>
	/// Aggregate size of cached items as estimated by the cache implementation.
	/// </summary>
	long CurrentSize { get; }

	/// <summary>
	/// Maximum size allowed before reaping occurs.
	/// </summary>
	long MaxCapacity { get; }

	/// <summary>
	/// Timestamp of the last write operation.
	/// </summary>
	DateTime LastUpdateOn { get; }

	/// <summary>
	/// Timestamp of the last read or write.
	/// </summary>
	DateTime LastAccessedOn { get; }

	/// <summary>
	/// Count of read operations performed against the cache.
	/// </summary>
	long TotalAccesses { get; }

	/// <summary>
	/// Duration after which items become expired, based on <see cref="ExpirationPolicy"/>.
	/// </summary>
	TimeSpan ExpirationDuration { get; }

	/// <summary>
	/// Determines how expiration windows are measured.
	/// </summary>
	ExpirationPolicy ExpirationPolicy { get; }

	/// <summary>
	/// Policy used to pick victims when freeing space.
	/// </summary>
	CacheReapPolicy ReapPolicy { get; }

	/// <summary>
	/// Specifies how cached <c>null</c> values are handled.
	/// </summary>
	NullValuePolicy NullValuePolicy { get; }

	/// <summary>
	/// Checks whether a non-expired item exists for the supplied key.
	/// </summary>
	bool ContainsCachedItem(object key);

	/// <summary>
	/// Gets the cached item for the specified key, fetching and storing it when absent or expired.
	/// </summary>
	CachedItem Get(object key);

	/// <summary>
	/// Updates or inserts a cached value for the supplied key.
	/// </summary>
	void Set(object key, object value);

	/// <summary>
	/// Bulk loads pre-fetched values into the cache without triggering fetch callbacks.
	/// </summary>
	void BulkLoad(IEnumerable<KeyValuePair<object, object>> bulkLoadedValues);

	/// <summary>
	/// Marks an item as invalid without immediately removing it.
	/// </summary>
	void Invalidate(object key);

	/// <summary>
	/// Removes the item for the supplied key.
	/// </summary>
	void Remove(object key);

	/// <summary>
	/// Evicts all items and releases their resources.
	/// </summary>
	void Purge();

	/// <summary>
	/// Indexer for retrieving or setting cached values by key.
	/// </summary>
	object this[object index] { get; set; }

}


/// <summary>
/// Strongly-typed cache variant.
/// </summary>
/// <typeparam name="TKey">Key type.</typeparam>
/// <typeparam name="TValue">Value type.</typeparam>
public interface ICache<TKey, TValue> : ICache {

	/// <inheritdoc cref="ICache.ItemFetched"/>
	new event EventHandlerEx<TKey, TValue> ItemFetched;

	/// <inheritdoc cref="ICache.ItemRemoved"/>
	new event EventHandlerEx<TKey, CachedItem<TValue>> ItemRemoved;

	/// <summary>
	/// Cached entries with value type awareness.
	/// </summary>
	new IEnumerable<CachedItem<TValue>> CachedItems { get; }

	/// <inheritdoc cref="ICache.ContainsCachedItem(object)"/>
	bool ContainsCachedItem(TKey key);

	/// <inheritdoc cref="ICache.Get(object)"/>
	CachedItem<TValue> Get(TKey key);

	/// <inheritdoc cref="ICache.Set(object, object)"/>
	void Set(TKey key, TValue value);

	/// <inheritdoc cref="ICache.BulkLoad(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{object, object}})"/>
	void BulkLoad(IEnumerable<KeyValuePair<TKey, TValue>> bulkLoadedValues);

	/// <inheritdoc cref="ICache.Invalidate(object)"/>
	void Invalidate(TKey key);

	/// <inheritdoc cref="ICache.Remove(object)"/>
	void Remove(TKey key);

	/// <summary>
	/// Strongly-typed indexer for retrieving or setting cached values by key.
	/// </summary>
	TValue this[TKey index] { get; set; }

}
