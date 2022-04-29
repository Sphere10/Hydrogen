namespace Sphere10.Framework {

	public class ItemHasherDecorator<TItem, TItemHasher> : IItemHasher<TItem> 
		where TItemHasher : IItemHasher<TItem> {

		protected readonly TItemHasher InternalHasher;

		public ItemHasherDecorator(TItemHasher internalHasher) {
			InternalHasher = internalHasher;
		}

		public virtual byte[] Hash(TItem item) => InternalHasher.Hash(item);

		public int DigestLength => InternalHasher.DigestLength;
	}

}