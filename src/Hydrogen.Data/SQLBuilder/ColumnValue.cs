//-----------------------------------------------------------------------
// <copyright file="ColumnValue.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;

namespace Hydrogen.Data {

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
			return Tools.Object.CombineHashCodes(ColumnName.GetHashCode(), Value != null ? Value.GetHashCode() : 0);
		}
	}

}
