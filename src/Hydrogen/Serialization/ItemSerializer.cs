namespace Hydrogen;

public static class ItemSerializer<TItem> {
	public static IItemSerializer<TItem> Default => SerializerFactory.Default.GetSerializer<TItem>();
}
