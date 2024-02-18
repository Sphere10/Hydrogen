// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Used to maintain a merkle-tree of an <see cref="ObjectContainer"/>'s items. The merkle-tree is stored within a reserved stream within the container.
/// </summary>
/// <remarks>When fetching item bytes for hashing, a key-value-pair with same key but empty/null value will result in the same digest.</remarks>
internal class MerkleTreeIndex : IndexBase<byte[], MerkleTreeStore> {

	private readonly Func<long, byte[]> _itemDigestor;

	public MerkleTreeIndex(
		ObjectContainer objectContainer,
		long reservedStreamIndex,
		Func<long, byte[]> itemDigestor,
		CHF chf
	) : base(objectContainer, new MerkleTreeStore(objectContainer, reservedStreamIndex, chf)) {
		_itemDigestor = itemDigestor;
	}

	public IMerkleTree MerkleTree => KeyStore.MerkleTree;

	protected override void OnAdded(object item, long index) 
		=> KeyStore.Add(index, _itemDigestor(index));

	protected override void OnInserted(object item, long index) 
		=> KeyStore.Insert(index, _itemDigestor(index));

	protected override void OnUpdated(object item, long index) 
		=> KeyStore.Update(index,  _itemDigestor(index));

}
