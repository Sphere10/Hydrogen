using System;

namespace Hydrogen;

public sealed class ActionTransactionalScope<TTransaction> : SyncTransactionalScope<TTransaction> {

	private readonly Func<TTransaction> _beginTransactionFunc;
	private readonly Action<TTransaction> _commitTransactionFunc;
	private readonly Action<TTransaction> _rollbackTransactionFunc;
	private readonly Action<TTransaction> _closeTransactionFunc;

	public ActionTransactionalScope(
		ContextScopePolicy policy,
		string contextID,
		Func<TTransaction> beginTransactionFunc,
		Action<TTransaction> commitTransactionFunc,
		Action<TTransaction> rollbackTransactionFunc,
		Action<TTransaction> closeTransactionFunc)
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

	protected override TTransaction BeginTransactionInternal() => _beginTransactionFunc();

	protected override void CloseTransactionInternal(TTransaction transaction) => _closeTransactionFunc(transaction);

	protected override void CommitInternal(TTransaction transaction) => _commitTransactionFunc(transaction);

	protected override void RollbackInternal(TTransaction transaction) => _rollbackTransactionFunc(transaction);
}
