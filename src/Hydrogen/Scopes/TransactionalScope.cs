using System;
using System.Collections.Generic;

namespace Hydrogen {

	public abstract class TransactionalScope<TScope, TTransaction> : SyncContextScopeBase<TScope>
		where TScope : TransactionalScope<TScope, TTransaction> { 

		private bool _scopeOwnsTransaction;
		private TScope _transactionOwner;
		private bool _scopeHasOpenTransaction;
		private bool _voteRollback;
		private readonly Func<TScope, TTransaction> _beginTransactionFunc;
		private readonly Action<TScope, TTransaction> _commitTransactionFunc;
		private readonly Action<TScope, TTransaction> _rollbackTransactionFunc;
		private readonly Action<TScope, TTransaction> _disposeTransactionFunc;
		private TransactionAction? _defaultCloseAction;

		protected TransactionalScope(
			ContextScopePolicy policy,
			string contextId,
			Func<TScope, TTransaction> beginTransactionFunc,
			Action<TScope, TTransaction> commitTransactionFunc,
			Action<TScope, TTransaction> rollbackTransactionFunc,
			Action<TScope, TTransaction> disposeTransactionFunc,
			TransactionAction? defaultCloseAction = null)
			: base(policy, contextId) {

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

		public virtual void BeginTransaction() {
			if (Transaction == null) {
				Transaction = _beginTransactionFunc((TScope)this);
				_scopeOwnsTransaction = true;
				_transactionOwner = (TScope)this;
				// make sure child scopes can see this transaction and owner by placing it in rootscope (cleaned up on exit)
				RootScope.Transaction = Transaction;
				RootScope._transactionOwner = (TScope)this;
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
				_rollbackTransactionFunc((TScope)this, Transaction);
			} else {
				_transactionOwner._voteRollback = true;
			}
			CloseTransaction();
		}

		public virtual void Commit() {
			CheckTransactionExists();
			if (_scopeOwnsTransaction) {
				if (!_voteRollback) {
					_commitTransactionFunc((TScope)this, Transaction);
				} else {
					_rollbackTransactionFunc((TScope)this, Transaction);
				}
			}
			CloseTransaction();
		}

		protected sealed override void OnScopeEnd(TScope rootScope, bool inException) {
			if (!inException && _scopeHasOpenTransaction && _defaultCloseAction.HasValue) {
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
			OnScopeEndInternal(rootScope, inException, errors);

			if (scopeWasInOpenTransaction && !inException) {
				errors.Add(new SoftwareException("DacScope transaction was left open. Please call Commit or Rollback explicitly to close the transaction."));
			}

			if (!inException) {
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

		protected virtual void OnScopeEndInternal(TScope rootScope, bool inException, List<Exception> errors) {
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
					_disposeTransactionFunc((TScope)this, Transaction);
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