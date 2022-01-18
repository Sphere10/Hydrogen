using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Sphere10.Framework {

	/// <summary>
	/// A re-implementation of <see cref="PagedListBase{TItem}"/> methods optimized for spans and used by <see cref="IBuffer"/> implementations.
	/// </summary>
	/// <remarks>PagedBuffer span operations implemented in a not-so-great way. Relying on page operations to do the bulk insertions</remarks>
	internal static class PagedBufferImplementationHelper {
		public static ReadOnlySpan<byte> ReadRange(IPagedListDelegate<byte> pagedBuffer, int index, int count) {
			var builder = new ByteArrayBuilder();

			pagedBuffer.CheckRequiresLoad();
			pagedBuffer.NotifyAccessing();
			pagedBuffer.CheckRange(index, count, false);

			foreach (var pageSegment in pagedBuffer.GetPageSegments(index, count)) {
				var page = (IBufferPage)pageSegment.Item1;
				var pageStartIndex = pageSegment.Item2;
				var pageItemCount = pageSegment.Item3;
				pagedBuffer.NotifyPageAccessing(page);
				using (pagedBuffer.EnterOpenPageScope(page)) {
					pagedBuffer.NotifyPageReading(page);
					builder.Append(page.ReadSpan(pageStartIndex, pageItemCount));
					pagedBuffer.NotifyPageRead(page);
				}
				pagedBuffer.NotifyPageAccessed(page);
			}
			pagedBuffer.NotifyAccessed();

			return builder.ToArray();
		}

		public static void AddRange(IPagedListDelegate<byte> pagedBuffer, ReadOnlySpan<byte> span) {
			if (span.IsEmpty)
				return;
			pagedBuffer.CheckRequiresLoad();
			pagedBuffer.NotifyAccessing();
			var page = pagedBuffer.InternalPages().Any() ? pagedBuffer.InternalPages().Last() : pagedBuffer.CreateNextPage();
			var bufferPage = (IBufferPage)page;
			pagedBuffer.NotifyPageAccessing(page);
			var fittedCompletely = false;
			while (!fittedCompletely) {
				using (pagedBuffer.EnterOpenPageScope(page)) {
					pagedBuffer.NotifyPageWriting(page);
					pagedBuffer.UpdateVersion();
					fittedCompletely = bufferPage.AppendSpan(span, out span);
					pagedBuffer.NotifyPageWrite(page);
				}
				pagedBuffer.NotifyPageAccessed(page);
				if (!fittedCompletely) {
					page = pagedBuffer.CreateNextPage() as IBufferPage;
					bufferPage = (IBufferPage)page;
				}
			}
			pagedBuffer.NotifyAccessed();
		}

		public static void UpdateRange(IPagedListDelegate<byte> pagedBuffer, int index, ReadOnlySpan<byte> items) {
			pagedBuffer.CheckRange(index, items.Length, false);
			pagedBuffer.CheckRequiresLoad();
			pagedBuffer.NotifyAccessing();

			if (items.Length == 0)
				return;

			foreach (var pageSegment in pagedBuffer.GetPageSegments(index, items.Length)) {
				var page = pageSegment.Item1;
				var bufferPage = (IBufferPage)page;
				var pageStartIx = pageSegment.Item2;
				var pageCount = pageSegment.Item3;
				var pageSpan = items.Slice(pageStartIx - index, pageCount);
				using (pagedBuffer.EnterOpenPageScope(page)) {
					pagedBuffer.NotifyPageAccessing(page);
					pagedBuffer.NotifyPageWriting(page);
					pagedBuffer.UpdateVersion();
					bufferPage.UpdateSpan(pageStartIx, pageSpan);
					pagedBuffer.NotifyPageWrite(page);
				}
				pagedBuffer.NotifyPageAccessed(page);
			}
			pagedBuffer.NotifyAccessed();
		}

		public static void InsertRange(IPagedListDelegate<byte> pagedBuffer, in int count, in int index, in ReadOnlySpan<byte> items) {
			if (index == count)
				AddRange(pagedBuffer, items);
			else throw new NotSupportedException("This collection can only be appended from the end");
		}

		public static Span<byte> AsSpan(IPagedListDelegate<byte> internalMethods, int index, int count) {
			// TODO: this needs to check if the span range covers a contiguous region of a page, and if not throw
			// returns span for the page (needs to provide a locking scope to ensure the page isn't swapped)
			var readOnlySpan = ReadRange(internalMethods, index, count);

			// https://github.com/dotnet/runtime/issues/23494#issuecomment-648290373
			return MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(readOnlySpan), readOnlySpan.Length);
		}

		public static ReadOnlySpan<byte> ReadPageSpan(MemoryPageBase<byte> page, MemoryBuffer memoryStore, int index, int count) {
			page.CheckPageState(PageState.Loaded);
			page.CheckRange(index, count);
			return memoryStore.ReadSpan(index - page.StartIndex, count);
		}

		public static bool AppendPageSpan(MemoryPageBase<byte> page, MemoryBuffer memoryStore, ReadOnlySpan<byte> items, out ReadOnlySpan<byte> overflow) {
			page.CheckPageState(PageState.Loaded);

			// Nothing to write case
			if (items.Length == 0) {
				overflow = ReadOnlySpan<byte>.Empty;
				return true;
			}

			var remainingPageBytes = page.MaxSize - page.Size;
			var writeSpan = items.Slice(0, Math.Min(remainingPageBytes, items.Length));

			memoryStore.Write(page.Size, writeSpan);

			page.Count += writeSpan.Length;
			page.EndIndex += writeSpan.Length;
			page.Size += writeSpan.Length;

			var totalWriteCount = writeSpan.Length;

			if (page.Count == 0 && totalWriteCount == 0) // Was unable to write the first element in an empty page, item too large
				throw new InvalidOperationException("Span cannot be fitted onto a page of this collection");
			if (totalWriteCount > 0)
				page.Dirty = true;
			overflow = totalWriteCount < items.Length ? items.Slice(totalWriteCount) : ReadOnlySpan<byte>.Empty;

			Debug.Assert(totalWriteCount <= items.Length);
			return totalWriteCount == items.Length;
		}

		public static void UpdatePageSpan(MemoryPageBase<byte> page, MemoryBuffer memoryStore, in int index, in ReadOnlySpan<byte> items) {
			page.CheckPageState(PageState.Loaded);
			Guard.ArgumentInRange(index, page.StartIndex, Math.Max(page.StartIndex, page.EndIndex) + 1, nameof(index));
			Guard.Ensure(index - page.StartIndex + items.Length - 1 <= page.MaxSize, "Update span would not fit on page.");

			// Nothing to write case
			if (items.Length == 0)
				return;

			var newBytesCount = Math.Max(index - page.StartIndex + items.Length - 1, page.Size) - page.Size;

			memoryStore.Write(index - page.StartIndex, items);

			page.Count += newBytesCount;
			page.EndIndex += newBytesCount;
			page.Size += newBytesCount;
			page.Dirty = true;
		}
	}

}
