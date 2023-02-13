namespace Hydrogen;

public interface IItemDigestor<TItem> : IItemSerializer<TItem>, IItemHasher<TItem> {
}
