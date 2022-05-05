//-----------------------------------------------------------------------
// <copyright file="TableSpecification.cs" company="Sphere 10 Software">
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
	public class TableSpecification {
		public string Name { get; set; }
		public TableType Type { get; set; }
		public IEnumerable<ColumnSpecification> Columns { get; set; }
		public PrimaryKeySpecification PrimaryKey { get; set; }
	}
}
