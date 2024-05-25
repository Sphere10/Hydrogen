namespace Hydrogen;

public static class ItemSerializer<TItem> {
	public static IItemSerializer<TItem> Default => SerializerBuilder.FactoryAssemble<TItem>(SerializerFactory.Default);
}
