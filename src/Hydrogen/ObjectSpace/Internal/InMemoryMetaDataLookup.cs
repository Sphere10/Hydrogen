using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.ObjectSpace.MetaData;

internal class InMemoryMetaDataLookup<TKey> : InMemoryMetaDataStoreBase<TKey>, IMetaDataLookup<TKey> {
	
	private readonly ExtendedLookup<TKey, long> _lookup;
	
	public InMemoryMetaDataLookup(IMetaDataStore<TKey> innerMetaDataStore, IEqualityComparer<TKey> keyComparer = null) 
		: base(innerMetaDataStore) {
		_lookup = new ExtendedLookup<TKey, long>(_ => new SortedList<long>(), keyComparer);
	}

	public ILookup<TKey, long> Lookup { 
		get {
			//CheckAttached();
			return _lookup;
		}
	}

	protected override void ClearMemory() => _lookup.Clear();

	protected override TKey ReadFromMemory(long index) => throw new NotSupportedException(); 

	protected override void AddToMemory(TKey key, long index) => _lookup.Add(key, index);

	protected override void UpdateMemory(TKey oldKey, TKey newKey, long index) {
		RemoveFromMemory(oldKey, index);
		AddToMemory(newKey, index);
	}

	protected override void InsertInMemory(TKey key, long index) {
		// scan entire index and increment all values greater than or equal to index
		base.LoadMetaData();
	}
	
	protected override void RemoveFromMemory(TKey key, long index) => _lookup.Remove(key, index);

	protected override void ReapFromMemory(TKey data, long index) => RemoveFromMemory(data, index);

}