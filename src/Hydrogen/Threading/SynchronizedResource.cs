// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading;

namespace Hydrogen;

public abstract class SynchronizedResource : SynchronizedResource<Scope, Scope>, ISynchronizedResource {
}


public abstract class SynchronizedResource<TReadScope, TWriteScope> : SynchronizedObject<TReadScope, TWriteScope>, ISynchronizedResource<TReadScope, TWriteScope>
	where TReadScope : IScope, new()
	where TWriteScope : IScope, new() {
	private readonly Synchronized<ScopeTracker> _scopeTracker;

	public event EventHandlerEx<object> Initializing;
	public event EventHandlerEx<object> InitializingRead;
	public event EventHandlerEx<object> InitializingWrite;
	public event EventHandlerEx<object> Initialized;
	public event EventHandlerEx<object> InitializedRead;
	public event EventHandlerEx<object> InitializedWrite;
	public event EventHandlerEx<object> Finalizing;
	public event EventHandlerEx<object> FinalizingRead;
	public event EventHandlerEx<object> FinalizingWrite;
	public event EventHandlerEx<object> Finalized;
	public event EventHandlerEx<object> FinalizedRead;
	public event EventHandlerEx<object> FinalizedWrite;

	protected SynchronizedResource() : this(LockRecursionPolicy.SupportsRecursion) {
	}

	protected SynchronizedResource(LockRecursionPolicy policy) : base(policy) {
		_scopeTracker = new Synchronized<ScopeTracker>(ScopeTracker.Zero);
	}

	protected abstract void InitializeReadScope();

	protected abstract void InitializeWriteScope();

	protected abstract void FinalizeReadScope();

	protected abstract void FinalizeWriteScope();

	protected virtual void OnScopeInitializing() {
	}

	protected virtual void OnScopeInitialized() {
	}

	protected virtual void OnScopeFinalizing() {
	}

	protected virtual void OnScopeFinalized() {
	}

	protected virtual void OnReadScopeInitializing() {
	}

	protected virtual void OnReadScopeInitialized() {
	}

	protected virtual void OnReadScopeFinalizing() {
	}

	protected virtual void OnReadScopeFinalized() {
	}

	protected virtual void OnWriteScopeInitializing() {
	}

	protected virtual void OnWriteScopeInitialized() {
	}

	protected virtual void OnWriteScopeFinalizing() {
	}

	protected virtual void OnWriteScopeFinalized() {
	}

	protected override void OnReadScopeOpen() {
		base.OnReadScopeOpen();
		var initialize = false;
		using (_scopeTracker.EnterWriteScope()) {
			if (_scopeTracker.Value.Writes == 0 && _scopeTracker.Value.Reads == 0)
				initialize = true;
			_scopeTracker.Value.Reads++;
		}
		if (initialize) {
			NotifyInitializingReadScope();
			InitializeReadScope();
			NotifyInitializedReadScope();
		}
	}

	protected override void OnWriteScopeOpen() {
		base.OnWriteScopeOpen();
		var initialize = false;
		using (_scopeTracker.EnterWriteScope()) {
			if (_scopeTracker.Value.Reads > 0)
				throw new SoftwareException("Resource already opened in read-only mode"); // this should never be thrown
			if (_scopeTracker.Value.Writes == 0)
				initialize = true;
			_scopeTracker.Value.Writes++;
		}
		if (initialize) {
			NotifyInitializingWriteScope();
			InitializeWriteScope();
			NotifyInitializedWriteScope();
		}
	}

	protected override void OnReadScopeClosed() {
		base.OnReadScopeClosed();
		var finalize = false;
		using (_scopeTracker.EnterWriteScope()) {
			// never cleanup a read scope if an outer write is still open
			if (_scopeTracker.Value.Writes == 0 && _scopeTracker.Value.Reads == 1)
				finalize = true;
			_scopeTracker.Value.Reads--;
		}
		if (finalize) {
			NotifyFinalizingReadScope();
			FinalizeReadScope();
			NotifyFinalizedReadScope();
		}
	}

	protected override void OnWriteScopeClosed() {
		base.OnWriteScopeClosed();
		var finalize = false;
		using (_scopeTracker.EnterWriteScope()) {
			if (_scopeTracker.Value.Writes == 1)
				finalize = true;
			_scopeTracker.Value.Writes--;
		}
		if (finalize) {
			NotifyFinalizingWriteScope();
			FinalizeWriteScope();
			NotifyFinalizedWriteScope();
		}
	}

	protected override void EnsureReadable() {
		using (_scopeTracker.EnterReadScope())
			if (_scopeTracker.Value.Reads <= 0)
				throw new SoftwareException("Resource has not entered a read scope");
	}

	protected override void EnsureWritable() {
		using (_scopeTracker.EnterReadScope())
			if (_scopeTracker.Value.Writes <= 0)
				throw new SoftwareException("Resource has not entered a write scope");
	}

	private void NotifyInitializing() {
		OnScopeInitializing();
		Initializing?.Invoke(this);
	}

	private void NotifyInitialized() {
		OnScopeInitialized();
		Initialized?.Invoke(this);
	}

	private void NotifyInitializingReadScope() {
		NotifyInitializing();
		OnReadScopeInitializing();
		InitializingRead?.Invoke(this);
	}

	private void NotifyInitializedReadScope() {
		NotifyInitializing();
		OnReadScopeInitialized();
		InitializedRead?.Invoke(this);
	}

	private void NotifyInitializingWriteScope() {
		NotifyInitializing();
		OnWriteScopeInitializing();
		InitializingWrite?.Invoke(this);
	}

	private void NotifyInitializedWriteScope() {
		NotifyInitialized();
		OnWriteScopeInitialized();
		InitializedWrite?.Invoke(this);
	}

	private void NotifyFinalizing() {
		OnScopeFinalizing();
		Finalizing?.Invoke(this);
	}

	private void NotifyFinalized() {
		OnScopeFinalized();
		Finalized?.Invoke(this);
	}

	private void NotifyFinalizingReadScope() {
		NotifyFinalizing();
		OnReadScopeFinalizing();
		FinalizingRead?.Invoke(this);
	}

	private void NotifyFinalizedReadScope() {
		NotifyFinalized();
		OnReadScopeFinalized();
		FinalizedRead?.Invoke(this);
	}

	private void NotifyFinalizingWriteScope() {
		NotifyFinalizing();
		OnWriteScopeFinalizing();
		FinalizingWrite?.Invoke(this);
	}

	private void NotifyFinalizedWriteScope() {
		NotifyFinalized();
		OnWriteScopeFinalized();
		FinalizedWrite?.Invoke(this);
	}


	private class ScopeTracker {
		public int Reads;
		public int Writes;
		public static ScopeTracker Zero => new ScopeTracker { Reads = 0, Writes = 0 };
	}

}
