namespace Sphere10.Framework {

	public abstract class ItemSerializerBase<TItem> : ItemSizer<TItem>, IItemSerializer<TItem> {
		public abstract TItem Deserialize(int size, EndianBinaryReader reader);

		public abstract int Serialize(TItem item, EndianBinaryWriter writer);

	}
}