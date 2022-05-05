namespace Hydrogen {

	public interface IItemHasher<in TItem> {
		byte[] Hash(TItem item);

		int DigestLength { get; }
	}

}