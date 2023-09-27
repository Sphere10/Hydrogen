using System;
using System.IO;

namespace Hydrogen;

public static class StreamDecoratorExtensions {

	public static OnDisposeStream OnDispose<TInner>(this TInner stream, Action action) where TInner : Stream 
		=> new(stream, action);
	
	public static OnDisposeStream OnDispose(this Stream stream, Action action) 
		=> new(stream, action);

	public static OnDisposeStream OnDispose(this Stream stream, Action<Stream> action) 
		=> new (stream, action);

	public static BoundedStream<TInner> AsBounded<TInner>(this TInner stream, long minPosition, long maxPosition, bool allowResize = true, bool useRelativeOffset = false) where TInner : Stream 
		=> new(stream, minPosition, maxPosition) { AllowInnerResize = allowResize, UseRelativeAddressing = useRelativeOffset };

	public static BoundedStream AsBounded(this Stream stream, long minPosition, long maxPosition, bool allowInnerResize = true, bool useRelativeOffset = false) 
		=> new(stream, minPosition, maxPosition) { AllowInnerResize = allowInnerResize, UseRelativeAddressing = useRelativeOffset };

	public static TransactionalStream<TInner> AsTransactional<TInner>(this TInner stream, ITransactionalObject transactionalObject) where TInner : Stream 
		=> new(stream, transactionalObject);

	public static NonClosingStream<Stream> AsNonClosing<TInner>(this TInner stream) where TInner : Stream 
		=> stream as NonClosingStream<Stream> ?? new(stream);

	public static NonClosingStream ToNonClosing(this Stream stream) 
		=> stream as NonClosingStream ?? new(stream);

	public static ReadOnlyStream<TInner> AsReadOnly<TInner>(this TInner stream) where TInner : Stream 
		=> stream as ReadOnlyStream<TInner> ?? new (stream);

	public static ReadOnlyStream AsReadOnly(this Stream stream)
		=> stream as ReadOnlyStream ?? new (stream);
	
	public static WriteOnlyStream<TInner> AsWriteOnly<TInner>(this TInner stream) where TInner : Stream 
		=> stream as WriteOnlyStream<TInner> ?? new (stream);

	public static WriteOnlyStream AsWriteOnly(this Stream stream) 
		=> stream as WriteOnlyStream ?? new (stream);


	public static ConcurrentStream AsConcurrent(this Stream stream) 
		=> stream as ConcurrentStream ?? new (stream);

	public static ConcurrentStream AsConcurrent(this Stream stream, ICriticalObject @lock) {
		return new ConcurrentStream(stream, @lock);
	}

	public static ConcurrentStream AsConcurrent(this Stream stream, object _lock) {
		if (stream is ConcurrentStream concurrentStream) {
			if (!ReferenceEquals(concurrentStream.Lock, _lock)) 
				throw new ArgumentException("Stream is already a concurrent stream with a different lock.", nameof(stream));
			return concurrentStream;
		}
		return new ConcurrentStream(stream);
	}
}
