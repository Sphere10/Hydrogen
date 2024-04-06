// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Used to store keys of an item in an <see cref="ObjectStream"/>. Used primarily for <see cref="StreamMappedDictionaryCLK{TKey,TValue}"/>"/> which
/// stores only the value part in the objectStream, the keys are stored in these (mapped to a reserved stream).
/// </summary>
/// <remarks>Unlike <see cref="MemberIndex{TItem,TKey}"/> which automatically extracts the key from the item and stores it, this is used as a primary storage for the key itself. Thus it is not an index, it is a pure store.</remarks>
/// <typeparam name="TKey"></typeparam>
internal class MemberStore<TKey> : MetaDataStoreDecorator<TKey> {

	public MemberStore(ClusteredStreams streams, long reservedStreamIndex, IEqualityComparer<TKey> keyComparer, IItemSerializer<TKey> keySerializer)
		: base(
			new MemoryCachedMetaDataLookup<TKey>(
				new ListBasedMetaDataStore<TKey>(
					streams,
					reservedStreamIndex,
					keySerializer
				),
				keyComparer
			)
		) {
	}

	public ILookup<TKey, long> Lookup {
		get {
			var store = ((MemoryCachedMetaDataLookup<TKey>)InnerStore);
			Guard.Ensure(store.IsAttached, "Key store is not attached");
			return store.Lookup;
		}
	}
}
