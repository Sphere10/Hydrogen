namespace Sphere10.Framework;

public sealed class WithNullValueItemHasher<TItem, TItemHasher> : ItemHasherDecorator<TItem, TItemHasher> 
	where TItemHasher : IItemHasher<TItem> {

	private readonly byte[] _nullItemHash;
	public WithNullValueItemHasher(TItemHasher internalHasher, byte[] nullItemHash) 
		: base(internalHasher) {
		_nullItemHash = nullItemHash;
		;
	}

	public override byte[] Hash(TItem item) 
		=> item == null ? Tools.Array.Clone(_nullItemHash) : base.Hash(item);
}
