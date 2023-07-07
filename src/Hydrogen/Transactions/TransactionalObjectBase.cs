// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

public abstract class TransactionalObjectBase : ITransactionalObject {

	public event EventHandlerEx<object> Committing;
	public event EventHandlerEx<object> Committed;
	public event EventHandlerEx<object> RollingBack;
	public event EventHandlerEx<object> RolledBack;

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
		if (Committing != null) {
			Committing(this);
		}
	}

	protected void NotifyCommitted() {
		OnCommitted();
		if (Committed != null) {
			Committed(this);
		}
	}

	protected void NotifyRollingBack() {
		OnRollingBack();
		if (RollingBack != null) {
			RollingBack(this);
		}
	}

	protected void NotifyRolledBack() {
		OnRolledBack();
		if (RolledBack != null) {
			RolledBack(this);
		}
	}



}