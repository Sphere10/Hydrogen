// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.
//
// NOTE: This file is part of the reference implementation for Dynamic Merkle-Trees. Read the paper at:
// Web: https://sphere10.com/tech/dynamic-merkle-trees
// e-print: https://vixra.org/abs/2305.0087

namespace Hydrogen;

/// <summary>
/// A merkleized object is one that maintains a merkle tree of it's state.
/// </summary>
public interface IMerkleObject {
	/// <summary>
	/// Gets the merkle tree that represents the object's current state.
	/// </summary>
	IMerkleTree MerkleTree { get; }

	// TODO: add proof building scope
	//  using (merkleCollection.EnterBuildProofScope()) {
	//     merkleCollection.AddRange(...);
	//     
	//  
}
