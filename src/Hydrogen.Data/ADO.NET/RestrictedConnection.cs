//-----------------------------------------------------------------------
// <copyright file="RestrictedConnection.cs" company="Sphere 10 Software">
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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.Data {
    public class RestrictedConnection : DbConnectionDecorator {
        public RestrictedConnection(IDbConnection internalConnection)
            : base(internalConnection) {
        }

        public IDbConnection DangerousInternalConnection => base.InternalConnection;

		public sealed override IDbTransaction BeginTransaction() {
            throw new NotSupportedException("Please use DacScope.BeginTransaction. Creating Transactions directly from Connection is prohibited.");
        }

        public sealed override IDbTransaction BeginTransaction(IsolationLevel il) {
            throw new NotSupportedException("Please use DacScope.BeginTransaction. Creating Transactions directly from Connection is prohibited.");
        }

        public override void Close() {
            throw new NotSupportedException("Cannot Close Connection as it is being managed by DacScope.");
        }
        public sealed override void Dispose() {
            throw new NotSupportedException("Cannot Dispose Connection as it is being managed by DacScope.");
        }


        public RestrictedTransaction BeginTransactionInternal() {
            return new RestrictedTransaction(this, DangerousInternalConnection.BeginTransaction());
        }

        public RestrictedTransaction BeginTransactionInternal(IsolationLevel il) {
            return new RestrictedTransaction(this, DangerousInternalConnection.BeginTransaction(il));
        }

        public void CloseInternal() {
            DangerousInternalConnection.Close();
        }

        public void DisposeInternal() {
            DangerousInternalConnection.Dispose();
        }


    }

}
