namespace Sphere10.Framework {

	public class ObjectSerializerDecorator<TItem, TSerializer> : ObjectSizerDecorator<TItem, TSerializer>, IObjectSerializer<TItem>
		where TSerializer : IObjectSerializer<TItem> {

		public ObjectSerializerDecorator(TSerializer internalSerializer)
			: base(internalSerializer) {
		}

		public virtual int Serialize(TItem @object, EndianBinaryWriter writer) => Internal.Serialize(@object, writer);

		public virtual TItem Deserialize(int size, EndianBinaryReader reader) => Internal.Deserialize(size, reader);
	}

	public class ObjectSerializerDecorator<TItem> : ObjectSerializerDecorator<TItem, IObjectSerializer<TItem>> {
		public ObjectSerializerDecorator(IObjectSerializer<TItem> internalSerializer)
			: base(internalSerializer) {
		}

	}
}