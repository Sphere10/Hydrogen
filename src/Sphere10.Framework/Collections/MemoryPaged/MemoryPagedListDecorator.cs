namespace Sphere10.Framework {
	public class MemoryPagedListDecorator<TItem, TMemoryPagedList> : PagedListDecorator<TItem, TMemoryPagedList>, IMemoryPagedList<TItem> where TMemoryPagedList : IMemoryPagedList<TItem> { 
        public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoading { add => InternalExtendedList.PageLoading += value; remove => InternalExtendedList.PageLoading -= value; }
        public event EventHandlerEx<object, IMemoryPage<TItem>> PageLoaded { add => InternalExtendedList.PageLoaded += value; remove => InternalExtendedList.PageLoaded -= value; }
        public event EventHandlerEx<object, IMemoryPage<TItem>> PageSaving { add => InternalExtendedList.PageSaving += value; remove => InternalExtendedList.PageSaving -= value; }
        public event EventHandlerEx<object, IMemoryPage<TItem>> PageSaved { add => InternalExtendedList.PageSaved += value; remove => InternalExtendedList.PageSaved -= value; }
        public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloading { add => InternalExtendedList.PageUnloading += value; remove => InternalExtendedList.PageUnloading -= value; }
        public event EventHandlerEx<object, IMemoryPage<TItem>> PageUnloaded { add => InternalExtendedList.PageUnloaded += value; remove => InternalExtendedList.PageUnloaded -= value; }

        public MemoryPagedListDecorator(TMemoryPagedList internalPagedList)
            : base(internalPagedList) {
            internalPagedList.PageLoading += (o, p) => OnPageLoading(p);
            internalPagedList.PageLoaded += (o, p) => OnPageLoaded(p);
            internalPagedList.PageSaving += (o, p) => OnPageSaving(p);
            internalPagedList.PageSaved += (o, p) => OnPageSaved(p);
            internalPagedList.PageUnloading += (o, p) => OnPageUnloading(p);
            internalPagedList.PageUnloaded += (o, p) => OnPageUnloaded(p);
        }
		public virtual void Flush() => InternalExtendedList.Flush();

		public virtual void Dispose() => InternalExtendedList.Dispose();

        protected virtual void OnPageLoading(IMemoryPage<TItem> page) {
        }

        protected virtual void OnPageLoaded(IMemoryPage<TItem> page) {
        }

        protected virtual void OnPageSaving(IMemoryPage<TItem> page) {
        }

        protected virtual void OnPageSaved(IMemoryPage<TItem> page) {
        }

        protected virtual void OnPageUnloading(IMemoryPage<TItem> page) {
        }

        protected virtual void OnPageUnloaded(IMemoryPage<TItem> page) {
        }
	}
	public abstract class MemoryPagedListDecorator<TItem> : MemoryPagedListDecorator<TItem, IMemoryPagedList<TItem>> {

		protected MemoryPagedListDecorator(IMemoryPagedList<TItem> internalPagedList)
			: base(internalPagedList) {
		}
	}
}