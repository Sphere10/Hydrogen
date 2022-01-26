using System;

namespace Sphere10.Framework {
	public abstract class BufferDecorator<TBuffer> : ExtendedListDecorator<byte, TBuffer>, IBuffer where TBuffer : IBuffer {

		protected BufferDecorator(TBuffer internalBuffer)
            : base(internalBuffer) {
        }

        public virtual void AddRange(ReadOnlySpan<byte> span) => InternalExtendedList.AddRange(span);

        public virtual Span<byte> AsSpan(int index, int count) => InternalExtendedList.AsSpan(index, count);

        public virtual void InsertRange(int index, ReadOnlySpan<byte> items) => InternalExtendedList.InsertRange(index, items);

        public virtual ReadOnlySpan<byte> ReadSpan(int index, int count) => InternalExtendedList.ReadSpan(index, count);

        public virtual void UpdateRange(int index, ReadOnlySpan<byte> items) => InternalExtendedList.UpdateRange(index, items);
    }

}