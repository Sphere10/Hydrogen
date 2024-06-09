// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;

namespace Hydrogen;

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
