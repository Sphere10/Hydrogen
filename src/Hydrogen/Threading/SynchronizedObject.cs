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

public class SynchronizedObject : ISynchronizedObject {
	internal readonly ReaderWriterLockSlim _threadLock;

	public SynchronizedObject()
		: this(LockRecursionPolicy.SupportsRecursion) {
	}

	public SynchronizedObject(LockRecursionPolicy policy) {
		_threadLock = new ReaderWriterLockSlim(policy);
	}

	public ISynchronizedObject ParentSyncObject { get; set; }

	public ReaderWriterLockSlim ThreadLock => ParentSyncObject?.ThreadLock ?? _threadLock;

	public IDisposable EnterReadScope() {
		ThreadLock.EnterReadLock();
		OnReadScopeOpen();
		var scope = new ActionDisposable(
			() => {
				ThreadLock.ExitReadLock();
				OnReadScopeClosed();
			}
		);
		return scope;
	}

	public IDisposable EnterWriteScope() {
		ThreadLock.EnterWriteLock();
		OnWriteScopeOpen();
		var scope = new ActionDisposable(
			() => {
				ThreadLock.ExitWriteLock();
				OnWriteScopeClosed();
			}
		);
		return scope;
	}

	protected virtual void OnReadScopeOpen() {
	}

	protected virtual void OnReadScopeClosed() {
	}

	protected virtual void OnWriteScopeOpen() {
	}

	protected virtual void OnWriteScopeClosed() {
	}

	protected virtual void EnsureReadable() {
		if (!(ThreadLock.IsReadLockHeld || ThreadLock.IsUpgradeableReadLockHeld))
			throw new SoftwareException("Resource has not entered a read scope");
	}

	protected virtual void EnsureWritable() {
		if (!ThreadLock.IsWriteLockHeld)
			throw new SoftwareException("Resource has not entered a write scope");
	}

}


public class SynchronizedObject<TReadScope, TWriteScope> : ISynchronizedObject<TReadScope, TWriteScope>
	where TReadScope : IScope, new()
	where TWriteScope : IScope, new() {
	internal readonly ReaderWriterLockSlim _threadLock;

	public SynchronizedObject()
		: this(LockRecursionPolicy.SupportsRecursion) {
	}

	public SynchronizedObject(LockRecursionPolicy policy) {
		_threadLock = new ReaderWriterLockSlim(policy);
	}

	public ISynchronizedObject<TReadScope, TWriteScope> ParentSyncObject { get; set; }

	ISynchronizedObject ISynchronizedObject.ParentSyncObject {
		get => ParentSyncObject;
		set => ParentSyncObject = (ISynchronizedObject<TReadScope, TWriteScope>)value;
	}

	public ReaderWriterLockSlim ThreadLock => ParentSyncObject?.ThreadLock ?? _threadLock;

	public TReadScope EnterReadScope() {
		ThreadLock.EnterReadLock();
		OnReadScopeOpen();
		var scope = new TReadScope();
		scope.ScopeEnd += () => {
			ThreadLock.ExitReadLock();
			OnReadScopeClosed();
		};
		return scope;
	}

	IDisposable ISynchronizedObject.EnterReadScope() {
		return EnterReadScope();
	}

	public TWriteScope EnterWriteScope() {
		ThreadLock.EnterWriteLock();
		OnWriteScopeOpen();
		var scope = new TWriteScope();
		scope.ScopeEnd += () => {
			ThreadLock.ExitWriteLock();
			OnWriteScopeClosed();
		};
		return scope;
	}

	IDisposable ISynchronizedObject.EnterWriteScope() {
		return EnterWriteScope();
	}

	protected virtual void OnReadScopeOpen() {
	}

	protected virtual void OnReadScopeClosed() {
	}

	protected virtual void OnWriteScopeOpen() {
	}

	protected virtual void OnWriteScopeClosed() {
	}

	protected virtual void EnsureReadable() {
		if (!(ThreadLock.IsReadLockHeld || ThreadLock.IsUpgradeableReadLockHeld))
			throw new SoftwareException("Resource has not entered a read scope");
	}

	protected virtual void EnsureWritable() {
		if (!ThreadLock.IsWriteLockHeld)
			throw new SoftwareException("Resource has not entered a write scope");
	}

}
