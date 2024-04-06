// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Reflection;

namespace Hydrogen;

/// <summary>
/// Used to maintain a merkle-tree of an <see cref="ObjectStream"/>'s items. The merkle-tree is stored within a reserved stream within the objectStream.
/// </summary>
/// <remarks>When fetching item bytes for hashing, a key-value-pair with same key but empty/null value will result in the same digest.</remarks>
internal class MerkleTreeIndex : IndexBase<byte[], MerkleTreeStore> {

	private readonly IItemHasher<long> _itemHasher;

	public MerkleTreeIndex(
		ObjectStream objectStream,
		long reservedStreamIndex,
		IItemHasher<long> itemHasher,
		CHF chf
	) : base(objectStream, new MerkleTreeStore(objectStream.Streams, reservedStreamIndex, chf, false)) {
		_itemHasher = itemHasher;
	}

	public IMerkleTree MerkleTree => Store.MerkleTree;

	protected override void OnAdded(object item, long index) 
		=> Store.Add(index, _itemHasher.Hash(index));

	protected override void OnInserted(object item, long index) 
		=> Store.Insert(index, _itemHasher.Hash(index));

	protected override void OnUpdated(object item, long index) 
		=> Store.Update(index,  _itemHasher.Hash(index));

}