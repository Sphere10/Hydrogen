using System;
using System.Collections.Generic;

namespace Sphere10.Framework {
	public abstract class PagedListDecorator<TItem, TPagedList> : ExtendedListDecorator<TItem, TPagedList>, IPagedList<TItem> where TPagedList : IPagedList<TItem> {

		public event EventHandlerEx<object> Accessing { add => InternalExtendedList.Accessing += value; remove => InternalExtendedList.Accessing -= value; }
		public event EventHandlerEx<object> Accessed { add => InternalExtendedList.Accessed += value; remove => InternalExtendedList.Accessed -= value; }
		public event EventHandlerEx<object> Loading { add => InternalExtendedList.Loading += value; remove => InternalExtendedList.Loading -= value; }
		public event EventHandlerEx<object> Loaded { add => InternalExtendedList.Loaded += value; remove => InternalExtendedList.Loaded -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageAccessing { add => InternalExtendedList.PageAccessing += value; remove => InternalExtendedList.PageAccessing -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageAccessed { add => InternalExtendedList.PageAccessed += value; remove => InternalExtendedList.PageAccessed -= value; }
		public event EventHandlerEx<object, int> PageCreating { add => InternalExtendedList.PageCreating += value; remove => InternalExtendedList.PageCreating -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageCreated { add => InternalExtendedList.PageCreated += value; remove => InternalExtendedList.PageCreated -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageReading { add => InternalExtendedList.PageReading += value; remove => InternalExtendedList.PageReading -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageRead { add => InternalExtendedList.PageRead += value; remove => InternalExtendedList.PageRead -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageWriting { add => InternalExtendedList.PageWriting += value; remove => InternalExtendedList.PageWriting -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageWrite { add => InternalExtendedList.PageWrite += value; remove => InternalExtendedList.PageWrite -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageDeleting { add => InternalExtendedList.PageDeleting += value; remove => InternalExtendedList.PageDeleting -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageDeleted { add => InternalExtendedList.PageDeleted += value; remove => InternalExtendedList.PageDeleted -= value; }

		protected PagedListDecorator(TPagedList internalPagedList)
			: base(internalPagedList) {
			internalPagedList.Accessing += (o) => OnAccessing();
			internalPagedList.Accessed += (o) => OnAccessed();
			internalPagedList.Loading += (o) => OnLoading();
			internalPagedList.Loaded += (o) => OnLoaded();
			internalPagedList.PageAccessing += (o, p) => OnPageAccessing(p);
			internalPagedList.PageAccessed += (o, p) => OnPageAccessed(p);
			internalPagedList.PageCreating += (o, p) => OnPageCreating(p);
			internalPagedList.PageCreated += (o, p) => OnPageCreated(p);
			internalPagedList.PageReading += (o, p) => OnPageReading(p);
			internalPagedList.PageRead += (o, p) => OnPageRead(p);
			internalPagedList.PageWriting += (o, p) => OnPageWriting(p);
			internalPagedList.PageWrite += (o, p) => OnPageWrite(p);
			internalPagedList.PageDeleting += (o, p) => OnPageDeleting(p);
			internalPagedList.PageDeleted += (o, p) => OnPageDeleted(p);
		}

		public IReadOnlyList<IPage<TItem>> Pages => InternalExtendedList.Pages;

		public bool RequiresLoad => InternalExtendedList.RequiresLoad;

		public void Load() => InternalExtendedList.Load();

		public IDisposable EnterOpenPageScope(IPage<TItem> page) => InternalExtendedList.EnterOpenPageScope(page);

		protected virtual void OnAccessing() {
		}

		protected virtual void OnAccessed() {
		}

		protected virtual void OnLoading() {
		}

		protected virtual void OnLoaded() {
		}

		protected virtual void OnPageAccessing(IPage<TItem> page) {
		}

		protected virtual void OnPageAccessed(IPage<TItem> page) {
		}

		protected virtual void OnPageCreating(int pageNumber) {
		}

		protected virtual void OnPageCreated(IPage<TItem> page) {
		}

		protected virtual void OnPageReading(IPage<TItem> page) {
		}

		protected virtual void OnPageRead(IPage<TItem> page) {
		}

		protected virtual void OnPageWriting(IPage<TItem> page) {
		}

		protected virtual void OnPageWrite(IPage<TItem> page) {
		}

		protected virtual void OnPageDeleting(IPage<TItem> page) {
		}

		protected virtual void OnPageDeleted(IPage<TItem> page) {
		}
	}

	public abstract class PagedListDecorator<TItem> : PagedListDecorator<TItem, IPagedList<TItem>> {
		protected PagedListDecorator(IPagedList<TItem> internalPagedList)
			: base(internalPagedList) {
		}
	}
}