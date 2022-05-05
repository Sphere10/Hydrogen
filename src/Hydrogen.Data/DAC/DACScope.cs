//-----------------------------------------------------------------------
// <copyright file="DACScope.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;


namespace Hydrogen.Data {

	public sealed class DACScope : TransactionalScope<DACScope, IDbTransaction> {
        private const string DefaultContextPrefix = "EA9CC911-C209-42B9-B113-84562706145D";
        private const string ContextNameTemplate = "DACScope:{0}:{1}";
        private readonly bool _scopeOwnsConnection;
	    private RestrictedConnection _connection;
	    private bool _withinSystemTransactionScope;
		private IsolationLevel _transactionIsolationLevel;
		internal DACScope(IDAC dac, ScopeContextPolicy policy, bool openConnection, string contextPrefix = DefaultContextPrefix, TransactionAction? defaultCloseAction = null)
			: base(
				  policy, 
				  string.Format(ContextNameTemplate, contextPrefix, dac.ConnectionString),
				  (scope) => scope._connection.BeginTransactionInternal(scope._transactionIsolationLevel), 
				  (scope, txn) => ((RestrictedTransaction)txn).DangerousInternalTransaction.Commit(),
				  (scope, txn) => ((RestrictedTransaction)txn).DangerousInternalTransaction.Rollback(),
				  (scope, txn) => ((RestrictedTransaction)txn).DangerousInternalTransaction.Dispose(),
				  defaultCloseAction
				) {
			DAC = dac ?? throw new ArgumentNullException(nameof(dac));
		    if (IsRootScope) {
                _connection = new RestrictedConnection( DAC.CreateConnection());
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
			_transactionIsolationLevel = isolationLevel;
			if (_withinSystemTransactionScope) {
	            _withinSystemTransactionScope = System.Transactions.Transaction.Current != null; // may have been removed
	            if (_withinSystemTransactionScope) {
	                throw new SoftwareException("DACScope transactions cannot be used a System.Transactions.TransactionScope.");
	            }
	        }

			// parent scope already defined a transaction, so check isolation levels match
			if (Transaction != null && isolationLevel > Transaction.IsolationLevel)
				throw new SoftwareException("A transaction already exists with lower isolation level. Requested = {0}, Current = {1}", isolationLevel, Transaction.IsolationLevel);
			
			base.BeginTransaction();
	    }

	    public override void Rollback() {
            if (_withinSystemTransactionScope) {
                throw new SoftwareException("DACScope transactions cannot be used within a System.Transactions.TransactionScope.");
            }
			base.Rollback();
	    }

	    public override void Commit() {
	        if (_withinSystemTransactionScope) {
	            throw new SoftwareException("DACScope transactions cannot be used a System.Transactions.TransactionScope.");
	        }
			base.Commit();
	    }

	    protected override void OnScopeEndInternal(DACScope rootScope, bool inException, List<Exception> errors) {
	        if (_scopeOwnsConnection) {
	            Tools.Exceptions.ExecuteIgnoringException(_connection.CloseInternal, errors);
	            Tools.Exceptions.ExecuteIgnoringException(_connection.DisposeInternal, errors);
	            _connection = null;
	        }
	    }

	    public new static DACScope GetCurrent(string connectionString) {
	        return ScopeContext<DACScope>.GetCurrent(string.Format(ContextNameTemplate, DefaultContextPrefix, connectionString));
	    }

        public static DACScope GetCurrent(string connectionString, string contextPrefix) {
            return ScopeContext<DACScope>.GetCurrent(string.Format(ContextNameTemplate, contextPrefix, connectionString));
        }
    
	}

}
