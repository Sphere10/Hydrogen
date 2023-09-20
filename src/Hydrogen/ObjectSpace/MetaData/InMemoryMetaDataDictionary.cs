// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hydrogen;

internal class InMemoryMetaDataDictionary<TKey> : InMemoryMetaDataStoreBase<TKey>, IMetaDataDictionary<TKey> {
	
	private readonly IReadOnlyDictionary<TKey, long> _inMemoryIndexReadOnly;
	private readonly IDictionary<TKey, long> _inMemoryIndex;
	

	public InMemoryMetaDataDictionary(IMetaDataStore<TKey> metaDataStore, IEqualityComparer<TKey> keyComparer = null) 
		: base(metaDataStore) {
		_inMemoryIndex = new Dictionary<TKey, long>(keyComparer);
		_inMemoryIndexReadOnly = new ReadOnlyDictionary<TKey, long>(_inMemoryIndex);
		KeyComparer = keyComparer;
	}

	public IReadOnlyDictionary<TKey, long> Dictionary { 
		get {
			//CheckAttached();
			return _inMemoryIndexReadOnly;
		}
	}

	public IItemSerializer<TKey> KeySerializer => InnerStore.DatumSerializer;

	public IEqualityComparer<TKey> KeyComparer { get; }

	protected override void ValidateMemoryAdd(TKey key, long index) => Guard.Against(Dictionary.ContainsKey(key), $"Key '{key}' already exists in index.");

	protected override void ValidateMemoryUpdate(TKey oldKey, TKey newKey, long index) => Guard.Against(!KeyComparer.Equals(oldKey, newKey) && Dictionary.ContainsKey(newKey), $"Key '{newKey}' already exists in index.");

	protected override void ValidateMemoryInsert(TKey key, long index) => Guard.Against(Dictionary.ContainsKey(key), $"Key '{key}' already exists in index.");

	protected override void ValidateMemoryRemove(TKey key, long index) => Guard.Ensure(!Dictionary.ContainsKey(key), $"Key '{key}' does not exist in index.");

	protected override void ClearMemory() => _inMemoryIndex.Clear();

	protected override TKey ReadFromMemory(long index) => throw new NotSupportedException();

	protected override void AddToMemory(TKey key, long index) => _inMemoryIndex.Add(key, index);

	protected override void UpdateMemory(TKey oldKey, TKey newKey, long index) {
		if (oldKey is not null) // oldKey can be null for a reaped item
			RemoveFromMemory(oldKey, index);
		AddToMemory(newKey, index);
	}

	protected override void InsertInMemory(TKey key, long index) {
		base.LoadMetaData();
	}

	protected override void RemoveFromMemory(TKey key, long index) => _inMemoryIndex.Remove(key);

	protected override void ReapFromMemory(TKey data, long index) => RemoveFromMemory(data, index);
}
