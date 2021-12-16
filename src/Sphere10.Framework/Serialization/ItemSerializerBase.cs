namespace Sphere10.Framework {

	public abstract class ItemSerializerBase<TItem> : ItemSizer<TItem>, IItemSerializer<TItem> {
		public abstract bool TrySerialize(TItem item, EndianBinaryWriter writer, out int bytesWritten);

		public abstract bool TryDeserialize(int byteSize, EndianBinaryReader reader, out TItem item);

	}
}