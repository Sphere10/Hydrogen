// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen;

public abstract class StreamDecorator<TStream> : Stream where TStream : Stream {
	internal readonly TStream InnerStream;

	protected StreamDecorator(TStream stream) {
		InnerStream = stream;
	}

	public override void Flush() {
		InnerStream.Flush();
	}

	public override long Seek(long offset, SeekOrigin origin) {
		return InnerStream.Seek(offset, origin);
	}

	public override void SetLength(long value) {
		InnerStream.SetLength(value);
	}

	public override int Read(byte[] buffer, int offset, int count) {
		return InnerStream.Read(buffer, offset, count);
	}

	public override void Write(byte[] buffer, int offset, int count) {
		InnerStream.Write(buffer, offset, count);
	}

	public override bool CanRead => InnerStream.CanRead;

	public override bool CanSeek => InnerStream.CanSeek;

	public override bool CanWrite => InnerStream.CanWrite;

	public override long Length => InnerStream.Length;

	public override long Position {
		get => InnerStream.Position;
		set => InnerStream.Position = value;
	}

	protected override void Dispose(bool disposing) {
		InnerStream.Dispose();
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
		return InnerStream.BeginRead(buffer, offset, count, callback, state);
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
		return InnerStream.BeginWrite(buffer, offset, count, callback, state);
	}

	public override bool CanTimeout => InnerStream.CanTimeout;

	public override void Close() {
//		base.Close();   // removed because it calls Dispose() which is not what we want (may break 
		InnerStream.Close();
	}

	public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) {
		return InnerStream.CopyToAsync(destination, bufferSize, cancellationToken);
	}

	public override int EndRead(IAsyncResult asyncResult) {
		return InnerStream.EndRead(asyncResult);
	}

	public override void EndWrite(IAsyncResult asyncResult) {
		InnerStream.EndWrite(asyncResult);
	}

	public override bool Equals(object obj) {
		return InnerStream.Equals(obj);
	}

	public override Task FlushAsync(CancellationToken cancellationToken) {
		return InnerStream.FlushAsync(cancellationToken);
	}

	public override int GetHashCode() {
		return InnerStream.GetHashCode();
	}

	public override object InitializeLifetimeService() {
		return InnerStream.InitializeLifetimeService();
	}

	public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
		return InnerStream.ReadAsync(buffer, offset, count, cancellationToken);
	}

	public override int ReadByte() {
		return InnerStream.ReadByte();
	}

	public override int ReadTimeout {
		get => InnerStream.ReadTimeout;
		set => InnerStream.ReadTimeout = value;
	}

	public override string ToString() {
		return InnerStream.ToString();
	}

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
		return InnerStream.WriteAsync(buffer, offset, count, cancellationToken);
	}

	public override void WriteByte(byte value) {
		InnerStream.WriteByte(value);
	}

	public override int WriteTimeout {
		get => InnerStream.WriteTimeout;
		set => InnerStream.WriteTimeout = value;
	}

	[Obsolete]
	protected override WaitHandle CreateWaitHandle() {
		throw new NotSupportedException();
	}

	[Obsolete]
	protected override void ObjectInvariant() {
		throw new NotSupportedException();
	}
}


public abstract class StreamDecorator : StreamDecorator<Stream> {
	protected StreamDecorator(Stream stream)
		: base(stream) {
	}
}
