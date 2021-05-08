namespace Sphere10.Framework {

	public interface IItemHasher<in TItem> {
		byte[] Hash(TItem @object);
	}

}