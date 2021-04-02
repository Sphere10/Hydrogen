namespace Sphere10.Framework {

	public class ObjectHasherDecorator<TItem, TObjectHasher> : IObjectHasher<TItem> 
		where TObjectHasher : IObjectHasher<TItem> {

		protected readonly TObjectHasher InternalHasher;

		public ObjectHasherDecorator(TObjectHasher internalHasher) {
			InternalHasher = internalHasher;
		}

		public virtual byte[] Hash(TItem @object) => InternalHasher.Hash(@object);
	}

}