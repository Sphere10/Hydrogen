using System;
using System.Collections.Generic;

namespace Hydrogen {

	// Note: needs refactoring to support async impl
	public abstract class TransactionalScope<TTransaction> : SyncContextScope { 

		private bool _scopeOwnsTransaction;
		private TransactionalScope<TTransaction> _transactionOwner;
		private bool _scopeHasOpenTransaction;
		private bool _voteRollback;
		private readonly Func<IContextScope, TTransaction> _beginTransactionFunc;
		private readonly Action<IContextScope, TTransaction> _commitTransactionFunc;
		private readonly Action<IContextScope, TTransaction> _rollbackTransactionFunc;
		private readonly Action<IContextScope, TTransaction> _disposeTransactionFunc;
		private TransactionAction? _defaultCloseAction;

		protected TransactionalScope(
			ContextScopePolicy policy,
			string contextID,
			Func<IContextScope, TTransaction> beginTransactionFunc,
			Action<IContextScope, TTransaction> commitTransactionFunc,
			Action<IContextScope, TTransaction> rollbackTransactionFunc,
			Action<IContextScope, TTransaction> disposeTransactionFunc,
			TransactionAction? defaultCloseAction = null)
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
			_beginTransactionFunc = beginTransactionFunc;
			_commitTransactionFunc = commitTransactionFunc;
			_rollbackTransactionFunc = rollbackTransactionFunc;
			_disposeTransactionFunc = disposeTransactionFunc;
			_defaultCloseAction = defaultCloseAction;
		}

		public TTransaction Transaction { get; private set; }
		
		public new TransactionalScope<TTransaction> RootScope => (TransactionalScope<TTransaction>)base.RootScope;

		public virtual void BeginTransaction() {
			if (Transaction == null) {
				Transaction = _beginTransactionFunc(this);
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
			if (_scopeOwnsTransaction) {
				_rollbackTransactionFunc(this, Transaction);
			} else {
				_transactionOwner._voteRollback = true;
			}
			CloseTransaction();
		}

		public virtual void Commit() {
			CheckTransactionExists();
			if (_scopeOwnsTransaction) {
				if (!_voteRollback) {
					_commitTransactionFunc(this, Transaction);
				} else {
					_rollbackTransactionFunc(this, Transaction);
				}
			}
			CloseTransaction();
		}

		protected sealed override void OnScopeEndInternal() {
			if (!InException && _scopeHasOpenTransaction && _defaultCloseAction.HasValue) {
				switch (_defaultCloseAction.Value) {
					case TransactionAction.Commit:
						Commit();
						break;
					case TransactionAction.Rollback:
						Rollback();
						break;
					default:
						throw new NotSupportedException(_defaultCloseAction.Value.ToString());
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

		protected override void OnContextEnd() {
			// nothing to do, handled by scope end
		}

		protected virtual void OnTransactionalScopeEnd(List<Exception> errors) {
		}

		/// <summary>
		/// Closes the transaction. Behaviour is different for root scope, owning scope & child scope.
		/// </summary>
		private void CloseTransaction() {
			// indicate local scope has closed txn
			_scopeHasOpenTransaction = false;

			// owning scope needs to do extra
			if (_scopeOwnsTransaction) {

				// dispose transaction and remove from root scope
				if (Transaction != null) {
					_disposeTransactionFunc(this, Transaction);
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

		protected void CheckTransactionExists() {
			if (Transaction == null)
				throw new SoftwareException("No transaction has been declared");
		}
	}

}