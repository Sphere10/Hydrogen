using System.Collections.Generic;

namespace Sphere10.Framework {
    public class MemoryPagedListDecorator<TItem, TPage> : PagedListDecorator<TItem, TPage>, IMemoryPagedList<TItem, TPage> where TPage : IPage<TItem> {
        public event EventHandlerEx<object, TPage> PageLoading { add => InternalExtendedList.PageLoading += value; remove => InternalExtendedList.PageLoading -= value; }
        public event EventHandlerEx<object, TPage> PageLoaded { add => InternalExtendedList.PageLoaded += value; remove => InternalExtendedList.PageLoaded -= value; }
        public event EventHandlerEx<object, TPage> PageSaving { add => InternalExtendedList.PageSaving += value; remove => InternalExtendedList.PageSaving -= value; }
        public event EventHandlerEx<object, TPage> PageSaved { add => InternalExtendedList.PageSaved += value; remove => InternalExtendedList.PageSaved -= value; }
        public event EventHandlerEx<object, TPage> PageUnloading { add => InternalExtendedList.PageUnloading += value; remove => InternalExtendedList.PageUnloading -= value; }
        public event EventHandlerEx<object, TPage> PageUnloaded { add => InternalExtendedList.PageUnloaded += value; remove => InternalExtendedList.PageUnloaded -= value; }

        protected MemoryPagedListDecorator(IMemoryPagedList<TItem, TPage> internalPagedList)
            : base(internalPagedList) {
            internalPagedList.PageLoading += (o, p) => OnPageLoading(p);
            internalPagedList.PageLoaded += (o, p) => OnPageLoaded(p);
            internalPagedList.PageSaving += (o, p) => OnPageSaving(p);
            internalPagedList.PageSaved += (o, p) => OnPageSaved(p);
            internalPagedList.PageUnloading += (o, p) => OnPageUnloading(p);
            internalPagedList.PageUnloaded += (o, p) => OnPageUnloaded(p);

        }
        public void Dispose() => InternalExtendedList.Dispose();

        protected new IMemoryPagedList<TItem, TPage> InternalExtendedList => (IMemoryPagedList<TItem, TPage>)base.InternalExtendedList;

        protected virtual void OnPageLoading(TPage page) {
        }

        protected virtual void OnPageLoaded(TPage page) {
        }

        protected virtual void OnPageSaving(TPage page) {
        }

        protected virtual void OnPageSaved(TPage page) {
        }

        protected virtual void OnPageUnloading(TPage page) {
        }

        protected virtual void OnPageUnloaded(TPage page) {
        }

    }
}