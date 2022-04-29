namespace Hydrogen {

	public interface IEditableMerkleTree : IMerkleTree {
		IExtendedList<byte[]> Leafs { get; }
	}

}
