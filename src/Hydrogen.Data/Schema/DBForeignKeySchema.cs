//-----------------------------------------------------------------------
// <copyright file="DBForeignKeySchema.cs" company="Sphere 10 Software">
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

	public class DBForeignKeySchema : DBObject{
		public override string Name { get; set; }
		public DBKeyType KeyType { get; internal set; }
		public string ForeignKeyTable { get; set; }
		public string[] ForeignKeyColumns { get; set; }
		public string ReferenceTable { get; set; }
		public string[] ReferenceColumns { get; set; }
		public string[] ReferenceTablePrimaryKey { get; internal set; }
		public bool IsNullable { get; set; }
		public bool ReferenceIsUniqueConstraint { get; set; }
		public bool CascadesOnUpdate { get; set; }
		public bool CascadesOnDelete { get; set; }
		public int Position { get; set; }
		public override string SQL { get; set; }
	}
}
