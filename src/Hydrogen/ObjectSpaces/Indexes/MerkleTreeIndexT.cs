// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.ObjectSpaces;

/// <inheritdoc />
internal class MerkleTreeIndex<TItem> : MerkleTreeIndex {
	public MerkleTreeIndex(ObjectContainer<TItem> objectContainer, CHF hashAlgorithm, long reservedStreamIndex)
		: base(
			objectContainer,
			objectContainer.StreamContainer.ReadAll,
			hashAlgorithm,
			reservedStreamIndex
		) {
	}

	protected new ObjectContainer<TItem> Container => (ObjectContainer<TItem>)base.Container;
}
