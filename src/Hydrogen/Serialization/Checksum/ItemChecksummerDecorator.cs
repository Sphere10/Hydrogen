namespace Hydrogen;

public class ItemChecksummerDecorator<TItem, TInner> : IItemChecksummer<TItem> where TInner : IItemChecksummer<TItem> {
	internal readonly TInner InnerChecksummer;

	public ItemChecksummerDecorator(TInner innerChecksummer) {
		InnerChecksummer = innerChecksummer;
	}

	public virtual int CalculateChecksum(TItem item) => InnerChecksummer.CalculateChecksum(item);
}

public class ItemChecksummerDecorator<TItem> : ItemChecksummerDecorator<TItem, IItemChecksummer<TItem>> {
	public ItemChecksummerDecorator(IItemChecksummer<TItem> innerChecksummer) 
		: base(innerChecksummer) {
	}
}