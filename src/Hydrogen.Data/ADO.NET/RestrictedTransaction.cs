//-----------------------------------------------------------------------
// <copyright file="RestrictedTransaction.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.Data {
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
            throw new NotSupportedException("Please use DacScope.Commit. Committing Transactions directly is prohibited.");
        }

        public override void Rollback() {
            throw new NotSupportedException("Please use DacScope.Rollback. Rolling back Transactions directly is prohibited.");
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

}
