namespace Sphere10.Framework {
    public interface IFilePagedList<TItem> : IMemoryPagedList<TItem>  {
        string Path { get; }

    }
}
