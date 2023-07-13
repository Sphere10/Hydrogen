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
using System.Threading.Tasks;
using NHibernate;

namespace Hydrogen.Data.NHibernate;

public class DataAccessScope : TransactionalScopeBase<ITransaction> {
	private const string DefaultContextPrefix = "A8BFD7F5-FB72-4105-9E2C-A924E903500F";
	private const string ContextNameTemplate = "DataAccessScope:{0}:{1}:{2}";
	private IsolationLevel? _transactionIsolationLevel;
	protected readonly bool ScopeOwnsSession;


	public DataAccessScope(INHDatabaseManager sessionProvider, DBMSType dbmsType, string connectionString, ContextScopePolicy policy = ContextScopePolicy.None, string contextPrefix = DefaultContextPrefix)
		: this(sessionProvider, new DBReference(dbmsType, connectionString), policy) {
	}

	public DataAccessScope(INHDatabaseManager sessionProvider, DBReference databaseReference, ContextScopePolicy policy = ContextScopePolicy.None, string contextPrefix = DefaultContextPrefix)
		: base(policy, string.Format(ContextNameTemplate, contextPrefix, databaseReference.DBMSType, databaseReference.ConnectionString)) {
		DatabaseReference = databaseReference;
		var withinSystemTransactionScope = System.Transactions.Transaction.Current != null;
		if (withinSystemTransactionScope)
			throw new Exception("System transactions not supported, please use DataAccessScope transactions only");

		if (IsRootScope) {
			var sessionFactory = sessionProvider.OpenDatabase(databaseReference.ConnectionString);
			Session = sessionFactory.OpenSession();
			Guard.Ensure(Session != null);
			ScopeOwnsSession = true;
			_transactionIsolationLevel = null;
		} else {
			Session = RootScope.Session ?? throw new InternalErrorException("609D85A1-785E-4E80-8429-933D5381B4DA");
			_transactionIsolationLevel = RootScope._transactionIsolationLevel;
			ScopeOwnsSession = false;
		}
	}

	public new DataAccessScope RootScope => base.RootScope as DataAccessScope;

	public DBReference DatabaseReference { get; }
	public ISession Session { get; }

	public sealed override void BeginTransaction() {
		BeginTransaction(IsolationLevel.ReadCommitted);
	}

	public void BeginTransaction(IsolationLevel isolationLevel) {
		CheckNoSystemTransaction();
		_transactionIsolationLevel = isolationLevel;

		// parent scope already defined a transaction, so check isolation levels match
		/*if (Transaction != null && isolationLevel > Transaction.IsolationLevel)
			throw new SoftwareException("A transaction already exists with lower isolation level. Requested = {0}, Current = {1}", isolationLevel, Transaction.IsolationLevel);*/

		base.BeginTransaction();
	}

	public override Task BeginTransactionAsync()
		=> BeginTransactionAsync(IsolationLevel.ReadCommitted);

	public Task BeginTransactionAsync(IsolationLevel isolationLevel) {
		CheckNoSystemTransaction();
		_transactionIsolationLevel = isolationLevel;
		return base.BeginTransactionAsync();
	}

	protected override ITransaction BeginTransactionInternal() {
		Guard.Ensure(_transactionIsolationLevel != null, "Isolation level was not set");
		return Session.BeginTransaction(_transactionIsolationLevel.Value);
	}

	protected override Task<ITransaction> BeginTransactionInternalAsync()
		=> Task.Run(BeginTransactionInternal);

	protected override void CloseTransactionInternal(ITransaction transaction) {
		transaction.Dispose();
		_transactionIsolationLevel = null;
	}

	protected override Task CloseTransactionInternalAsync(ITransaction transaction)
		=> Task.Run(() => CloseTransactionInternal(transaction));

	protected override void CommitInternal(ITransaction transaction)
		=> transaction.Commit();

	protected override Task CommitInternalAsync(ITransaction transaction)
		=> transaction.CommitAsync();

	protected override void RollbackInternal(ITransaction transaction)
		=> transaction.Rollback();

	protected override Task RollbackInternalAsync(ITransaction transaction)
		=> transaction.RollbackAsync();

	protected override void OnTransactionalScopeEnd(List<Exception> errors) {
		if (ScopeOwnsSession) {
			Tools.Exceptions.ExecuteCapturingException(Session.Flush, errors);
			Tools.Exceptions.ExecuteCapturingException(() => Session.Close(), errors);
			Tools.Exceptions.ExecuteCapturingException(Session.Dispose, errors);
		}
	}

	protected override async Task OnTransactionalScopeEndAsync(List<Exception> errors) {
		if (ScopeOwnsSession) {
			await Session.FlushAsync().IgnoringExceptions(errors);
			Tools.Exceptions.ExecuteCapturingException(() => Session.Close(), errors);
			Tools.Exceptions.ExecuteCapturingException(Session.Dispose, errors);
		}
	}

	private void CheckNoSystemTransaction() {
		Guard.Against(System.Transactions.Transaction.Current != null, "DACScope transactions cannot be used a System.Transactions.TransactionScope.");
	}

}
