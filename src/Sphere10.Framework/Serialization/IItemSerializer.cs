namespace Sphere10.Framework {
	public interface IItemSerializer<TItem> : IItemSizer<TItem> {

		int Serialize(TItem @object, EndianBinaryWriter writer);

		TItem Deserialize(int size, EndianBinaryReader reader);

	}

}