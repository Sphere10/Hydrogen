// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Hydrogen;

public class BackgroundOutputStream : Stream {
	private readonly Queue<byte[]> _writeQueue = new Queue<byte[]>();
	private readonly EventWaitHandle _idleEvent = new EventWaitHandle(true, EventResetMode.ManualReset);
	private bool _writing;
	private Exception _lastException;
	private bool _disposed;

	private readonly Stream _underlyingStream;
	private readonly Action _underlyingFlushAction;

	public BackgroundOutputStream(Stream underlyingStream, Action underlyingFlushAction) {
		_underlyingStream = underlyingStream;
		_underlyingFlushAction = underlyingFlushAction;
	}

	public override void Flush() {
		if (null != _lastException)
			throw _lastException;

		if (!_idleEvent.WaitOne(1000 * 3600 * 4)) // 4 hours
			throw _lastException
			      ?? new IOException("BackgroundOutputStream timed out.");

		if (null != _lastException)
			throw _lastException;
	}

	protected override void Dispose(bool disposing) {
		if (!_disposed) {
			if (disposing) {
				Flush();
			}
			_disposed = true;
		}
		base.Dispose(disposing);
	}

	public override void Write(byte[] buffer, int offset, int count) {
		if (null != _lastException)
			throw _lastException;

		if (count < 0)
			throw new ArgumentOutOfRangeException("count", count, "Count is negative.");
		if (0 == count)
			return;

		byte[] localBuffer = new byte[count];
		System.Array.Copy(buffer, offset, localBuffer, 0, count);

		lock (this) {
			_idleEvent.Reset();
			_writeQueue.Enqueue(localBuffer);
			if (!_writing) {
				_writing = true;
				ThreadPool.QueueUserWorkItem(BackgroundWrite);
			}
		}
	}

	private void BackgroundWrite(object nothing) {
		try {
			while (true) {
				byte[] localBuffer;
				lock (this) {
					if (_writeQueue.Count < 1) {
						_writing = false;
						_idleEvent.Set();
						return;
					}
					localBuffer = _writeQueue.Dequeue();
				}
				_underlyingStream.Write(localBuffer, 0, localBuffer.Length);
				_underlyingFlushAction.Invoke();
			}
		} catch (Exception ee) {
			_lastException = ee;
		}
	}

	public override bool CanWrite {
		get { return true; }
	}

	#region Unsupported operations

	public override bool CanRead {
		get { return false; }
	}

	public override long Seek(long offset, SeekOrigin origin) {
		throw new NotSupportedException();
	}

	public override bool CanSeek {
		get { return false; }
	}

	public override void SetLength(long value) {
		throw new NotSupportedException();
	}

	public override int Read(byte[] buffer, int offset, int count) {
		throw new NotSupportedException();
	}

	public override long Length {
		get { throw new NotSupportedException(); }
	}

	public override long Position {
		get { throw new NotSupportedException(); }
		set { throw new NotSupportedException(); }
	}

	#endregion

}
