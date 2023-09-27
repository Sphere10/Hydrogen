// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Data;

namespace Hydrogen.Data;

public abstract class DbTransactionDecorator : IDbTransaction {
	protected readonly IDbTransaction InternalTransaction;

	protected DbTransactionDecorator(IDbTransaction internalTransaction) {
		InternalTransaction = internalTransaction;
	}

	public virtual void Dispose() {
		InternalTransaction.Dispose();
	}

	public virtual void Commit() {
		InternalTransaction.Commit();
	}

	public virtual void Rollback() {
		InternalTransaction.Rollback();
	}

	public virtual IDbConnection Connection {
		get { return InternalTransaction.Connection; }
	}

	public virtual IsolationLevel IsolationLevel {
		get { return InternalTransaction.IsolationLevel; }
	}
}
