namespace Sphere10.Framework {

	public class ObjectSerializerDecorator<TItem> : ObjectSizerDecorator<TItem>, IObjectSerializer<TItem> {

		public ObjectSerializerDecorator(IObjectSerializer<TItem> internalSerializer) : base(internalSerializer) {
		}

		protected new IObjectSerializer<TItem> Internal => (IObjectSerializer<TItem>)base.Internal;

		public virtual int Serialize(TItem @object, EndianBinaryWriter writer) => Internal.Serialize(@object, writer);

		public virtual TItem Deserialize(int size, EndianBinaryReader reader) => Internal.Deserialize(size, reader);
	}

}