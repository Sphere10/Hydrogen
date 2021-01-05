namespace Sphere10.Framework {
    public interface IFilePagedList<TItem, TPage> : IMemoryPagedList<TItem, TPage> where TPage : IFilePage<TItem> {
        string Path { get; }

    }
}
