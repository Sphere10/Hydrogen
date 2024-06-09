// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;

namespace Hydrogen;

public sealed class FastLock {
	private readonly ReaderWriterLockSlim _lock;

	public FastLock() {
		_lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
	}

	public IDisposable EnterLockScope() {
		_lock.EnterWriteLock();
		return new ActionScope(() => _lock.ExitWriteLock());
	}

}
