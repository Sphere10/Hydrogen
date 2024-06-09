// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// Base implementation of <see cref="ITransactionalScope"/> suitable for implementing ACID
/// transactions in a repository.
/// </summary>
/// <typeparam name="TTransaction">Type of transaction</typeparam>
public abstract class TransactionalScopeBase<TTransaction> : ContextScope, ITransactionalScope {

	public event EventHandlerEx Committing;
	public event EventHandlerEx Committed;
	public event EventHandlerEx RollingBack;
	public event EventHandlerEx RolledBack;

	private bool _scopeOwnsTransaction;
	private TransactionalScopeBase<TTransaction> _transactionOwner;
	private bool _scopeHasOpenTransaction;
	private bool _voteRollback;


	protected TransactionalScopeBase(ContextScopePolicy policy, string contextID)
		: base(policy, contextID) {

		if (IsRootScope) {
			Transaction = default;
			_scopeOwnsTransaction = false;
			_transactionOwner = null;
		} else {
			Transaction = RootScope.Transaction;
			_transactionOwner = RootScope._transactionOwner;
			_scopeOwnsTransaction = false;
		}
		_voteRollback = false;
		_scopeHasOpenTransaction = false;
	}

	public TTransaction Transaction { get; private set; }

	public virtual bool ParticipatesWithinTransaction => Transaction != null;

	public new TransactionalScopeBase<TTransaction> RootScope => (TransactionalScopeBase<TTransaction>)base.RootScope;

	ITransactionalScope ITransactionalScope.RootScope => RootScope;

	public virtual void BeginTransaction() {
		if (Transaction == null) {
			Transaction = BeginTransactionInternal();
			_scopeOwnsTransaction = true;
			_transactionOwner = this;
			// make sure child scopes can see this transaction and owner by placing it in rootscope (cleaned up on exit)
			RootScope.Transaction = Transaction;
			RootScope._transactionOwner = this;
		} else {
			// if this is the parent scope, error
			if (_scopeOwnsTransaction)
				throw new SoftwareException("Scope has already created a transaction");

			// parent scope already defined a transaction, so use it
			_scopeOwnsTransaction = false;
		}
		_scopeHasOpenTransaction = true;
	}

	public virtual async Task BeginTransactionAsync() {
		if (Transaction == null) {
			Transaction = await BeginTransactionInternalAsync();
			_scopeOwnsTransaction = true;
			_transactionOwner = this;
			// make sure child scopes can see this transaction and owner by placing it in rootscope (cleaned up on exit)
			RootScope.Transaction = Transaction;
			RootScope._transactionOwner = this;
		} else {
			// if this is the parent scope, error
			if (_scopeOwnsTransaction)
				throw new SoftwareException("Scope has already created a transaction");

			// parent scope already defined a transaction, so use it
			_scopeOwnsTransaction = false;
		}
		_scopeHasOpenTransaction = true;
	}

	public virtual void Rollback() {
		CheckTransactionExists();
		NotifyRollingBack();
		if (_scopeOwnsTransaction) {
			RollbackInternal(Transaction);
		} else {
			_transactionOwner._voteRollback = true;
		}
		NotifyRolledBack();
		CloseTransaction();
	}

	public virtual async Task RollbackAsync() {
		CheckTransactionExists();
		await NotifyRollingBackAsync();
		if (_scopeOwnsTransaction) {
			await RollbackInternalAsync(Transaction);
		} else {
			_transactionOwner._voteRollback = true;
		}
		await NotifyRolledBackAsync();
		await CloseTransactionAsync();
	}

	public virtual void Commit() {
		CheckTransactionExists();
		NotifyCommitting();
		if (_scopeOwnsTransaction) {
			if (!_voteRollback) {
				CommitInternal(Transaction);
			} else {
				RollbackInternal(Transaction);
			}
		}
		NotifyCommitted();
		CloseTransaction();
	}

	public virtual async Task CommitAsync() {
		CheckTransactionExists();
		await NotifyCommittingAsync();
		if (_scopeOwnsTransaction) {
			if (!_voteRollback) {
				await CommitInternalAsync(Transaction);
			} else {
				await RollbackInternalAsync(Transaction);
			}
		}
		await NotifyCommittedAsync();
		await CloseTransactionAsync();
	}

