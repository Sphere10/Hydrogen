namespace Sphere10.Framework {

	public interface IMerklizedCollection<TItem> : IExtendedCollection<TItem> {
		IMerkleTree MerkleTree { get; }
	}

}
