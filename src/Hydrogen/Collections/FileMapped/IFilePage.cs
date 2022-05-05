namespace Hydrogen {

	public interface IFilePage<TItem> : IMemoryPage<TItem> {
        
        long StartPosition { get; set; }
        
        long EndPosition { get; set; }

    }

}
