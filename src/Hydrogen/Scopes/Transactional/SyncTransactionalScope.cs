using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen;

// Note: needs refactoring to support async impl
public abstract class SyncTransactionalScope<TTransaction> : TransactionalScopeBase<TTransaction> {

	protected SyncTransactionalScope(ContextScopePolicy policy, string contextID) : base(policy, contextID) {
	}

	protected sealed override Task<TTransaction> BeginTransactionInternalAsync() => Task.Run(BeginTransactionInternal);

	protected sealed override Task CloseTransactionInternalAsync(TTransaction transaction) => Task.Run(() => CloseTransactionInternal(transaction));

	protected sealed override Task CommitInternalAsync(TTransaction transaction) => Task.Run(() => CommitInternal(transaction));

	protected sealed override Task RollbackInternalAsync(TTransaction transaction) => Task.Run(() => RollbackInternal(transaction));

	protected sealed override async ValueTask OnContextEndAsync() => await Task.Run(OnContextEnd);

	protected sealed override Task OnTransactionalScopeEndAsync(List<Exception> errors) => Task.Run(() =>OnTransactionalScopeEnd(errors));

	protected sealed override Task OnCommittingAsync() => Task.Run(OnCommitting);

	protected sealed override Task OnCommittedAsync() => Task.Run(OnCommitted);

	protected sealed override Task OnRollingBackAsync() => Task.Run(OnRollingBack);

	protected sealed override Task OnRolledBackAsync() => Task.Run(OnRolledBack);

}