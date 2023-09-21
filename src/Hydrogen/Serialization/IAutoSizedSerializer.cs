namespace Hydrogen;

public interface IAutoSizedSerializer<TItem> : IItemSerializer<TItem>, IAutoSizedSerializer {
	TItem Deserialize(EndianBinaryReader reader);

	object IAutoSizedSerializer.DeserializeInternal(EndianBinaryReader reader) => Deserialize(reader);
}
