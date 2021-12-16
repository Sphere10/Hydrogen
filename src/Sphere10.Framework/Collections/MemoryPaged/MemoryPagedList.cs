using System;

namespace Sphere10.Framework {
	public class MemoryPagedList<TItem> : MemoryPagedListBase<TItem> {
	    private readonly IItemSizer<TItem> _sizer;

	    public MemoryPagedList(int pageSize, long maxMemory, int fixedItemSize)
		    : this(pageSize, maxMemory, new StaticSizeItemSizer<TItem>(fixedItemSize)) {
	    }

	    public MemoryPagedList(int pageSize, long maxMemory, Func<TItem, int> itemSizer)
		    : this(pageSize, maxMemory, new ActionItemSizer<TItem>(itemSizer)) {
	    }

	    private MemoryPagedList(int pageSize, long maxMemory, IItemSizer<TItem> sizer)
		    : base(pageSize, maxMemory) {
		    _sizer = sizer;
	    }

	    protected override IPage<TItem> NewPageInstance(int pageNumber) {
		    return new BinaryFormattedPage<TItem>(this.PageSize, _sizer);
	    }

		protected override IPage<TItem>[] LoadPages() {
			throw new NotSupportedException("Pages are not loadable across runtime sessions in this implementation. See FileMappedList class."); 
		}
	}

}