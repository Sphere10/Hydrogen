//-----------------------------------------------------------------------
// <copyright file="DbConnectionDecorator.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Data {
    public abstract class DbConnectionDecorator : IDbConnection {
        protected readonly IDbConnection InternalConnection;


		protected DbConnectionDecorator(IDbConnection internalConnection) {
            InternalConnection = internalConnection;
        }

        public virtual void Dispose() {
            InternalConnection.Dispose();
        }

        public virtual IDbTransaction BeginTransaction() {
            return InternalConnection.BeginTransaction();
        }

        public virtual IDbTransaction BeginTransaction(IsolationLevel il) {
            return InternalConnection.BeginTransaction(il);
        }

        public virtual void Close() {
            InternalConnection.Close();
        }

        public virtual void ChangeDatabase(string databaseName) {
            InternalConnection.ChangeDatabase(databaseName);
        }

        public virtual IDbCommand CreateCommand() {
            return InternalConnection.CreateCommand();
        }

        public virtual void Open() {
            InternalConnection.Open();
        }

        public virtual string ConnectionString {
            get => InternalConnection.ConnectionString;
			set => InternalConnection.ConnectionString = value;
		}

        public virtual int ConnectionTimeout => InternalConnection.ConnectionTimeout;

		public virtual string Database => InternalConnection.Database;

		public virtual ConnectionState State => InternalConnection.State;
	}
}
