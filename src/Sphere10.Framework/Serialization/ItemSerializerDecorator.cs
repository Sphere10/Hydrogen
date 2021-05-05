namespace Sphere10.Framework {

	public class ItemSerializerDecorator<TItem, TSerializer> : ItemSizerDecorator<TItem, TSerializer>, IItemSerializer<TItem>
		where TSerializer : IItemSerializer<TItem> {

		public ItemSerializerDecorator(TSerializer internalSerializer)
			: base(internalSerializer) {
		}

		public virtual int Serialize(TItem @object, EndianBinaryWriter writer) => Internal.Serialize(@object, writer);

		public virtual TItem Deserialize(int size, EndianBinaryReader reader) => Internal.Deserialize(size, reader);
	}

	public class ItemSerializerDecorator<TItem> : ItemSerializerDecorator<TItem, IItemSerializer<TItem>> {
		public ItemSerializerDecorator(IItemSerializer<TItem> internalSerializer)
			: base(internalSerializer) {
		}

	}
}