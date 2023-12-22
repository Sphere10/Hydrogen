// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// A wrapper for an <see cref="IMetaDataStore{TData}"/> that caches the meta-data as an in-memory <see cref="ILookup{TKey,TValue}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the meta-data datum (and <see cref="ILookup{TKey,TValue}"/> key).</typeparam>
internal class MemoryCachedMetaDataLookup<TKey> : MemoryCachedMetaDataStoreBase<TKey> {

	private readonly ExtendedLookup<TKey, long> _lookup;

	public MemoryCachedMetaDataLookup(MetaDataStoreBase<TKey> innerMetaDataStore, IEqualityComparer<TKey> keyComparer = null)
		: base(innerMetaDataStore) {
		_lookup = new ExtendedLookup<TKey, long>(_ => new SortedList<long>(), keyComparer);
	}

	public ILookup<TKey, long> Lookup => _lookup;

	protected override void ClearMemory() => _lookup.Clear();

	protected override TKey ReadFromMemory(long index) => throw new NotSupportedException();

	protected override void AddToMemory(TKey key, long index) => _lookup.Add(key, index);

	protected override void UpdateMemory(TKey oldKey, TKey newKey, long index) {
		RemoveFromMemory(oldKey, index);
		AddToMemory(newKey, index);
	}

	protected override void InsertInMemory(TKey key, long index) {
		// scan entire index and increment all values greater than or equal to index
		ClearMemory();
		LoadMetaData();
	}

	protected override void RemoveFromMemory(TKey key, long index) => _lookup.Remove(key, index);

	protected override void ReapFromMemory(TKey data, long index) => RemoveFromMemory(data, index);

}