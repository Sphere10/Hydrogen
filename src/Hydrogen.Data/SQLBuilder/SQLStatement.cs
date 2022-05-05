//-----------------------------------------------------------------------
// <copyright file="SQLStatement.cs" company="Sphere 10 Software">
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
using System.Runtime.Serialization;
using System.Text;

namespace Hydrogen.Data {

    [Serializable]
	public struct SQLStatement  {
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
			return obj is SQLStatement && Equals((SQLStatement) obj);
		}
	}
}
