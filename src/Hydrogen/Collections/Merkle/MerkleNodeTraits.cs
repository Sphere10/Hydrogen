using System;

namespace Hydrogen {

	[Flags]
	public enum MerkleNodeTraits : ushort {
		Root           = 1 << 0,
		Leaf           = 1 << 1,
		Perfect      = 1 << 2,
		IsLeftChild    = 1 << 3,
		IsRightChild   = 1 << 4,
		BubblesUp      = 1 << 5,
		HasLeftChild   = 1 << 6,
		HasRightChild  = 1 << 7,
		BubbledUp      = 1 << 8,
	}

}