using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen;

// Note: needs refactoring to support async impl
public sealed class TaskTransactionalScope<TTransaction> : AsyncTransactionalScope<TTransaction> {

	private readonly Func<Task<TTransaction>> _beginTransactionFunc;
	private readonly Func<TTransaction, Task> _commitTransactionFunc;
	private readonly Func<TTransaction, Task> _rollbackTransactionFunc;
	private readonly Func<TTransaction, Task> _closeTransactionFunc;

	public TaskTransactionalScope(
		ContextScopePolicy policy,
		string contextID,
		Func<Task<TTransaction>> beginTransactionFunc,
		Func<TTransaction, Task> commitTransactionFunc,
		Func<TTransaction, Task> rollbackTransactionFunc,
		Func<TTransaction, Task> closeTransactionFunc)
		: base(policy, contextID) {
		Guard.ArgumentNotNull(beginTransactionFunc, nameof(beginTransactionFunc));
		Guard.ArgumentNotNull(commitTransactionFunc, nameof(commitTransactionFunc));
		Guard.ArgumentNotNull(rollbackTransactionFunc, nameof(rollbackTransactionFunc));
		Guard.ArgumentNotNull(closeTransactionFunc, nameof(closeTransactionFunc));

		_beginTransactionFunc = beginTransactionFunc;
		_commitTransactionFunc = commitTransactionFunc;
		_rollbackTransactionFunc = rollbackTransactionFunc;
		_closeTransactionFunc = closeTransactionFunc;
	}

	protected override Task<TTransaction> BeginTransactionInternalAsync() => _beginTransactionFunc();

	protected override Task CloseTransactionInternalAsync(TTransaction transaction) => _closeTransactionFunc(transaction);

	protected override Task CommitInternalAsync(TTransaction transaction) => _commitTransactionFunc(transaction);

	protected override Task RollbackInternalAsync(TTransaction transaction) => _rollbackTransactionFunc(transaction);
}

