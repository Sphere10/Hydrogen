namespace Hydrogen {

	public record MerkleSubRoot {
		public readonly int Height;
		public readonly byte[] Hash;

		public MerkleSubRoot(int height, byte[] hash) {
			Height = height;
			Hash = hash;
		}
	}

}