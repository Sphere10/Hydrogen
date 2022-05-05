//-----------------------------------------------------------------------
// <copyright file="DbTransactionDecorator.cs" company="Sphere 10 Software">
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
}
