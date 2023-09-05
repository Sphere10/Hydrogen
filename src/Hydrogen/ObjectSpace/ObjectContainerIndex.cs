using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.ObjectSpace.MetaData;

public class ObjectContainerIndex<TItem, TKey> : IMetaDataLookup<TKey> {
	private readonly InMemoryMetaDataLookup<TKey> _indexLookup;
	private readonly ContainerMetaDataListener<TItem, TKey> _connector;

	public ObjectContainerIndex(ObjectContainer container, long reservedStreamIndex, Func<TItem, TKey> projection, IEqualityComparer<TKey> keyComparer, IItemSerializer<TKey> keySerializer) {

		_indexLookup = new InMemoryMetaDataLookup<TKey>(
			new ListBasedMetaDataStore<TKey>(
				container,
				reservedStreamIndex,
				0,
				keySerializer
			),
			keyComparer
		);

		_connector = new ContainerMetaDataListener<TItem, TKey>(
			container,
			_indexLookup,
			projection
		);
	}

	public ILookup<TKey, long> Lookup => _indexLookup.Lookup;

	public void Dispose() => _indexLookup.Dispose();

}
