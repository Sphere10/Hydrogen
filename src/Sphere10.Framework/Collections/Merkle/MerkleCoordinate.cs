using System;
using System.Collections.Generic;

namespace Sphere10.Framework {

	/// <summary>
	/// Denotes the coordinate of a node within an merkle-tree.
	/// </summary>
	public record MerkleCoordinate : IEquatable<MerkleCoordinate> {
		public readonly int Level;
		public readonly int Index;

		private MerkleCoordinate(int level, int index) {
			Level = level;
			Index = index;
		}

		public bool IsRoot(MerkleSize size) {
			return Equals(Root(size));
		}

		public static MerkleCoordinate Null =>
			new MerkleCoordinate(-1, -1);

		public static MerkleCoordinate LeafAt(int index) {
			return From(0, index);
		}

		public static MerkleCoordinate From(int level, int index) {
			Guard.ArgumentInRange(level, 0, int.MaxValue, nameof(level));
			Guard.ArgumentInRange(index, 0, int.MaxValue, nameof(index));
			return new MerkleCoordinate(level, index);
		}

		public static MerkleCoordinate Root(int leafCount) {
			return Root(MerkleSize.FromLeafCount(leafCount));
		}

		public static MerkleCoordinate Root(MerkleSize size) {
			return size.Height == 0 ? Null : From(size.Height - 1, 0);
		}

		public static IEnumerable<MerkleCoordinate> SubRoots(int leafCount)
			=> MerkleMath.CalculateSubRoots(leafCount);

		public static MerkleCoordinate Next(MerkleCoordinate coordinate) {
			return From(coordinate.Level, coordinate.Index + 1);
		}

		public static MerkleCoordinate Last(MerkleSize size, int level) {
			return From(level, MerkleMath.CalculateLevelLength(size.LeafCount, level) - 1);
		}

		public int ToFlatIndex() {
			return (int)MerkleMath.ToFlatIndex(this);
		}

		public override int GetHashCode() {
			unchecked {
				return (Level * 397) ^ Index;
			}
		}

		public override string ToString() {
			return $"(L:{Level}, IX:{Index})";
		}
	
	}

}