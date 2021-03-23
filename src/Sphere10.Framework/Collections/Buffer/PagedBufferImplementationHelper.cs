using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;

namespace Sphere10.Framework
{

    // PagedBuffer span operations implemented in a not-so-great way. Relying on page operations to do the bulk insertions/
    internal static class PagedBufferImplementationHelper
    {
        public static ReadOnlySpan<byte> ReadSpan(IPagedListInternalMethods<byte> internalMethods, int index, int count)
        {
            ByteArrayBuilder builder = new ByteArrayBuilder();

            internalMethods.CheckRequiresLoad();
            internalMethods.NotifyAccessing();
            internalMethods.CheckRange(index, count);

            foreach (var pageSegment in internalMethods.GetPageSegments(index, count))
            {
                var page = pageSegment.Item1 as IBufferPage ??
                    throw new InvalidOperationException("ReadSpan not supported by page type");

                var pageStartIndex = pageSegment.Item2;
                var pageItemCount = pageSegment.Item3;
                internalMethods.NotifyPageAccessing(page);
                using (internalMethods.EnterOpenPageScope(page))
                {
                    internalMethods.NotifyPageReading(page);
                    builder.Append(page.ReadSpan(pageStartIndex, pageItemCount));
                    internalMethods.NotifyPageRead(page);
                }
                internalMethods.NotifyPageAccessed(page);
            }
            internalMethods.NotifyAccessed();

            return builder.ToArray();
        }

        public static void AddRange(IPagedListInternalMethods<byte> internalMethods, ReadOnlySpan<byte> span)
        {
            if (span.IsEmpty)
                return;

            internalMethods.CheckRequiresLoad();
            internalMethods.NotifyAccessing();

            var page = internalMethods.InternalPages().Any()
                ? internalMethods.InternalPages().Last()
                : internalMethods.CreateNextPage();
            IBufferPage bufferPage = Guard.ArgumentCast<IBufferPage>(page, nameof(page));

            internalMethods.NotifyPageAccessing(page);

            bool fittedCompletely = false;

            while (!fittedCompletely)
            {
                using (internalMethods.EnterOpenPageScope(page))
                {
                    internalMethods.NotifyPageWriting(page);
                    internalMethods.UpdateVersion();
                    fittedCompletely = bufferPage.AppendSpan( span, out span);
                    internalMethods.NotifyPageWrite(page);
                }

                internalMethods.NotifyPageAccessed(page);

                if (!fittedCompletely)
                {
                    page = internalMethods.CreateNextPage() as IBufferPage;
                    bufferPage = Guard.ArgumentCast<IBufferPage>(page, nameof(page));
                }
            }

            internalMethods.NotifyAccessed();
        }

        public static void UpdateRange(IPagedListInternalMethods<byte> internalMethods, int index,
            ReadOnlySpan<byte> items)
        {
            internalMethods.CheckRequiresLoad();
            internalMethods.NotifyAccessing();

            if (items.Length == 0)
                return;

            foreach (var pageSegment in internalMethods.GetPageSegments(index, items.Length))
            {
                IPage<byte> page = pageSegment.Item1;
                IBufferPage bufferPage = Guard.ArgumentCast<IBufferPage>(page, nameof(pageSegment));

                var pageStartIx = pageSegment.Item2;
                var pageCount = pageSegment.Item3;
                ReadOnlySpan<byte> pageSpan = items.Slice(pageStartIx - index, pageCount);

                using (internalMethods.EnterOpenPageScope(page))
                {
                    internalMethods.NotifyPageAccessing(page);
                    internalMethods.NotifyPageWriting(page);
                    internalMethods.UpdateVersion();
                    
                    bufferPage.UpdateSpan(pageStartIx, pageSpan);
                    
                    internalMethods.NotifyPageWrite(page);
                }
                internalMethods.NotifyPageAccessed(page);
            }

            internalMethods.NotifyAccessed();
        }

        public static void InsertRange(IMemoryPagedBuffer buffer, int index, ReadOnlySpan<byte> items) {
            // TODO: add optimized implementation
            buffer.InsertRange(index, (IEnumerable<byte>) items.ToArray());
        }

        public static Span<byte> AsSpan(IMemoryPagedBuffer buffer, int index, int count)
        {
            throw new NotSupportedException();
        }

        public static ReadOnlySpan<byte> ReadPageSpan(MemoryPageBase<byte> page, MemoryBuffer memoryStore, int index, int count) {
            page.CheckPageState(PageState.Loaded);
            page.CheckRange(index, count);
            return memoryStore.ReadSpan(index - page.StartIndex, count);
        }

        public static bool AppendPageSpan(MemoryPageBase<byte> page, MemoryBuffer memoryStore, ReadOnlySpan<byte> items, out ReadOnlySpan<byte> overflow) {
            page.CheckPageState(PageState.Loaded);

            // Nothing to write case
            if (items.Length == 0)
            {
                overflow = ReadOnlySpan<byte>.Empty;
                return true;
            }

            int remainingPageBytes = page.MaxSize - page.Size;
            ReadOnlySpan<byte> writeSpan = items.Slice(0, Math.Min(remainingPageBytes, items.Length));

            memoryStore.Write(page.Size, writeSpan);

            page.Count += writeSpan.Length;
            page.EndIndex += writeSpan.Length;
            page.Size += writeSpan.Length;

            var totalWriteCount = writeSpan.Length;

            if (page.Count == 0 && totalWriteCount == 0)
            {
                // Was unable to write the first element in an empty page, item too large
                throw new InvalidOperationException($"Span cannot be fitted onto a page of this collection");
            }
            if (totalWriteCount > 0)
                page.Dirty = true;
            overflow = totalWriteCount < items.Length
                ? items.Slice(totalWriteCount).ToArray()
                : ReadOnlySpan<byte>.Empty;

            Debug.Assert(totalWriteCount <= items.Length);
            return totalWriteCount == items.Length;
        }

        public static void UpdatePageSpan(MemoryPageBase<byte> page, MemoryBuffer memoryStore, in int index, in ReadOnlySpan<byte> items)
        {
            page.CheckPageState(PageState.Loaded);
            Guard.ArgumentInRange(index, page.StartIndex, Math.Max(page.StartIndex, page.EndIndex) + 1, nameof(index));
            Guard.Ensure(index - page.StartIndex + items.Length - 1 <= page.MaxSize, "Update span would not fit on page.");

            // Nothing to write case
            if (items.Length == 0)
            {
                return;
            }

            int newBytesCount = Math.Max(index - page.StartIndex + items.Length - 1, page.Size) - page.Size;
            
            memoryStore.Write(index - page.StartIndex, items);
            
            page.Count += newBytesCount;
            page.EndIndex += newBytesCount;
            page.Size += newBytesCount;
            page.Dirty = true;
        }
    }

}