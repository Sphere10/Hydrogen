// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Data;

public class ANSI2003BaseSQLBuilder : SQLBuilderBase {
	public override ISQLBuilder BeginTransaction() {
		throw new NotSupportedException();
	}

	public override ISQLBuilder CommitTransaction() {
		throw new NotSupportedException();
	}

	public override ISQLBuilder RollbackTransaction() {
		throw new NotSupportedException();
	}

	public override ISQLBuilder DisableAutoIncrementID(string table) {
		throw new NotSupportedException();
	}

	public override ISQLBuilder EnableAutoIncrementID(string table) {
		throw new NotSupportedException();
	}

	public override ISQLBuilder NextSequenceValue(string sequenceName) {
		throw new NotSupportedException();
	}

	public override ISQLBuilder GetLastIdentity(string hint = null) {
		throw new NotSupportedException();
	}

	public override ISQLBuilder VariableName(string variableName) {
		throw new NotSupportedException();
	}

	public override ISQLBuilder DeclareVariable(string variableName, Type type) {
		throw new NotSupportedException();
	}

	public override ISQLBuilder AssignVariable(string variableName, object value) {
		throw new NotSupportedException();
	}

	public override ISQLBuilder EmitQueryResultLimit(int limit, int? offset = null) {
		throw new NotSupportedException();
	}

	public override ISQLBuilder CreateBuilder() {
		return new ANSI2003BaseSQLBuilder();
	}
}
