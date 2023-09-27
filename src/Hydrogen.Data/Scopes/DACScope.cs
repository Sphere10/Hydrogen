// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Data;

namespace Hydrogen.Data;

public sealed class DACScope : SyncTransactionalScope<IDbTransaction> {
	private const string DefaultContextPrefix = "EA9CC911-C209-42B9-B113-84562706145D";
	private const string ContextNameTemplate = "DACScope:{0}:{1}";
	private readonly bool _scopeOwnsConnection;
	private RestrictedConnection _connection;
	private bool _withinSystemTransactionScope;
	private IsolationLevel _transactionIsolationLevel;

	internal DACScope(IDAC dac, ContextScopePolicy policy, bool openConnection, string contextPrefix = DefaultContextPrefix)
		: base(policy, string.Format(ContextNameTemplate, contextPrefix, dac.ConnectionString)) {
		DAC = dac ?? throw new ArgumentNullException(nameof(dac));
		if (IsRootScope) {
			_connection = new RestrictedConnection(DAC.CreateConnection());
			if (openConnection)
				_connection.Open();
			_scopeOwnsConnection = true;
		} else {
			_connection = RootScope._connection ?? throw new SoftwareException("Internal Error: RootScope DAC had null connection");
			_scopeOwnsConnection = false;
			if (openConnection && _connection.State.IsIn(ConnectionState.Closed, ConnectionState.Broken))
				_connection.Open();
		}
		_withinSystemTransactionScope = System.Transactions.Transaction.Current != null;
		if (_scopeOwnsConnection && _withinSystemTransactionScope)
			DAC.EnlistInSystemTransaction(_connection.DangerousInternalConnection, System.Transactions.Transaction.Current);
		_transactionIsolationLevel = DAC.DefaultIsolationLevel;
	}

	public new DACScope RootScope => (DACScope)base.RootScope;

	public IDAC DAC { get; internal set; }

	public IDbConnection Connection => _connection;

	/// <summary>
	/// Whether or not DAC commands are subject to a DACSCope transaction or system transaction
	/// </summary>
	public bool ParticipatesWithinTransaction => _withinSystemTransactionScope || Transaction != null;

	public void EnlistInSystemTransaction() {
		if (Transaction != null) {
			throw new SoftwareException("Unable to enlist in system transaction as DACScope has declared it's own transaction");
		}

		var systemTransaction = System.Transactions.Transaction.Current;
		if (systemTransaction == null) {
			throw new SoftwareException("No system transaction has been declared (i.e. TransactionScope)");
		}
		DAC.EnlistInSystemTransaction(_connection.DangerousInternalConnection, systemTransaction);
		_withinSystemTransactionScope = true;
	}

	public sealed override void BeginTransaction() {
		BeginTransaction(DAC.DefaultIsolationLevel);
	}

	public void BeginTransaction(IsolationLevel isolationLevel) {
		CheckNoSystemTransaction();
		_transactionIsolationLevel = isolationLevel;


		// parent scope already defined a transaction, so check isolation levels match
		if (Transaction != null && isolationLevel > Transaction.IsolationLevel)
			throw new SoftwareException("A transaction already exists with lower isolation level. Requested = {0}, Current = {1}", isolationLevel, Transaction.IsolationLevel);

		base.BeginTransaction();
	}

	public override void Rollback() {
		CheckNoSystemTransaction();
		base.Rollback();
	}

	public override void Commit() {
		CheckNoSystemTransaction();
		base.Commit();
	}

	protected override IDbTransaction BeginTransactionInternal()
		=> _connection.BeginTransactionInternal(_transactionIsolationLevel);

	protected override void CloseTransactionInternal(IDbTransaction transaction)
		=> ((RestrictedTransaction)transaction).DangerousInternalTransaction.Dispose();

	protected override void CommitInternal(IDbTransaction transaction)
		=> ((RestrictedTransaction)transaction).DangerousInternalTransaction.Commit();

	protected override void RollbackInternal(IDbTransaction transaction)
		=> ((RestrictedTransaction)transaction).DangerousInternalTransaction.Rollback();

	protected override void OnTransactionalScopeEnd(List<Exception> errors) {
		if (_scopeOwnsConnection) {
			Tools.Exceptions.ExecuteCapturingException(_connection.CloseInternal, errors);
			Tools.Exceptions.ExecuteCapturingException(_connection.DisposeInternal, errors);
			_connection = null;
		}
	}

	public new static DACScope GetCurrent(string connectionString) {
		return ContextScope.GetCurrent(string.Format(ContextNameTemplate, DefaultContextPrefix, connectionString)) as DACScope;
	}

	public static DACScope GetCurrent(string connectionString, string contextPrefix) {
		return ContextScope.GetCurrent(string.Format(ContextNameTemplate, contextPrefix, connectionString)) as DACScope;
	}

	private void CheckNoSystemTransaction() {
		if (_withinSystemTransactionScope) {
			_withinSystemTransactionScope = System.Transactions.Transaction.Current != null; // may have been removed
			if (_withinSystemTransactionScope) {
				throw new SoftwareException("DACScope transactions cannot be used a System.Transactions.TransactionScope.");
			}
		}
	}
}
