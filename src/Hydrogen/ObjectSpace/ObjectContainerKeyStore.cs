// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

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
