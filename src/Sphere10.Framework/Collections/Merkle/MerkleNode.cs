using System;

namespace Sphere10.Framework {

	public record MerkleNode : IEquatable<MerkleNode> {
		public readonly MerkleCoordinate Coordinate;
		public readonly byte[] Hash;

		public MerkleNode(MerkleCoordinate coordinate, byte[] hash) {
			Coordinate = coordinate;
			Hash = hash;
		}

		public override int GetHashCode() {
			unchecked {
				return (Coordinate.GetHashCode() * 397) ^ ByteArrayEqualityComparer.Instance.GetHashCode(Hash);
			}
		}
	}

}