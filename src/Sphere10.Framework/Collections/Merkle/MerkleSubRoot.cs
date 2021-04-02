using System;
using System.Collections.Generic;

namespace Sphere10.Framework {

	public record MerkleSubRoot {
		public readonly int Height;
		public readonly byte[] Hash;

		public MerkleSubRoot(int height, byte[] hash) {
			Height = height;
			Hash = hash;
		}
	}

}