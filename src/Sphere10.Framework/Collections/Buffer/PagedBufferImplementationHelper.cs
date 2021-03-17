using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework {

    // PagedBuffer span operations implemented in a not-so-great way. Relying on page operations to do the bulk insertions/
    internal static class PagedBufferImplementationHelper {

        public static ReadOnlySpan<byte> ReadSpan(IMemoryPagedBuffer buffer, PagedListBase<byte> bufferAsList, int index, int count)
        {
			ByteArrayBuilder builder = new ByteArrayBuilder();

            bufferAsList.CheckRequiresLoad();
            bufferAsList.NotifyAccessing();
            bufferAsList.CheckRange(index, count);

            foreach (var pageSegment in bufferAsList.GetPageSegments(index, count))
            {
                var page = pageSegment.Item1 as IBufferPage ??
                    throw new InvalidOperationException("ReadSpan not supported by page type");
                
                var pageStartIndex = pageSegment.Item2;
                var pageItemCount = pageSegment.Item3;
                bufferAsList.NotifyPageAccessing(page);
                using (bufferAsList.EnterOpenPageScope(page)) {
                    bufferAsList.NotifyPageReading(page);
                    builder.Append(page.ReadSpan(pageStartIndex, pageItemCount));
                    bufferAsList.NotifyPageRead(page);
                }
                bufferAsList.NotifyPageAccessed(page);
            }
            bufferAsList.NotifyAccessed();

            return builder.ToArray();
        }

        public static void AddRange(IMemoryPagedBuffer buffer, ReadOnlySpan<byte> span) {
            // TODO: add optimized implementation
            buffer.AddRange((IEnumerable<byte>)span.ToArray());
        }

        public static void UpdateRange(IMemoryPagedBuffer buffer, int index, ReadOnlySpan<byte> items) {
            // TODO: add optimized implementation
            buffer.UpdateRange(index, (IEnumerable<byte>) items.ToArray());
        }

        public static void InsertRange(IMemoryPagedBuffer buffer, int index, ReadOnlySpan<byte> items) {
            // TODO: add optimized implementation
            buffer.InsertRange(index, (IEnumerable<byte>)items.ToArray());
        }

        public static Span<byte> AsSpan(IMemoryPagedBuffer buffer, int index, int count) {
            throw new NotSupportedException();
        }

		public static ReadOnlySpan<byte> ReadPageSpan(IBufferPage bufferPage, MemoryBuffer memoryStore, int index, int count) {
			var bufferAsMemoryPage = bufferPage as MemoryPageBase<byte>;
			bufferAsMemoryPage.CheckPageState(PageState.Loaded);
			bufferAsMemoryPage.CheckRange(index, count);
			return memoryStore.ReadSpan(index - bufferAsMemoryPage.StartIndex, count);
		}


        public static bool WriteSpan(IBufferPage bufferPage, int index, ReadOnlySpan<byte> items, out ReadOnlySpan<byte> overflow) {
			overflow = null;
			return false;
			/*   Guard.ArgumentNotNull(items, nameof(items));
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
				return totalWriteCount == itemsArr.Length;*/
		}
    }

}