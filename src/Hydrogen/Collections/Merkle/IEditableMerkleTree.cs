namespace Sphere10.Framework {

	public interface IEditableMerkleTree : IMerkleTree {
		IExtendedList<byte[]> Leafs { get; }
	}

}
