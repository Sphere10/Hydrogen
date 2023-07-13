// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Data;

namespace Hydrogen.Data;

public sealed class RestrictedTransaction : DbTransactionDecorator {
	private readonly RestrictedConnection _connection;

	public RestrictedTransaction(RestrictedConnection connection, IDbTransaction internalTransaction)
		: base(internalTransaction) {
		_connection = connection;
		HasBeenRolledBack = false;
	}

	public bool HasBeenRolledBack { get; private set; }

	public bool HasBeenCommitted { get; private set; }


	public new IDbTransaction DangerousInternalTransaction {
		get { return base.InternalTransaction; }
	}

	public override void Commit() {
		throw new NotSupportedException("Please use DACScope.Commit. Committing Transactions directly is prohibited.");
	}

	public override void Rollback() {
		throw new NotSupportedException("Please use DACScope.Rollback. Rolling back Transactions directly is prohibited.");
	}

	public override void Dispose() {
		throw new NotSupportedException("Dispoing transaction is prohibited. Please use DACSCope for connection and transaction management.");
	}

	public override IDbConnection Connection {
		get { return _connection; }
	}

	public void CommitInternal() {
		if (!HasBeenRolledBack) {
			InternalTransaction.Commit();
			HasBeenCommitted = true;
		}
	}

	public void RollbackInternal() {
		if (HasBeenCommitted)
			throw new SoftwareException("Transaction has already been committed.");

		InternalTransaction.Rollback();
		HasBeenRolledBack = true;
	}

	public void DisposeInternal() {
		InternalTransaction.Dispose();
	}
}
