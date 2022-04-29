//-----------------------------------------------------------------------
// <copyright file="DBPrimaryKeySchema.cs" company="Sphere 10 Software">
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

	public class DBPrimaryKeySchema : DBObject {
		public DBPrimaryKeySchema() {
			ColumnNames = new string[0];
		}

		public override string Name { get; set; }
		public DBKeyType KeyType { get; internal set; }
		public string[] ColumnNames { get; set; }
		public string Sequence { get; set; }
		public override string SQL { get; set; }
		public bool ColumnsAlsoForeignKeys { get; internal set; }
	}


}
