namespace Sphere10.Framework {

	public record MerkleSize {
		public int LeafCount;
		public int Height;

		public static MerkleSize FromLeafCount(int leafCount) {
			Guard.ArgumentInRange(leafCount, 0, int.MaxValue, nameof(leafCount));
			return new MerkleSize {
				LeafCount = leafCount,
				Height = MerkleMath.CalculateHeight(leafCount)
			};
		}

		public override string ToString() {
			return $"(H:{Height}, LC:{LeafCount})";
		}

	}

}