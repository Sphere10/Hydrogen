using System;

namespace Sphere10.Framework {

	public class ItemSerializerDecorator<TItem, TSerializer> : ItemSizerDecorator<TItem, TSerializer>, IItemSerializer<TItem>
		where TSerializer : IItemSerializer<TItem> {

		public ItemSerializerDecorator(TSerializer internalSerializer)
			: base(internalSerializer) {
		}
		
		public bool TrySerialize(TItem item, EndianBinaryWriter writer, out int bytesWritten) {
			try {
				bytesWritten = Internal.Serialize(item, writer);
				return true;
			} catch (Exception) {
				bytesWritten = 0;
				return false;
			}
		}

		public bool TryDeserialize(int byteSize, EndianBinaryReader reader, out TItem item) {
			try {
				item = Internal.Deserialize(byteSize, reader);
				return true;
			} catch (Exception) {
				item = default;
				return false;
			}
		}
	}

	public class ItemSerializerDecorator<TItem> : ItemSerializerDecorator<TItem, IItemSerializer<TItem>> {
		public ItemSerializerDecorator(IItemSerializer<TItem> internalSerializer)
			: base(internalSerializer) {
		}

	}
}