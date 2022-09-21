using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen;

// Note: needs refactoring to support async impl
public abstract class TransactionalScopeBase<TTransaction> : ContextScope, ITransactionalScope {

	public event EventHandlerEx<object> Committing;
	public event EventHandlerEx<object> Committed;
	public event EventHandlerEx<object> RollingBack;
	public event EventHandlerEx<object> RolledBack;

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

	public TransactionAction? DefaultCloseAction { get; init; } = null;

	public TTransaction Transaction { get; private set; }

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
		if (!InException && _scopeHasOpenTransaction && DefaultCloseAction.HasValue) {
			switch (DefaultCloseAction.Value) {
				case TransactionAction.Commit:
					Commit();
					break;
				case TransactionAction.Rollback:
					Rollback();
					break;
				default:
					throw new NotSupportedException(DefaultCloseAction.Value.ToString());
			}
		}
		var scopeWasInOpenTransaction = _scopeHasOpenTransaction;
		var errors = new List<Exception>();
		if (Transaction != null && _scopeOwnsTransaction) {
			Tools.Exceptions.ExecuteCapturingException(CloseTransaction, errors);
		}

		// Allow sub-class to cleanup
		OnTransactionalScopeEnd(errors);

		if (scopeWasInOpenTransaction && !InException) {
			errors.Add(new SoftwareException("DacScope transaction was left open. Please call Commit or Rollback explicitly to close the transaction."));
		}

		if (!InException) {
			switch (errors.Count) {
				case 0:
					break;
				case 1:
					throw errors[0];
				default:
					throw new AggregateException(errors);
			}
		}
	}

	protected sealed override async ValueTask OnScopeEndInternalAsync() {
		if (!InException && _scopeHasOpenTransaction && DefaultCloseAction.HasValue) {
			switch (DefaultCloseAction.Value) {
				case TransactionAction.Commit:
					await CommitAsync();
					break;
				case TransactionAction.Rollback:
					await RollbackAsync();
					break;
				default:
					throw new NotSupportedException(DefaultCloseAction.Value.ToString());
			}
		}
		var scopeWasInOpenTransaction = _scopeHasOpenTransaction;
		var errors = new List<Exception>();
		if (Transaction != null && _scopeOwnsTransaction) {
			await CloseTransactionAsync().IgnoringExceptions(errors);
		}

		// Allow sub-class to cleanup
		await OnTransactionalScopeEndAsync(errors);

		if (scopeWasInOpenTransaction && !InException) {
			errors.Add(new SoftwareException("DacScope transaction was left open. Please call Commit or Rollback explicitly to close the transaction."));
		}

		if (!InException) {
			switch (errors.Count) {
				case 0:
					break;
				case 1:
					throw errors[0];
				default:
					throw new AggregateException(errors);
			}
		}
	}


	protected override void OnContextEnd() {
		// nothing to do since handled by scope end
	}

	protected override async ValueTask OnContextEndAsync() {
		// nothing to do since all handled by scope end
	}

	protected virtual void OnTransactionalScopeEnd(List<Exception> errors) {
	}

	protected virtual async Task OnTransactionalScopeEndAsync(List<Exception> errors) {
	}

	protected virtual void OnCommitting() {
	}

	protected virtual async Task OnCommittingAsync() {
	}

	protected virtual void OnCommitted() {
	}

	protected virtual async Task OnCommittedAsync() {
	}

	protected virtual void OnRollingBack() {
	}

	protected virtual async Task OnRollingBackAsync() {
	}

	protected virtual void OnRolledBack() {
	}

	protected virtual async Task OnRolledBackAsync() {
	}

	private void NotifyCommitting() {
		OnCommitting();
		Committing?.Invoke(this);
	}

	private async Task NotifyCommittingAsync() {
		await OnCommittingAsync();
		Committing?.Invoke(this);
	}

	private void NotifyCommitted() {
		OnCommitted();
		Committed?.Invoke(this);
	}

	private async Task NotifyCommittedAsync() {
		await OnCommittedAsync();
		Committed?.Invoke(this);
	}

	private void NotifyRollingBack() {
		OnRollingBack();
		RollingBack?.Invoke(this);
	}

	private async Task NotifyRollingBackAsync() {
		await OnRollingBackAsync();
		RollingBack?.Invoke(this);
	}

	private void NotifyRolledBack() {
		OnRolledBack();
		RolledBack?.Invoke(this);
	}

	private async Task NotifyRolledBackAsync() {
		await OnRolledBackAsync();
		RolledBack?.Invoke(this);
	}

	protected void CheckTransactionExists() {
		if (Transaction == null)
			throw new SoftwareException("No transaction has been declared");
	}


}