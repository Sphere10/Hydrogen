namespace Hydrogen {
	public interface IFilePagedList<TItem> : IMemoryPagedList<TItem>  {
        string Path { get; }

    }
}
