using System;
using System.Collections.Generic;

namespace Sphere10.Framework {

	public readonly struct MerkleNode : IEquatable<MerkleNode> {
		public readonly MerkleCoordinate Coordinate;
		public readonly byte[] Hash;

		public MerkleNode(MerkleCoordinate coordinate, byte[] hash) {
			Coordinate = coordinate;
			Hash = hash;
		}

		public bool Equals(MerkleNode other) {
			return Coordinate.Equals(other.Coordinate) && ByteArrayEqualityComparer.EqualsImplementation(Hash, other.Hash);
		}

		public override bool Equals(object obj) {
			return obj is MerkleNode other && Equals(other);
		}

		public override int GetHashCode() {
			unchecked {
				return (Coordinate.GetHashCode() * 397) ^ ByteArrayEqualityComparer.Instance.GetHashCode(Hash);
			}
		}
	}

}