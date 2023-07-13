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

public class RestrictedConnection : DbConnectionDecorator {
	public RestrictedConnection(IDbConnection internalConnection)
		: base(internalConnection) {
	}

	public IDbConnection DangerousInternalConnection => base.InternalConnection;

	public sealed override IDbTransaction BeginTransaction() {
		throw new NotSupportedException("Please use DACScope.BeginTransaction. Creating Transactions directly from Connection is prohibited.");
	}

	public sealed override IDbTransaction BeginTransaction(IsolationLevel il) {
		throw new NotSupportedException("Please use DACScope.BeginTransaction. Creating Transactions directly from Connection is prohibited.");
	}

	public override void Close() {
		throw new NotSupportedException("Cannot Close Connection as it is being managed by DACScope.");
	}
	public sealed override void Dispose() {
		throw new NotSupportedException("Cannot Dispose Connection as it is being managed by DACScope.");
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
