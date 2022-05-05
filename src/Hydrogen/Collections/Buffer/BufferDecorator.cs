using System;

namespace Hydrogen {
	public abstract class BufferDecorator<TBuffer> : ExtendedListDecorator<byte, TBuffer>, IBuffer where TBuffer : IBuffer {

		protected BufferDecorator(TBuffer internalBuffer)
            : base(internalBuffer) {
        }

        public virtual void AddRange(ReadOnlySpan<byte> span) => InternalCollection.AddRange(span);

        public virtual Span<byte> AsSpan(int index, int count) => InternalCollection.AsSpan(index, count);

        public virtual void InsertRange(int index, ReadOnlySpan<byte> items) => InternalCollection.InsertRange(index, items);

        public virtual ReadOnlySpan<byte> ReadSpan(int index, int count) => InternalCollection.ReadSpan(index, count);

        public virtual void UpdateRange(int index, ReadOnlySpan<byte> items) => InternalCollection.UpdateRange(index, items);

        public virtual void ExpandTo(int totalBytes) => InternalCollection.ExpandTo(totalBytes);

        public virtual void ExpandBy(int newBytes) => InternalCollection.ExpandBy(newBytes);
	}

}