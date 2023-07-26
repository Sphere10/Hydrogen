using System;
using System.IO;

namespace Hydrogen;

public static class StreamDecoratorExtensions {

	public static OnDisposeStream OnDispose<TInner>(this TInner stream, Action action) where TInner : Stream => new(stream, action);
	
	public static OnDisposeStream OnDispose(this Stream stream, Action action) => new(stream, action);

	public static OnDisposeStream OnDispose(this Stream stream, Action<Stream> action) => new (stream, action);

	public static BoundedStream<TInner> ToBounded<TInner>(this TInner stream, long minPosition, long maxPosition) where TInner : Stream => new(stream, minPosition, maxPosition);

	public static BoundedStream ToBounded(this Stream stream, long minPosition, long maxPosition) => new(stream, minPosition, maxPosition);

	public static TransactionalStream<TInner> ToTransactional<TInner>(this TInner stream, ITransactionalObject transactionalObject) where TInner : Stream => new(stream, transactionalObject);

	public static NonClosingStream<Stream> ToNonClosing<TInner>(this TInner stream) where TInner : Stream => new(stream);

	public static NonClosingStream ToNonClosing(this Stream stream) => new(stream);


}
