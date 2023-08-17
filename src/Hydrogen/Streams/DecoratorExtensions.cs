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

	public static BoundedStream<TInner> AsBounded<TInner>(this TInner stream, long minPosition, long maxPosition) where TInner : Stream 
		=> new(stream, minPosition, maxPosition);

	public static BoundedStream AsBounded(this Stream stream, long minPosition, long maxPosition, bool allowInnerResize = true, bool useRelativeOffset = false) 
		=> new(stream, minPosition, maxPosition) { UseRelativeOffset = useRelativeOffset, AllowInnerResize = allowInnerResize };

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

	public static ConcurrentStream<TInner> AsConcurrent<TInner>(this TInner stream) where TInner : Stream 
		=> stream as ConcurrentStream<TInner> ?? new (stream);
}
