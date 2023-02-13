using System;

namespace Hydrogen {

	public class ItemSerializerDecorator<TItem, TSerializer> : ItemSizerDecorator<TItem, TSerializer>, IItemSerializer<TItem>
		where TSerializer : IItemSerializer<TItem> {

		public ItemSerializerDecorator(TSerializer internalSerializer)
			: base(internalSerializer) {
		}

		public virtual bool TrySerialize(TItem item, EndianBinaryWriter writer, out int bytesWritten)
			=> Internal.TrySerialize(item, writer, out bytesWritten);

		public virtual bool TryDeserialize(int byteSize, EndianBinaryReader reader, out TItem item) 
			=> Internal.TryDeserialize(byteSize, reader, out item);
	}

	public class ItemSerializerDecorator<TItem> : ItemSerializerDecorator<TItem, IItemSerializer<TItem>> {
		public ItemSerializerDecorator(IItemSerializer<TItem> internalSerializer)
			: base(internalSerializer) {
		}

	}
}