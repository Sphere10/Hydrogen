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

namespace Hydrogen;

internal class ObjectContainerIndex<TItem, TKey> : IMetaDataLookup<TKey> {
	private readonly InMemoryMetaDataLookup<TKey> _indexLookup;
	private readonly ObjectContainerMetaDataListener<TItem, TKey> _connector;
	private readonly Func<TItem, TKey> _projection;

	public ObjectContainerIndex(ObjectContainer container, long reservedStreamIndex, Func<TItem, TKey> projection, IEqualityComparer<TKey> keyComparer, IItemSerializer<TKey> keySerializer) {
		ReservedStreamIndex = reservedStreamIndex;
		_projection = projection;

		_indexLookup = new InMemoryMetaDataLookup<TKey>(
			new ListBasedMetaDataStore<TKey>(
				container,
				reservedStreamIndex,
				0,
				keySerializer
			),
			keyComparer
		);

		_connector = new ObjectContainerMetaDataListener<TItem, TKey>(
			container,
			_indexLookup,
			projection
		);
	}

	public long ReservedStreamIndex { get; }

	public TKey CalculateKey(TItem item) => _projection.Invoke(item);

	public ILookup<TKey, long> Lookup => _indexLookup.Lookup;

	public void Dispose() => _indexLookup.Dispose();
	
}
