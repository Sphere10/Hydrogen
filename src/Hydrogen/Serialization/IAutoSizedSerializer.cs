namespace Hydrogen;

public interface IAutoSizedSerializer<TItem> : IItemSerializer<TItem> {
	TItem Deserialize(EndianBinaryReader reader);
}
