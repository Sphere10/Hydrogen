// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Used to maintain a merkle-tree of an <see cref="ObjectStream"/>'s items. The merkle-tree is stored within a reserved stream within the objectStream.
/// </summary>
/// <remarks>When fetching item bytes for hashing, a key-value-pair with same key but empty/null value will result in the same digest.</remarks>
internal class MerkleTreeIndex : IndexBase<MerkleTreeStorageAttachment> {

	private readonly IItemHasher<long> _itemHasher;

	public MerkleTreeIndex(ObjectStream objectStream, string indexName, IItemHasher<long> itemHasher, CHF chf) 
		: base(objectStream, new MerkleTreeStorageAttachment(objectStream.Streams, indexName, chf, false)) {
		_itemHasher = itemHasher;
	}

	public IMerkleTree MerkleTree => Store;

	protected override void OnAdded(object item, long index) 
		=> Store.Leafs.Add(_itemHasher.Hash(index));

	protected override void OnInserted(object item, long index) 
		=> Store.Leafs.Insert(index, _itemHasher.Hash(index));

	protected override void OnUpdated(object item, long index) 
		=> Store.Leafs.Update(index,  _itemHasher.Hash(index));

	protected override void OnRemoved(long index) 
		=> Store.Leafs.RemoveAt(index);

	protected override void OnReaped(long index) {
		var digest = Hashers.ZeroHash(MerkleTree.HashAlgorithm);
		Store.Leafs.Update(index, digest);
	}
	
	protected override void OnContainerClearing() {
		// Inform the store to clear
		Store.Leafs.Clear();

		// When the object stream about to be cleared, we detach the observer
		CheckAttached();
		Store.Detach();
	}

	protected override void OnContainerCleared() {
		// After ObjectStream was cleared, we reboot the index
		CheckDetached();
		Store.Attach();
	}
}
