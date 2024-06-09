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
/// Provides a mechanism that synchronizes access to objects,
/// allowing one thread at a time to acquire the lock, and preventing reentry by
/// the locking thread.
/// </summary>
public class NonReentrantLock {
	private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
	private int? _owningThreadId = null;

	/// <summary>
	/// Gets a value indicating whether the lock is currently acquired.
	/// </summary>
	/// <value>
	///   <c>true</c> if the lock is acquired; otherwise, <c>false</c>.
	/// </value>
	public bool IsLocked => _owningThreadId.HasValue;

	/// <summary>
	/// Acquires the lock and returns an IDisposable that releases the lock when disposed.
	/// This method is intended to be used in a using statement.
	/// </summary>
	/// <returns>An IDisposable that releases the lock when disposed.</returns>
	/// <example>
	/// <code>
	/// using (var scope = myNonReentrantLock.EnterLockScope()) {
	///     // Code protected by the lock goes here.
	/// }
	/// // The lock is automatically released when the using block is exited.
	/// </code>
	/// </example>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IDisposable EnterLockScope() {
		Acquire();
		return new ActionScope(Release);
	}

	/// <summary>
	/// Attempts to acquire the lock.
	/// If the current thread already holds the lock, throws an InvalidOperationException.
	/// If the lock is not available, waits for it.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Acquire() {
		var currentThreadId = Thread.CurrentThread.ManagedThreadId;
		if (_owningThreadId == currentThreadId)
			throw new InvalidOperationException("Reentry is not allowed.");
		_semaphore.Wait();
		_owningThreadId = currentThreadId;
	}

	/// <summary>
	/// Releases the lock.
	/// If the current thread does not hold the lock, throws an InvalidOperationException.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Release() {
		var currentThreadId = Thread.CurrentThread.ManagedThreadId;
		if (_owningThreadId != currentThreadId)
			throw new InvalidOperationException("The lock can only be released by the thread that acquired it.");
		_owningThreadId = null;
		_semaphore.Release();
	}
}
