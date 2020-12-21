namespace Sphere10.Framework {

	public interface IUpdateableMerkleTree : IMerkleTree {
		IExtendedList<byte[]> Leafs { get; }
	}

}
