// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public abstract class AsyncTransactionalScope<TTransaction> : TransactionalScopeBase<TTransaction> {

	protected AsyncTransactionalScope(ContextScopePolicy policy, string contextID) : base(policy, contextID) {
	}

	protected sealed override TTransaction BeginTransactionInternal() => BeginTransactionInternalAsync().ResultSafe();

	protected sealed override void CloseTransactionInternal(TTransaction transaction) => CloseTransactionInternalAsync(transaction).WaitSafe();

	protected sealed override void CommitInternal(TTransaction transaction) => CommitInternalAsync(transaction).WaitSafe();

	protected sealed override void RollbackInternal(TTransaction transaction) => RollbackInternalAsync(transaction).WaitSafe();

	protected sealed override void OnContextEnd() => OnContextEndAsync().AsTask().WaitSafe();

	protected sealed override void OnTransactionalScopeEnd(List<Exception> errors) => OnTransactionalScopeEndAsync(errors).WaitSafe();

	protected sealed override void OnCommitting() => OnCommittingAsync().WaitSafe();

	protected sealed override void OnCommitted() => OnCommittedAsync().WaitSafe();

	protected sealed override void OnRollingBack() => OnRollingBackAsync().WaitSafe();

	protected sealed override void OnRolledBack() => OnRolledBackAsync().WaitSafe();

}
