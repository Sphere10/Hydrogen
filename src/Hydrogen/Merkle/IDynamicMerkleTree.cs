namespace Hydrogen {


	public interface IDynamicMerkleTree : IMerkleTree {
		IExtendedList<byte[]> Leafs { get; }
	}

}