	/// <summary>
	/// Closes the transaction. Behaviour is different for root scope, owning scope & child scope.
	/// </summary>
	protected virtual void CloseTransaction() {
		// indicate local scope has closed txn
		_scopeHasOpenTransaction = false;

		// owning scope needs to do extra
		if (_scopeOwnsTransaction) {

			// dispose transaction and remove from root scope
			if (Transaction != null) {
				CloseTransactionInternal(Transaction);
				Transaction = default;
				RootScope.Transaction = default;
			}

			// clear rollback flag
			_voteRollback = false;
			RootScope._voteRollback = false;

			// reset flags to allow consecutive txn's
			_scopeOwnsTransaction = false;
			_transactionOwner = null;
			RootScope._transactionOwner = null;
		}
	}

	/// <summary>
	/// Closes the transaction. Behaviour is different for root scope, owning scope & child scope.
	/// </summary>
	protected virtual async Task CloseTransactionAsync() {
		// indicate local scope has closed txn
		_scopeHasOpenTransaction = false;

		// owning scope needs to do extra
		if (_scopeOwnsTransaction) {

			// dispose transaction and remove from root scope
			if (Transaction != null) {
				await CloseTransactionInternalAsync(Transaction);
				Transaction = default;
				RootScope.Transaction = default;
			}

			// clear rollback flag
			_voteRollback = false;
			RootScope._voteRollback = false;

			// reset flags to allow consecutive txn's
			_scopeOwnsTransaction = false;
			_transactionOwner = null;
			RootScope._transactionOwner = null;
		}
	}

	protected abstract TTransaction BeginTransactionInternal();

	protected abstract Task<TTransaction> BeginTransactionInternalAsync();

	protected abstract void CloseTransactionInternal(TTransaction transaction);

	protected abstract Task CloseTransactionInternalAsync(TTransaction transaction);

	protected abstract void CommitInternal(TTransaction transaction);

	protected abstract Task CommitInternalAsync(TTransaction transaction);

	protected abstract void RollbackInternal(TTransaction transaction);

	protected abstract Task RollbackInternalAsync(TTransaction transaction);

	protected sealed override void OnScopeEndInternal() {
		var scopeWasInOpenTransaction = _scopeHasOpenTransaction;
		var errors = new List<Exception>();
		if (Transaction != null && _scopeOwnsTransaction) {
			Tools.Exceptions.ExecuteCapturingException(CloseTransaction, errors);
		}

		// Allow sub-class to cleanup
		OnTransactionalScopeEnd(errors);

	}

	protected sealed override async ValueTask OnScopeEndInternalAsync() {
		var scopeWasInOpenTransaction = _scopeHasOpenTransaction;
		var errors = new List<Exception>();
		if (Transaction != null && _scopeOwnsTransaction) {
			await CloseTransactionAsync().IgnoringExceptions(errors);
		}

		// Allow sub-class to cleanup
		await OnTransactionalScopeEndAsync(errors);
	}

	protected override void OnContextEnd() {
		// nothing to do since handled by scope end
	}

	protected override async ValueTask OnContextEndAsync() {
		// nothing to do since all handled by scope end
	}

	protected virtual void OnTransactionalScopeEnd(List<Exception> errors) {
	}

	protected virtual Task OnTransactionalScopeEndAsync(List<Exception> errors) => Task.CompletedTask;

	protected virtual void OnCommitting() {
	}

	protected virtual Task OnCommittingAsync() => Task.CompletedTask;

	protected virtual void OnCommitted() {
	}

	protected virtual Task OnCommittedAsync() => Task.CompletedTask;

	protected virtual void OnRollingBack() {
	}

	protected virtual Task OnRollingBackAsync() => Task.CompletedTask;

	protected virtual void OnRolledBack() {
	}

	protected virtual Task OnRolledBackAsync() => Task.CompletedTask;

	private void NotifyCommitting() {
		OnCommitting();
		Committing?.Invoke();
	}

	private async Task NotifyCommittingAsync() {
		await OnCommittingAsync();
		Committing?.Invoke();
	}

	private void NotifyCommitted() {
		OnCommitted();
		Committed?.Invoke();
	}

	private async Task NotifyCommittedAsync() {
		await OnCommittedAsync();
		Committed?.Invoke();
	}

	private void NotifyRollingBack() {
		OnRollingBack();
		RollingBack?.Invoke();
	}

	private async Task NotifyRollingBackAsync() {
		await OnRollingBackAsync();
		RollingBack?.Invoke();
	}

	private void NotifyRolledBack() {
		OnRolledBack();
		RolledBack?.Invoke();
	}

	private async Task NotifyRolledBackAsync() {
		await OnRolledBackAsync();
		RolledBack?.Invoke();
	}

	protected void CheckTransactionExists() {
		if (Transaction == null)
			throw new SoftwareException("No transaction has been declared");
	}


}
