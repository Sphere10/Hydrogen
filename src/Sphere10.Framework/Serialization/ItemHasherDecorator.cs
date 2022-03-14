namespace Sphere10.Framework {

	public class ItemHasherDecorator<TItem, TObjectHasher> : IItemHasher<TItem> 
		where TObjectHasher : IItemHasher<TItem> {

		protected readonly TObjectHasher InternalHasher;

		public ItemHasherDecorator(TObjectHasher internalHasher) {
			InternalHasher = internalHasher;
		}

		public virtual byte[] Hash(TItem @object) => InternalHasher.Hash(@object);

		public int DigestLength => InternalHasher.DigestLength;
	}

}