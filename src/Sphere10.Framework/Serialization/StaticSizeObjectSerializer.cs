namespace Sphere10.Framework {

	public abstract class StaticSizeObjectSerializer<TItem> : StaticSizeItemSizer<TItem>, IItemSerializer<TItem> {
		protected StaticSizeObjectSerializer(int fixedSize) : base(fixedSize) {
		}
		
		public abstract bool TrySerialize(TItem item, EndianBinaryWriter writer, out int bytesWritten);

		public abstract bool TryDeserialize(int byteSize, EndianBinaryReader reader, out TItem item);
	}

}
