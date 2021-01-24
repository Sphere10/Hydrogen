using System;
using System.Collections.Generic;

namespace Sphere10.Framework {
    public class BufferDecorator : ExtendedListDecorator<byte>, IBuffer {

        public BufferDecorator(IBuffer internalBuffer)
            : base(internalBuffer) {
        }

        protected new IBuffer InternalExtendedList => (IBuffer)base.InternalExtendedList;

        public virtual void AddRange(ReadOnlySpan<byte> span) => InternalExtendedList.AddRange(span);

        public virtual Span<byte> AsSpan(int index, int count) => InternalExtendedList.AsSpan(index, count);

        public virtual void InsertRange(int index, ReadOnlySpan<byte> items) => InternalExtendedList.InsertRange(index, items);

        public virtual ReadOnlySpan<byte> ReadSpan(int index, int count) => InternalExtendedList.ReadSpan(index, count);

        public virtual void UpdateRange(int index, ReadOnlySpan<byte> items) => InternalExtendedList.UpdateRange(index, items);
    }

}