using System;

namespace Sphere10.Framework {
	public interface IItemSerializer<TItem> : IItemSizer<TItem> {

		bool TrySerialize(TItem item, EndianBinaryWriter writer, out int bytesWritten);

		bool TryDeserialize(int byteSize, EndianBinaryReader reader, out TItem item);

		public TItem Deserialize(int byteSize, EndianBinaryReader reader) {
			if (!TryDeserialize(byteSize, reader, out var item))
				throw new InvalidOperationException("Unable to serialize object");
			return item;
		}

		public int Serialize(TItem @object, EndianBinaryWriter writer) {
			if (!TrySerialize(@object, writer, out var bytesWritten))
				throw new InvalidOperationException("Unable to serialize object");
			return bytesWritten;
		}
	}

}