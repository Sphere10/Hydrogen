// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <inheritdoc />
/// <summary>
/// Generic wrapper that wires type parameters for <see cref="MerkleTreeIndex"/> creation.
/// </summary>
internal class MerkleTreeIndex<TItem> : MerkleTreeIndex {
	public MerkleTreeIndex(ObjectStream<TItem> objectStream, string indexName, IItemHasher<long> itemHasher, CHF hashAlgorithm)
		: base(
			objectStream,
			indexName,
			itemHasher,
			hashAlgorithm
		) {
	}
}
