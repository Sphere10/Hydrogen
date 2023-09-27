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

public sealed class AmbientDAC : DACDecorator {

	private readonly Func<IDbConnection> _getConnectionFunc;
	private readonly Func<IsolationLevel, IDbTransaction> _getTransactionFunc;


	public AmbientDAC(IDAC decoratedDAC, Func<IDbConnection> getConnectionFunc, Func<IsolationLevel, IDbTransaction> getTransactionFunc)
		: base(decoratedDAC) {
		_getConnectionFunc = getConnectionFunc;
		_getTransactionFunc = getTransactionFunc;
	}

	public override IDbConnection CreateConnection() {
		return new AmbientConnection(_getConnectionFunc(), _getTransactionFunc);
	}


	/// <summary>
	/// Wraps a connection such that it cannot be closed or disposed. Transactions acquired from this connection
	/// are wrapped with PassThroughTransaction.
	/// 
	/// </summary>
	private class AmbientConnection : DbConnectionDecorator {
		private readonly Func<IsolationLevel, IDbTransaction> _getTransactionFunc;
		public AmbientConnection(IDbConnection connection, Func<IsolationLevel, IDbTransaction> getTransactionFunc) : base(connection) {
			_getTransactionFunc = getTransactionFunc;
		}

		public sealed override void Close() {
		}

		public sealed override void Dispose() {

		}

		public sealed override IDbTransaction BeginTransaction() {
			return BeginTransaction(IsolationLevel.ReadCommitted);
		}

		public sealed override IDbTransaction BeginTransaction(IsolationLevel il) {
			return new AmbientTransaction(this, _getTransactionFunc(il));
		}
	}


	/// <summary>
	/// A wrapper for IDbTransaction that does not dipose or commit (but can rollback).
	/// </summary>
	private class AmbientTransaction : DbTransactionDecorator {
		private readonly IDbConnection _connection;
		public AmbientTransaction(IDbConnection connection, IDbTransaction internalTransaction) : base(internalTransaction) {
			_connection = connection;
		}

		public sealed override void Dispose() {

		}

		public sealed override void Commit() {
		}

		public sealed override IDbConnection Connection {
			get { return _connection; }
		}
	}

}
