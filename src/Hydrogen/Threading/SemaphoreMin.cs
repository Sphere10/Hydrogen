// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Hydrogen;

/// <summary>
/// A semaphore that uses <see cref="Monitor"/> locking under the hood. 
/// </summary>
/// <remarks>This was created as a replacement for <see cref="SemaphoreSlim"/> which was believed to be a cause of a bug. Turns out Semaphore's can become very slow if logical threads >> physical cores and they wait on a semaphore.
/// <see cref="ProducerConsumerLock"/> comments for discussion).</remarks>
internal class SemaphoreMin : SyncDisposable {

	private readonly object _lock;
	private long _currentCount;

	public SemaphoreMin(int initialCount) {
		Guard.Argument(initialCount == 1, nameof(initialCount), "Only value of 1 is supported in current implementation");
		_lock = new object();
		_currentCount = 0;
	}

	public int CurrentCount => (int)Interlocked.Read(ref _currentCount);

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Wait() {
		Interlocked.Increment(ref _currentCount);
		Monitor.Enter(_lock);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Release() {
		Interlocked.Decrement(ref _currentCount);
		Monitor.Exit(_lock);
	}

	protected override void FreeManagedResources() {
		if (_currentCount != 0)
			throw new InvalidOperationException("SemaphoreMin has an active waiter");
		if (Monitor.IsEntered(_lock))
			Monitor.Exit(_lock);
	}
}
