namespace Sphere10.Framework;

public interface IItemChecksum<in TItem> {
	int Calculate(TItem item);
}