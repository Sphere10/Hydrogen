using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sphere10.Framework {

	public abstract class PagedListBase<TItem, TPage> : RangedListBase<TItem>
		where TPage : PagedListBase<TItem, TPage>.PageBase {

		public event EventHandlerEx<object> Accessing;
		public event EventHandlerEx<object> Accessed;
		public event EventHandlerEx<object, TPage> PageAccessing;
		public event EventHandlerEx<object, TPage> PageAccessed;
		public event EventHandlerEx<object, int> PageCreating;
		public event EventHandlerEx<object, TPage> PageCreated;
		public event EventHandlerEx<object, TPage> PageReading;
		public event EventHandlerEx<object, TPage> PageRead;
		public event EventHandlerEx<object, TPage> PageUpdating;
		public event EventHandlerEx<object, TPage> PageUpdated;
		public event EventHandlerEx<object, TPage> PageDeleting;
		public event EventHandlerEx<object, TPage> PageDeleted;

		protected IExtendedList<TPage> _pages;
		protected bool SuppressNotifications;

		protected PagedListBase() {
			SuppressNotifications = false;
			RequiresLoad = false;
			Loading = false;
			_pages = new ExtendedList<TPage>();
		}

		public override int Count => _pages.Sum(p => p.Count);

		public IReadOnlyList<TPage> Pages => _pages;

		public bool RequiresLoad { get; protected set; }

		protected bool Loading { get; private set; }

		public virtual void Load() {
			// load existing file
			RequiresLoad = false;
			Clear();
			try {
				Loading = true;
				LoadPages().ForEach(_pages.Add);
			} finally {
				Loading = false;
			}
		}

		public override IEnumerable<bool> ContainsRange(IEnumerable<TItem> items) => throw new NotSupportedException();

		public override IEnumerable<int> IndexOfRange(IEnumerable<TItem> items) => throw new NotSupportedException();

		public override IEnumerable<TItem> ReadRange(int index, int count) {
			return ReadRangeByPage(index, count).SelectMany(x => x);
		}

		public virtual IEnumerable<IEnumerable<TItem>> ReadRangeByPage(int index, int count) {
			CheckRequiresLoad();
			NotifyAccessing();
			CheckRange(index, count);
			foreach (var pageSegment in GetPageSegments(index, count)) {
				var page = pageSegment.Item1;
				var pageStartIndex = pageSegment.Item2;
				var pageItemCount = pageSegment.Item3;
				NotifyPageAccessing(page);
				using (EnterOpenPageScope(page)) {
					NotifyPageReading(page);
					yield return page.Read(pageStartIndex, pageItemCount);
					NotifyPageRead(page);
				}
				NotifyPageAccessed(page);
			}
			NotifyAccessed();
		}

		public override void AddRange(IEnumerable<TItem> items) {
			if (!items.Any())
				return;
			CheckRequiresLoad();
			NotifyAccessing();

			var page = _pages.Any() ? _pages.Last() : CreateNextPage();

			bool AppendToPage(out IEnumerable<TItem> remaining) {
				NotifyPageAccessing(page);
				bool fittedCompletely;
				using (EnterOpenPageScope(page)) {
					NotifyPageUpdating(page);
					UpdateVersion();
					fittedCompletely = page.Write(page.EndIndex + 1, items, out remaining);
					NotifyPageUpdated(page);
				}
				NotifyPageAccessed(page);
				return fittedCompletely;
			}

			while (!AppendToPage(out var remaining)) {
				page = CreateNextPage();
				items = remaining;
			}
			NotifyAccessed();
		}

		public override void UpdateRange(int index, IEnumerable<TItem> items) {
			CheckRequiresLoad();
			NotifyAccessing();
			var newItems = items.ToExtendedList();
			var newItemsCount = newItems.Count;
			CheckRange(index, newItemsCount);

			if (newItemsCount == 0) return;
			var endIndex = index + newItemsCount - 1;
			foreach (var pageSegment in GetPageSegments(index, newItemsCount)) {
				var page = pageSegment.Item1;
				var pageStartIX = pageSegment.Item2;
				var pageCount = pageSegment.Item3;
				var pageItems = newItems.ReadRange(pageStartIX - index, pageCount).ToArray();
				using (EnterOpenPageScope(page)) {
					NotifyPageAccessing(page);
					NotifyPageUpdating(page);
					UpdateVersion();
					if (!page.Write(pageStartIX, pageItems, out var overflow)) {
						throw new NotSupportedException("Overflow when updating is not supported");
					}
					NotifyPageUpdated(page);
				}
				NotifyPageAccessed(page);
			}
			NotifyAccessed();
		}

		public override void InsertRange(int index, IEnumerable<TItem> items) {
			throw new NotSupportedException();
		}

		public override IEnumerable<bool> RemoveRange(IEnumerable<TItem> items) => throw new NotSupportedException();

		public override void RemoveRange(int index, int count) {
			CheckRequiresLoad();
			NotifyAccessing();
			CheckRange(index, count);
			if (count == 0)
				return;
			var endIndex = index + count - 1;
			if (endIndex != _pages.Last().EndIndex)
				throw new NotSupportedException("Removing an inner region of items is not supported. This collection only supports removing from the end.");

			foreach (var page in GetPagesInRange(index, endIndex).ToArray().Reverse()) {
				var toRemoveCount = count > page.Count ? page.Count : count;
				if (toRemoveCount < page.Count) {
					NotifyPageAccessing(page);
					NotifyPageUpdating(page);
					UpdateVersion();
					using (EnterOpenPageScope(page)) {
						page.EraseFromEnd(toRemoveCount);
						NotifyPageUpdated(page);
					}
					NotifyPageAccessed(page);
				} else {
					DeletePage(page);
				}
				count -= toRemoveCount;
			}
			NotifyAccessed();
		}

		public override void Clear() {
			CheckRequiresLoad();
			NotifyAccessing();
			while (_pages.Count > 0)
				DeletePage(_pages[_pages.Count - 1]);
			_pages.Clear();
			NotifyAccessed();
		}



		public override IEnumerator<TItem> GetEnumerator() {
			var currentVersion = Version;
			return
				_pages
					.SelectMany(p => {
						using (EnterOpenPageScope(p)) {
							return p;
						}
					})
					.GetEnumerator()
					.OnMoveNext(() => CheckVersion(currentVersion));
		}

		protected abstract TPage NewPageInstance(int pageNumber);

		protected virtual TPage[] LoadPages() {
			// used by certain super-classes (will override abstract in their base implementations)
			throw new NotSupportedException(); 
		}

		protected TPage CreateNextPage() {
			TPage newPage;
			if (!_pages.Any()) {
				// First page
				NotifyPageCreating(0);
				newPage = NewPageInstance(0);
				newPage.Number = 0;
				newPage.StartIndex = 0;
				newPage.EndIndex = -1;
				newPage.Count = 0;
				newPage.Size = 0;
			} else {
				// Next page
				var lastPage = _pages.Last();
				var nextPageNumber = lastPage.Number + 1;
				NotifyPageCreating(nextPageNumber);
				newPage = NewPageInstance(nextPageNumber);
				newPage.Number = nextPageNumber;
				newPage.StartIndex = lastPage.EndIndex + 1;
				newPage.EndIndex = lastPage.EndIndex;
				newPage.Count = 0;
				newPage.Size = 0;
			};
			_pages.Add(newPage);
			NotifyPageCreated(newPage);
			return newPage;
		}

		protected void DeletePage(TPage page) {
			if (page.Number != Pages.Last().Number)
				throw new NotSupportedException("Deleting inner pages is not currently supported in this collection. Only deleting pages from the end is supported.");
			NotifyPageAccessing(page);
			page.State = PageState.Deleting;
			NotifyPageDeleting(page);
			_pages.RemoveAt(_pages.Count - 1);
			page.State = PageState.Deleted;
			NotifyPageDeleted(page);
			NotifyPageAccessed(page);
		}

		protected abstract IDisposable EnterOpenPageScope(TPage page);

		protected IEnumerable<Tuple<TPage, int, int>> GetPageSegments(int startIndex, int count) {
			if (count == 0)
				yield break;

			foreach (var page in GetPagesInRange(startIndex, startIndex + count - 1)) {
				Debug.Assert(startIndex >= page.StartIndex);
				var pageItemsToRead = Math.Min(page.Count - (startIndex - page.StartIndex), count); // read only what's needed
				yield return Tuple.Create(page, startIndex, pageItemsToRead);
				startIndex += pageItemsToRead;
				count -= pageItemsToRead;
				if (count <= 0)
					yield break;
			}
		}

		protected virtual IEnumerable<TPage> GetPagesInRange(int startIndex, int endIndex) {
			var index = _pages.BinarySearch(startIndex, (x, p) => {
				if (startIndex < p.StartIndex)
					return -1;
				if (startIndex > p.EndIndex)
					return +1;
				return 0;
			});
			TPage page;
			do {
				page = _pages[index++];
				yield return page;
			} while (endIndex > page.EndIndex && index < _pages.Count);

		}

		protected void CheckRequiresLoad() {
			if (RequiresLoad)
				throw new InvalidOperationException("File exists but has not been loaded");
		}

		protected void CheckRange(int index, int count) {
			Guard.Argument(_pages.Count > 0, nameof(index), "No pages");
			Guard.Argument(count >= 0, nameof(index), "Must be greater than or equal to 0");
			var startIX = _pages.First().StartIndex;
			var lastIX = _pages.Last().EndIndex;
			Guard.ArgumentInRange(index, startIX, lastIX, nameof(index));
			if (count > 0)
				Guard.ArgumentInRange(index + count - 1, startIX, lastIX, nameof(count));
		}

		#region Events 

		protected virtual void OnAccessing() {
		}

		protected virtual void OnAccessed() {
		}

		protected virtual void OnPageAccessing(TPage page) {
		}

		protected virtual void OnPageAccessed(TPage page) {
		}

		protected virtual void OnPageCreating(int pageNumber) {
		}

		protected virtual void OnPageCreated(TPage page) {
		}

		protected virtual void OnPageReading(TPage page) {
		}

		protected virtual void OnPageRead(TPage page) {
		}

		protected virtual void OnPageUpdating(TPage page) {
		}

		protected virtual void OnPageUpdated(TPage page) {
		}

		protected virtual void OnPageDeleting(TPage page) {
		}

		protected virtual void OnPageDeleted(TPage page) {
		}

		protected void NotifyAccessing() {
			if (SuppressNotifications)
				return;

			OnAccessing();
			Accessing?.Invoke(this);
		}

		protected void NotifyAccessed() {
			if (SuppressNotifications)
				return;

			OnAccessed();
			Accessed?.Invoke(this);
		}

		protected void NotifyPageAccessing(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageAccessing(page);
			PageAccessing?.Invoke(this, page);
		}

		protected void NotifyPageAccessed(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageAccessed(page);
			PageAccessed?.Invoke(this, page);
		}

		private void NotifyPageCreating(int pageNumber) {
			if (SuppressNotifications)
				return;

			OnPageCreating(pageNumber);
			PageCreating?.Invoke(this, pageNumber);
		}

		private void NotifyPageCreated(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageCreated(page);
			PageCreated?.Invoke(this, page);
		}

		private void NotifyPageUpdating(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageUpdating(page);
			PageUpdating?.Invoke(this, page);
		}

		private void NotifyPageUpdated(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageUpdated(page);
			PageUpdated?.Invoke(this, page);
		}

		private void NotifyPageReading(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageReading(page);
			PageReading?.Invoke(this, page);
		}

		private void NotifyPageRead(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageRead(page);
			PageRead?.Invoke(this, page);
		}

		private void NotifyPageDeleting(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageDeleting(page);
			PageDeleting?.Invoke(this, page);
		}

		private void NotifyPageDeleted(TPage page) {
			if (SuppressNotifications)
				return;

			OnPageDeleted(page);
			PageDeleted?.Invoke(this, page);
		}

		#endregion


		public abstract class PageBase : IEnumerable<TItem> {

			protected PageBase() {
				State = PageState.Unloaded;
			}

			public virtual int Number { get; set; }
			public virtual int StartIndex { get; set; }
			public virtual int EndIndex { get; set; }
			public virtual int Count { get; set; }
			public virtual int Size { get; set; }
			public virtual bool Dirty { get; set; }
			public virtual PageState State { get; set; }

			public IEnumerable<TItem> Read(int index, int count) {
				CheckPageState(PageState.Loaded);
				CheckRange(index, count);
				return ReadInternal(index, count);
			}

			public bool Write(int index, IEnumerable<TItem> items, out IEnumerable<TItem> overflow) {
				Guard.ArgumentNotNull(items, nameof(items));
				var itemsArr = items as TItem[] ?? items.ToArray();
				CheckPageState(PageState.Loaded);
				Guard.ArgumentInRange(index, StartIndex, Math.Max(StartIndex, EndIndex) + 1, nameof(index));

				// Nothing to write case
				if (itemsArr.Length == 0) {
					overflow = Enumerable.Empty<TItem>();
					return true;
				}

				// Update segment
				var updateCount = Math.Min(StartIndex + Count - index, itemsArr.Length);
				if (updateCount > 0) {
					var updateItems = itemsArr.Take(updateCount).ToArray();
					UpdateInternal(index, updateItems, out var oldItemsSpace, out var newItemsSpace);
					if (oldItemsSpace != newItemsSpace)
						// TODO: support this scenario if ever needed, lots of complexity in ensuring updated page doesn't overflow max size from superclasses.
						// Can lead to cascading page updates. 
						// For constant sized objects (like byte arrays) this will never fail since the updated regions will always remain the same size.
						throw new NotSupportedException("Updated a page with different sized objects is not supported in this collection.");  
					Size = Size - oldItemsSpace + newItemsSpace;
				}

				// Append segment
				var appendItems = updateCount > 0 ? itemsArr.Skip(updateCount).ToArray() : itemsArr;
				var appendCount = AppendInternal(appendItems, out var appendedItemsSpace);
				Count += appendCount;
				EndIndex += appendCount;
				Size += appendedItemsSpace;
				
				var totalWriteCount = updateCount + appendCount;
				if (Count == 0 && totalWriteCount == 0) {
					// Was unable to write the first element in an empty page, item too large
					throw new InvalidOperationException($"Item '{itemsArr[0]?.ToString() ?? "(NULL)"}' cannot be fitted onto a page of this collection");
				}
				if (totalWriteCount > 0)
					Dirty = true;
				overflow = totalWriteCount < itemsArr.Length ? itemsArr.Skip(totalWriteCount).ToArray() : Enumerable.Empty<TItem>();
				Debug.Assert(totalWriteCount <= itemsArr.Length);
				return totalWriteCount == itemsArr.Length;
			}

			public void EraseFromEnd(int count) {
				Guard.ArgumentInRange(count, 0, Count, nameof(count));
				if (count <= 0)
					return;

				EraseFromEndInternal(count, out var oldItemsSpace);
				Size -= oldItemsSpace;
				Count -= count;
				EndIndex -= count;
				Dirty = true;
			}
			
			public abstract IEnumerator<TItem> GetEnumerator();

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}

			protected abstract IEnumerable<TItem> ReadInternal(int index, int count);

			protected abstract int AppendInternal(TItem[] items, out int newItemsSize);

			protected abstract void UpdateInternal(int index, TItem[] items, out int oldItemsSize, out int newItemsSize);

			protected abstract void EraseFromEndInternal(int count, out int oldItemsSize);

			protected void CheckRange(int index, int count) {
				var startIX = StartIndex;
				var lastIX = startIX + (Count - 1).ClipTo(startIX, int.MaxValue);
				Guard.ArgumentInRange(index, startIX, lastIX, nameof(index));
				if (count > 0)
					Guard.ArgumentInRange(index + count - 1, startIX, lastIX, nameof(count));
			}

			protected void CheckPageState(PageState status) {
				if (State != status)
					throw new InvalidOperationException($"Page not {status}");
			}

			protected void CheckPageState(params PageState[] statuses) {
				if (!State.IsIn(statuses))
					throw new InvalidOperationException($"Page not in states {statuses.ToDelimittedString(",")}");
			}

			protected void CheckDirty() {
				if (!Dirty)
					throw new InvalidOperationException($"Page was not dirty");
			}

			protected void CheckNotDirty() {
				if (Dirty)
					throw new InvalidOperationException($"Page was dirty");
			}
			
		}

	}
}
