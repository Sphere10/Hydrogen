// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Data;

public class ColumnValue : IEquatable<ColumnValue> {

	public ColumnValue(string columnName, object @value) {
		ColumnName = columnName;
		Value = @value ?? DBNull.Value;
	}

	public string ColumnName { get; set; }

	public object Value { get; set; }

	public override bool Equals(object obj) {
		return this.Equals(obj as ColumnValue);
	}

	public bool Equals(ColumnValue other) {
		if (other == null)
			return false;

		return (ColumnName == other.ColumnName && Value.Equals(other.Value));

	}

	public override int GetHashCode() {
		return HashCode.Combine(ColumnName.GetHashCode(), Value != null ? Value.GetHashCode() : 0);
	}
}
