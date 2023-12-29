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
/// An index for an <see cref="ObjectContainer{TItem}"/> that tracks a key of type <see cref="TKey"/> from items of type <see cref="{TItem}"/>
/// being tracked in the container.
/// </summary>
/// <typeparam name="TItem">The type of the item stored in the container</typeparam>
/// <typeparam name="TKey">The type of the key from the item being indexed</typeparam>
internal class KeyIndex<TItem, TKey> : IndexBase<TItem, TKey, MemoryCachedMetaDataLookup<TKey>> {
	public KeyIndex(ObjectContainer<TItem> container, long reservedStreamIndex, Func<TItem, TKey> projection, IEqualityComparer<TKey> keyComparer, IItemSerializer<TKey> keySerializer)
		: base(
			container,
			projection,
			new MemoryCachedMetaDataLookup<TKey>(
				new ListBasedMetaDataStore<TKey>(
					container, 
					reservedStreamIndex, 
					keySerializer
				),
				keyComparer
			)
		) {
	}

	public ILookup<TKey, long> Lookup => KeyStore.Lookup;

}
