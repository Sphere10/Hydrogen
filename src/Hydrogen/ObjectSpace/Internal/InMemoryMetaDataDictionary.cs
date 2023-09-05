﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hydrogen.ObjectSpace.MetaData;

internal class InMemoryMetaDataDictionary<TKey> : InMemoryMetaDataStoreBase<TKey>, IMetaDataDictionary<TKey> {
	
	private readonly IReadOnlyDictionary<TKey, long> _inMemoryIndexReadOnly;
	private readonly IDictionary<TKey, long> _inMemoryIndex;
	private readonly IEqualityComparer<TKey> _keyComparer;
	
	public InMemoryMetaDataDictionary(IMetaDataStore<TKey> metaDataStore, IEqualityComparer<TKey> keyComparer = null) 
		: base(metaDataStore) {
		_inMemoryIndex = new Dictionary<TKey, long>(keyComparer);
		_inMemoryIndexReadOnly = new ReadOnlyDictionary<TKey, long>(_inMemoryIndex);
		_keyComparer = keyComparer;
	}

	public IReadOnlyDictionary<TKey, long> Dictionary { 
		get {
			//CheckAttached();
			return _inMemoryIndexReadOnly;
		}
	}

	protected override void ValidateMemoryAdd(TKey key, long index) => Guard.Against(Dictionary.ContainsKey(key), $"Key '{key}' already exists in index.");

	protected override void ValidateMemoryUpdate(TKey oldKey, TKey newKey, long index) => Guard.Against(!_keyComparer.Equals(oldKey, newKey) && Dictionary.ContainsKey(newKey), $"Key '{newKey}' already exists in index.");

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
