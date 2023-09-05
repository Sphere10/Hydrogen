using System;
using System.Collections.Generic;

namespace Hydrogen.ObjectSpace.MetaData;

public class ObjectContainerUniqueKey<TItem, TKey> : IMetaDataDictionary<TKey> {
	private readonly InMemoryMetaDataDictionary<TKey> _index;
	private readonly ContainerMetaDataListener<TItem, TKey> _connector;

	public ObjectContainerUniqueKey(ObjectContainer container, long reservedStreamIndex, Func<TItem, TKey> projection, IEqualityComparer<TKey> keyComparer, IItemSerializer<TKey> keySerializer) {

		_index = new InMemoryMetaDataDictionary<TKey>(
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
			_index,
			projection
		);
	}

	public IReadOnlyDictionary<TKey, long> Dictionary => _index.Dictionary;

	public void Dispose() => _index.Dispose();

}
