using System;
using System.Collections.Generic;

namespace Sphere10.Framework {

    // PagedBuffer span operations implemented in a not-so-great way. Relying on page operations to do the bulk insertions/
    internal static class PagedBufferImplementationHelper {

        public static ReadOnlySpan<byte> ReadSpan(IMemoryPagedBuffer buffer, int index, int count)
        {
            Guard.ArgumentInRange(index, 0, buffer.Count - 1, nameof(index));
            Guard.ArgumentInRange(count, 0, buffer.Count - index, nameof(count));
           
            ByteArrayBuilder builder = new ByteArrayBuilder();

            for (int i = 0; i < buffer.Pages.Count; i++)
            {
                IBufferPage page = buffer.Pages[i];

                if (index <= page.EndIndex)
                {
                    int spanIndex = page.StartIndex;
                    int toRead = Math.Min(count, page.Count);
                    
                    if (builder.Length == 0)
                    {
                        spanIndex = index;
                        toRead = page.Count - (spanIndex - page.StartIndex);
                    }

                    ReadOnlySpan<byte> pageSpan = page.ReadSpan(spanIndex, toRead);
                    builder.Append(pageSpan);
                    count -= toRead;
                }

                if (count == 0)
                {
                    break;
                }
            }

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

    }

}