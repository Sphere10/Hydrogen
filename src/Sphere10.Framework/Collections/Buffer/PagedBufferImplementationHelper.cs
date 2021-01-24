using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sphere10.Framework {

    // PagedBuffer span operations implemented in a not-so-great way. Relying on page operations to do the bulk insertions/
    internal static class PagedBufferImplementationHelper {

        public static ReadOnlySpan<byte> ReadSpan(IMemoryPagedBuffer buffer, int index, int count) {
            // TODO: add optimized implementation
            return buffer.ReadRange(index, count).ToArray();
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