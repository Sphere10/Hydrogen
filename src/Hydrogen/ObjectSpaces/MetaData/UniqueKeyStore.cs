// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Used to store keys of an item in an <see cref="ObjectContainer"/>. Used primarily for <see cref="StreamMappedDictionaryCLK{TKey,TValue}"/>"/> which
/// stores only the value part in the container, the keys are stored in these (mapped to a reserved stream).
/// </summary>
/// <remarks>Unlike <see cref="KeyIndex{TItem,TKey}"/> which automatically extracts the key from the item and stores it, this is used as a primary storage for the key itself. Thus it is not an index, it is a pure store.</remarks>
/// <typeparam name="TKey"></typeparam>
internal class UniqueKeyStore<TKey> : MetaDataStoreDecorator<TKey> {

	public UniqueKeyStore(ClusteredStreams container, long reservedStreamIndex, IEqualityComparer<TKey> keyComparer, IItemSerializer<TKey> keySerializer)
		: base(
			new MemoryCachedMetaDataDictionary<TKey>(
				new ListBasedMetaDataStore<TKey>(
					container,
					reservedStreamIndex,
					keySerializer
				),
				keyComparer
			)
		) {
	}

	public IReadOnlyDictionary<TKey, long> Dictionary {
		get {
			var store = ((MemoryCachedMetaDataDictionary<TKey>)InnerStore);
			Guard.Ensure(store.IsAttached, "Key store is not attached");
			return store.Dictionary;
		}
	}
}
