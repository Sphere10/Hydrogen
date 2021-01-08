using System.Collections.Generic;

namespace Sphere10.Framework {
    public class PagedListDecorator<TItem, TPage> : ExtendedListDecorator<TItem>, IPagedList<TItem, TPage>
        where TPage : IPage<TItem> {

        public event EventHandlerEx<object> Accessing { add => InternalExtendedList.Accessing += value; remove => InternalExtendedList.Accessing -= value; }
        public event EventHandlerEx<object> Accessed { add => InternalExtendedList.Accessed += value; remove => InternalExtendedList.Accessed -= value; }
		public event EventHandlerEx<object> Loading { add => InternalExtendedList.Loading += value; remove => InternalExtendedList.Loading -= value; }
        public event EventHandlerEx<object> Loaded { add => InternalExtendedList.Loaded += value; remove => InternalExtendedList.Loaded -= value; }
        public event EventHandlerEx<object, TPage> PageAccessing { add => InternalExtendedList.PageAccessing += value; remove => InternalExtendedList.PageAccessing -= value; }
        public event EventHandlerEx<object, TPage> PageAccessed { add => InternalExtendedList.PageAccessed += value; remove => InternalExtendedList.PageAccessed -= value; }
        public event EventHandlerEx<object, int> PageCreating { add => InternalExtendedList.PageCreating += value; remove => InternalExtendedList.PageCreating -= value; }
        public event EventHandlerEx<object, TPage> PageCreated { add => InternalExtendedList.PageCreated += value; remove => InternalExtendedList.PageCreated -= value; }
        public event EventHandlerEx<object, TPage> PageReading { add => InternalExtendedList.PageReading += value; remove => InternalExtendedList.PageReading -= value; }
        public event EventHandlerEx<object, TPage> PageRead { add => InternalExtendedList.PageRead += value; remove => InternalExtendedList.PageRead -= value; }
        public event EventHandlerEx<object, TPage> PageWriting { add => InternalExtendedList.PageWriting += value; remove => InternalExtendedList.PageWriting -= value; }
        public event EventHandlerEx<object, TPage> PageWrite { add => InternalExtendedList.PageWrite += value; remove => InternalExtendedList.PageWrite -= value; }
        public event EventHandlerEx<object, TPage> PageDeleting { add => InternalExtendedList.PageDeleting += value; remove => InternalExtendedList.PageDeleting -= value; }
        public event EventHandlerEx<object, TPage> PageDeleted { add => InternalExtendedList.PageDeleted += value; remove => InternalExtendedList.PageDeleted -= value; }

        protected PagedListDecorator(IPagedList<TItem, TPage> internalPagedList)
            : base(internalPagedList) {
        }

        protected new IPagedList<TItem, TPage> InternalExtendedList => (IPagedList<TItem, TPage>)base.InternalExtendedList;

        public IReadOnlyList<TPage> Pages => InternalExtendedList.Pages;

        public bool RequiresLoad => InternalExtendedList.RequiresLoad;

        public void Load() => InternalExtendedList.Load();
    }
}