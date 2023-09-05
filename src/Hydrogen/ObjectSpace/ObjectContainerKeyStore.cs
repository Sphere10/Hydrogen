using System;
using System.Collections.Generic;

namespace Hydrogen.ObjectSpace.MetaData;

public class ObjectContainerKeyStore<TKey> : IMetaDataDictionary<TKey> {
	private readonly InMemoryMetaDataDictionary<TKey> _index;

	public ObjectContainerKeyStore(ObjectContainer container, long reservedStreamIndex,  IEqualityComparer<TKey> keyComparer, IItemSerializer<TKey> keySerializer) {

		_index = new InMemoryMetaDataDictionary<TKey>(
			new ListBasedMetaDataStore<TKey>(
				container,
				reservedStreamIndex,
				0,
				keySerializer
			),
			keyComparer
		);

	}

	public IMetaDataStore<TKey> Store => _index;

	public IReadOnlyDictionary<TKey, long> Dictionary => _index.Dictionary;

	public void Dispose() => _index.Dispose();

}
