// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.DApp.Core.Blockchain;

/// <summary>
/// Used for handling block-trees from which a blockchain exists within. The purpose is to select a branch of that tree
/// as the official blockchain.  
/// </summary>
public interface IBlockTree {

	/// <summary>
	/// The block-tree is maintained from the finality block, which denotes the most recent block which
	/// can be be regressed via a re-org.
	/// </summary>
	BlockTreeNode FinalityBlock { get; }

}


public class BlockTreeNode {

	BlockGenealogy Genealogy { get; }

	public long BlockID { get; }

	public long[] NextBlocks { get; }

}


public enum BlockGenealogy {
	Descendent,
	Orphan,
	Invalid
}
