// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Data;

[Serializable]
public struct SQLStatement {
	public SQLStatementType Type;
	public string SQL;

	public bool Equals(SQLStatement other) {
		return Type == other.Type && string.Equals(SQL, other.SQL);
	}

	public override int GetHashCode() {
		unchecked {
			return ((int)Type * 397) ^ (SQL != null ? SQL.GetHashCode() : 0);
		}
	}
	public override bool Equals(object obj) {
		if (ReferenceEquals(null, obj)) return false;
		return obj is SQLStatement && Equals((SQLStatement)obj);
	}
}
