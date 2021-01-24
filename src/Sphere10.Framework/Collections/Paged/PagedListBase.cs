using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Sphere10.Framework {

    public abstract class PagedListBase<TItem> : RangedListBase<TItem>, IPagedList<TItem> {

		public event EventHandlerEx<object> Accessing;
		public event EventHandlerEx<object> Accessed;
		public event EventHandlerEx<object> Loading;
		public event EventHandlerEx<object> Loaded;
		public event EventHandlerEx<object, IPage<TItem>> PageAccessing;
		public event EventHandlerEx<object, IPage<TItem>> PageAccessed;
		public event EventHandlerEx<object, int> PageCreating;
		public event EventHandlerEx<object, IPage<TItem>> PageCreated;
		public event EventHandlerEx<object, IPage<TItem>> PageReading;
		public event EventHandlerEx<object, IPage<TItem>> PageRead;
		public event EventHandlerEx<object, IPage<TItem>> PageWriting;
		public event EventHandlerEx<object, IPage<TItem>> PageWrite;
		public event EventHandlerEx<object, IPage<TItem>> PageDeleting;
		public event EventHandlerEx<object, IPage<TItem>> PageDeleted;

		protected IExtendedList<IPage<TItem>> InternalPages;

		protected bool SuppressNotifications;

		protected PagedListBase() {
			SuppressNotifications = false;
			RequiresLoad = false;
			IsLoading = false;
			InternalPages = new ExtendedList<IPage<TItem>>();
		}

		public override int Count => InternalPages.Sum(p => p.Count);

		public virtual IReadOnlyList<IPage<TItem>> Pages => InternalPages;

		public bool RequiresLoad { get; protected set; }

		protected bool IsLoading { get; private set; }

		public void Load() {
			NotifyLoading();
			RequiresLoad = false;
			IsLoading = true;
			try {
				Clear();
				foreach (var page in LoadPages())
					InternalPages.Add(page);
			} finally {
				IsLoading = false;
			}
			NotifyLoaded();
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

			var page = InternalPages.Any() ? InternalPages.Last() : CreateNextPage();

			bool AppendToPage(out IEnumerable<TItem> remaining) {
				NotifyPageAccessing(page);
				bool fittedCompletely;
				using (EnterOpenPageScope(page)) {
					NotifyPageWriting(page);
					UpdateVersion();
					fittedCompletely = page.Write(page.EndIndex + 1, items, out remaining);
					NotifyPageWrite(page);
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
					NotifyPageWriting(page);
					UpdateVersion();
					if (!page.Write(pageStartIX, pageItems, out var overflow)) {
						throw new NotSupportedException("Overflow when updating is not supported");
					}
					NotifyPageWrite(page);
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
			if (endIndex != InternalPages.Last().EndIndex)
				throw new NotSupportedException("Removing an inner region of items is not supported. This collection only supports removing from the end.");

			foreach (var page in GetPagesInRange(index, endIndex).ToArray().Reverse()) {
				var toRemoveCount = count > page.Count ? page.Count : count;
				if (toRemoveCount < page.Count) {
					NotifyPageAccessing(page);
					NotifyPageWriting(page);
					UpdateVersion();
					using (EnterOpenPageScope(page)) {
						page.EraseFromEnd(toRemoveCount);
						NotifyPageWrite(page);
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
			while (InternalPages.Count > 0)
				DeletePage(InternalPages[InternalPages.Count - 1]);
			InternalPages.Clear();
			NotifyAccessed();
		}

		public override IEnumerator<TItem> GetEnumerator() {
			var currentVersion = Version;
			return
				InternalPages
					.SelectMany(p => {
						using (EnterOpenPageScope(p)) {
							return p;
						}
					})
					.GetEnumerator()
					.OnMoveNext(() => CheckVersion(currentVersion));
		}

		protected abstract IPage<TItem> NewPageInstance(int pageNumber);

		protected abstract IPage<TItem>[] LoadPages();

		protected IPage<TItem> CreateNextPage() {
			IPage<TItem> newPage;
			if (!InternalPages.Any()) {
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
				var lastPage = InternalPages.Last();
				var nextPageNumber = lastPage.Number + 1;
				NotifyPageCreating(nextPageNumber);
				newPage = NewPageInstance(nextPageNumber);
				newPage.Number = nextPageNumber;
				newPage.StartIndex = lastPage.EndIndex + 1;
				newPage.EndIndex = lastPage.EndIndex;
				newPage.Count = 0;
				newPage.Size = 0;
			};
			InternalPages.Add(newPage);
			NotifyPageCreated(newPage);
			return newPage;
		}

		protected void DeletePage(IPage<TItem> page) {
			if (page.Number != Pages.Last().Number)
				throw new NotSupportedException("Deleting inner pages is not currently supported in this collection. Only deleting pages from the end is supported.");
			NotifyPageAccessing(page);
			page.State = PageState.Deleting;
			NotifyPageDeleting(page);
			InternalPages.RemoveAt(InternalPages.Count - 1);
			page.State = PageState.Deleted;
			NotifyPageDeleted(page);
			NotifyPageAccessed(page);
		}

		protected IEnumerable<Tuple<IPage<TItem>, int, int>> GetPageSegments(int startIndex, int count) {
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

		protected virtual IEnumerable<IPage<TItem>> GetPagesInRange(int startIndex, int endIndex) {
			var index = InternalPages.BinarySearch(startIndex, (x, p) => {
				if (startIndex < p.StartIndex)
					return -1;
				if (startIndex > p.EndIndex)
					return +1;
				return 0;
			});
			IPage<TItem> page;
			do {
				page = InternalPages[index++];
				yield return page;
			} while (endIndex > page.EndIndex && index < InternalPages.Count);

		}

		public abstract IDisposable EnterOpenPageScope(IPage<TItem> page);

		protected void CheckRequiresLoad() {
			if (RequiresLoad)
				throw new InvalidOperationException("File exists but has not been loaded");
		}

		protected void CheckRange(int index, int count) {
			Guard.Argument(InternalPages.Count > 0, nameof(index), "No pages");
			Guard.Argument(count >= 0, nameof(index), "Must be greater than or equal to 0");
			var startIX = InternalPages.First().StartIndex;
			var lastIX = InternalPages.Last().EndIndex;
			Guard.ArgumentInRange(index, startIX, lastIX, nameof(index));
			if (count > 0)
				Guard.ArgumentInRange(index + count - 1, startIX, lastIX, nameof(count));
		}

		#region Events 

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

		protected void NotifyLoading() {
			if (SuppressNotifications)
				return;

			OnLoading();
			Loading?.Invoke(this);
		}

		protected void NotifyLoaded() {
			if (SuppressNotifications)
				return;

			OnLoaded();
			Loaded?.Invoke(this);
		}

		protected void NotifyPageAccessing(IPage<TItem> page) {
			if (SuppressNotifications)
				return;

			OnPageAccessing(page);
			PageAccessing?.Invoke(this, page);
		}

		protected void NotifyPageAccessed(IPage<TItem> page) {
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

		private void NotifyPageCreated(IPage<TItem> page) {
			if (SuppressNotifications)
				return;

			OnPageCreated(page);
			PageCreated?.Invoke(this, page);
		}

		private void NotifyPageWriting(IPage<TItem> page) {
			if (SuppressNotifications)
				return;

			OnPageWriting(page);
			PageWriting?.Invoke(this, page);
		}

		private void NotifyPageWrite(IPage<TItem> page) {
			if (SuppressNotifications)
				return;

			OnPageWrite(page);
			PageWrite?.Invoke(this, page);
		}

		private void NotifyPageReading(IPage<TItem> page) {
			if (SuppressNotifications)
				return;

			OnPageReading(page);
			PageReading?.Invoke(this, page);
		}

		private void NotifyPageRead(IPage<TItem> page) {
			if (SuppressNotifications)
				return;

			OnPageRead(page);
			PageRead?.Invoke(this, page);
		}

		private void NotifyPageDeleting(IPage<TItem> page) {
			if (SuppressNotifications)
				return;

			OnPageDeleting(page);
			PageDeleting?.Invoke(this, page);
		}

		private void NotifyPageDeleted(IPage<TItem> page) {
			if (SuppressNotifications)
				return;

			OnPageDeleted(page);
			PageDeleted?.Invoke(this, page);
		}

		#endregion

	}

}
