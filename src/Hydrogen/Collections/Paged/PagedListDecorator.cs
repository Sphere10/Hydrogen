﻿using System;
using System.Collections.Generic;

namespace Hydrogen {
	public abstract class PagedListDecorator<TItem, TPagedList> : ExtendedListDecorator<TItem, TPagedList>, IPagedList<TItem> where TPagedList : IPagedList<TItem> {

		public event EventHandlerEx<object> Accessing { add => InternalCollection.Accessing += value; remove => InternalCollection.Accessing -= value; }
		public event EventHandlerEx<object> Accessed { add => InternalCollection.Accessed += value; remove => InternalCollection.Accessed -= value; }
		public event EventHandlerEx<object> Loading { add => InternalCollection.Loading += value; remove => InternalCollection.Loading -= value; }
		public event EventHandlerEx<object> Loaded { add => InternalCollection.Loaded += value; remove => InternalCollection.Loaded -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageAccessing { add => InternalCollection.PageAccessing += value; remove => InternalCollection.PageAccessing -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageAccessed { add => InternalCollection.PageAccessed += value; remove => InternalCollection.PageAccessed -= value; }
		public event EventHandlerEx<object, int> PageCreating { add => InternalCollection.PageCreating += value; remove => InternalCollection.PageCreating -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageCreated { add => InternalCollection.PageCreated += value; remove => InternalCollection.PageCreated -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageReading { add => InternalCollection.PageReading += value; remove => InternalCollection.PageReading -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageRead { add => InternalCollection.PageRead += value; remove => InternalCollection.PageRead -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageWriting { add => InternalCollection.PageWriting += value; remove => InternalCollection.PageWriting -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageWrite { add => InternalCollection.PageWrite += value; remove => InternalCollection.PageWrite -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageDeleting { add => InternalCollection.PageDeleting += value; remove => InternalCollection.PageDeleting -= value; }
		public event EventHandlerEx<object, IPage<TItem>> PageDeleted { add => InternalCollection.PageDeleted += value; remove => InternalCollection.PageDeleted -= value; }

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

		internal IReadOnlyList<IPage<TItem>> Pages => InternalCollection.Pages;

		IReadOnlyList<IPage<TItem>> IPagedList<TItem>.Pages => this.Pages;

		public bool RequiresLoad => InternalCollection.RequiresLoad;



		public void Load() => InternalCollection.Load();

		public IDisposable EnterOpenPageScope(IPage<TItem> page) => InternalCollection.EnterOpenPageScope(page);

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