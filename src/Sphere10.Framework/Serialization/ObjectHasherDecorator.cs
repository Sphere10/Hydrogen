namespace Sphere10.Framework {

	public class ObjectHasherDecorator<TItem> : ObjectSerializerDecorator<TItem>, IObjectHasher<TItem> {

		public ObjectHasherDecorator(IObjectSerializer<TItem> internalHasher) : base(internalHasher) {
		}

		protected new IObjectHasher<TItem> Internal => (IObjectHasher<TItem>)base.Internal;

		public virtual byte[] Hash(TItem @object) => Internal.Hash(@object);
	}

}