namespace Hydrogen;

public interface IItemChecksum<in TItem> {
	int Calculate(TItem item);
}