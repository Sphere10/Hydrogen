// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// A stream wrapper that ensures a lock is attained before any member is invoked. It also provides a mechanism to acquire a locking scope.
/// </summary>
public class ConcurrentStream : StreamDecorator, ICriticalObject {
	// NOTE: implementation below calls the InnerStream directly as opposed to calling base method for performance reasons.

	private readonly ICriticalObject _lock;

	public ConcurrentStream(Stream stream, ICriticalObject @lock = null) : base(stream) {
		_lock = @lock ?? new CriticalObject();
	}

	public ICriticalObject ParentCriticalObject { get => _lock.ParentCriticalObject; set => _lock.ParentCriticalObject = value; }

	public object Lock => _lock.Lock;

	public bool IsLocked => _lock.IsLocked;
	
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IDisposable EnterAccessScope() => _lock.EnterAccessScope();

	public override void Flush() {
		CheckLocked();
		InnerStream.Flush();
	}

	public override long Seek(long offset, SeekOrigin origin) {
		CheckLocked();
		return InnerStream.Seek(offset, origin);
	}

	public override void SetLength(long value) {
		CheckLocked();
		InnerStream.SetLength(value);
	}

	public override int Read(byte[] buffer, int offset, int count) {
		CheckLocked();
		return InnerStream.Read(buffer, offset, count);
	}

	public override void Write(byte[] buffer, int offset, int count) {
		CheckLocked();
		InnerStream.Write(buffer, offset, count);
	}

	public override bool CanRead {
		get {
			CheckLocked();
			return InnerStream.CanRead;
		}
	}

	public override bool CanSeek {
		get {
			CheckLocked();
			return InnerStream.CanSeek;
		}
	}

	public override bool CanWrite {
		get {
			CheckLocked();
			return InnerStream.CanWrite;
		}
	}

	public override long Length {
		get {
			CheckLocked();
			return InnerStream.Length;
		}
	}

	public override long Position {
		get {
			CheckLocked();
			return InnerStream.Position;
		}
		set {
			CheckLocked();
			InnerStream.Position = value;
		}
	}

	protected override void Dispose(bool disposing) {
		// No lock needed for disposal
		InnerStream.Dispose();
	}

	public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
		CheckLocked();
		return InnerStream.BeginRead(buffer, offset, count, callback, state);
	}

	public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
		CheckLocked();
		return InnerStream.BeginWrite(buffer, offset, count, callback, state);
	}

	public override bool CanTimeout {
		get {
			CheckLocked();
			return InnerStream.CanTimeout;
		}
	}

	public override void Close() {
		//CheckLocked();
		using var scope = IsLocked ? EnterAccessScope() : new NoOpScope();
		InnerStream.Close();
	}

	public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) {
		CheckLocked();
		return InnerStream.CopyToAsync(destination, bufferSize, cancellationToken);
	}

	public override int EndRead(IAsyncResult asyncResult) {
		CheckLocked();
		return InnerStream.EndRead(asyncResult);
	}

	public override void EndWrite(IAsyncResult asyncResult) {
		CheckLocked();
		InnerStream.EndWrite(asyncResult);
	}

	public override bool Equals(object obj) {
		CheckLocked();
		return InnerStream.Equals(obj);
	}

	public override Task FlushAsync(CancellationToken cancellationToken) {
		CheckLocked();
		return InnerStream.FlushAsync(cancellationToken);
	}

	public override int GetHashCode() {
		CheckLocked();
		return InnerStream.GetHashCode();
	}

	public override object InitializeLifetimeService() {
		CheckLocked();
		return InnerStream.InitializeLifetimeService();
	}

	public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
		CheckLocked();
		return InnerStream.ReadAsync(buffer, offset, count, cancellationToken);
	}

	public override int ReadByte() {
		CheckLocked();
		return InnerStream.ReadByte();
	}

	public override int ReadTimeout {
		get {
			CheckLocked();
			return InnerStream.ReadTimeout;
		}
		set {
			CheckLocked();
			InnerStream.ReadTimeout = value;
		}
	}

	public override string ToString() {
		CheckLocked();
		return InnerStream.ToString();
	}

	public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
		CheckLocked();
		return InnerStream.WriteAsync(buffer, offset, count, cancellationToken);
	}

	public override void WriteByte(byte value) {
		CheckLocked();
		InnerStream.WriteByte(value);
	}

	public override int WriteTimeout {
		get {
			CheckLocked();
			return InnerStream.WriteTimeout;
		}
		set {
			CheckLocked();
			InnerStream.WriteTimeout = value;
		}
	}

	[Obsolete]
	protected override WaitHandle CreateWaitHandle() {
		throw new NotSupportedException();
	}

	[Obsolete]
	protected override void ObjectInvariant() {
		throw new NotSupportedException();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CheckLocked() {
		if (!IsLocked)
			throw new InvalidOperationException("Stream cannot be accessed without acquiring a locking scope");
	}
}