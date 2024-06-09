// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

public abstract class TransactionalObjectBase : ITransactionalObject {

	public event EventHandlerEx Committing;
	public event EventHandlerEx Committed;
	public event EventHandlerEx RollingBack;
	public event EventHandlerEx RolledBack;

	public abstract void Commit();

	public abstract Task CommitAsync();

	public abstract void Rollback();

	public abstract Task RollbackAsync();

	protected virtual void OnCommitting() {
	}

	protected virtual void OnCommitted() {
	}

	protected virtual void OnRollingBack() {
	}
	protected virtual void OnRolledBack() {
	}

	protected void NotifyCommitting() {
		OnCommitting();
		Committing?.Invoke();
	}

	protected void NotifyCommitted() {
		OnCommitted();
		Committed?.Invoke();
	}

	protected void NotifyRollingBack() {
		OnRollingBack();
		RollingBack?.Invoke();
	}

	protected void NotifyRolledBack() {
		OnRolledBack();
		RolledBack?.Invoke();
	}

}
