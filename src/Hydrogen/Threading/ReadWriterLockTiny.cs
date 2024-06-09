// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

// Credit: https://github.com/i255/ReaderWriterLockTiny/blob/master/ReaderWriterLockTiny/ReaderWriterLockTiny.cs

using System.Threading;

namespace Hydrogen.Threading;

/// <summary>
/// Hierarchical locks are not supported!!!
/// </summary>
public struct ReaderWriterLockTiny {
	// if lock is above this value then somebody has a write lock
	const int _writerLock = 1000000;

	// lock state counter
	private int _lock;

	public void EnterReadLock() {
		var w = new SpinWait();
		var tmpLock = _lock;
		while (tmpLock >= _writerLock ||
		       tmpLock != Interlocked.CompareExchange(ref _lock, tmpLock + 1, tmpLock)) {
			w.SpinOnce();
			tmpLock = _lock;
		}
	}

	public void EnterWriteLock() {
		var w = new SpinWait();

		while (0 != Interlocked.CompareExchange(ref _lock, _writerLock, 0)) {
			w.SpinOnce();
		}
	}

	/// <summary>
	/// read lock must be aquired prior to calling this!!! we do not check it !!!!
	/// </summary>
	public void UpgradeToWrite() {
		var w = new SpinWait();

		while (1 != Interlocked.CompareExchange(ref _lock, _writerLock + 1, 1)) {
			w.SpinOnce();
		}
	}

	public void DowngradeToRead() {
		_lock = 1;
	}

	public void ExitReadLock() {
		Interlocked.Decrement(ref _lock);
	}

	public void ExitWriteLock() {
		_lock = 0;
	}

	public override string ToString() {
		return "lock counter: " + _lock;
	}

}
