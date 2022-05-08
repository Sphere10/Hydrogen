//-----------------------------------------------------------------------
// <copyright file="CellSpec.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Data {

	public class DataTableCellInfo {

		public DataTableCellInfo(string columnName, string cellValue, bool columnVisible = true) {
			ColumnName = columnName;
			CellValue = cellValue;
			ColumnVisible = columnVisible;
		}

		public string ColumnName { get; set; }

		public bool ColumnVisible { get; set; }

		public string CellValue { get; set; }
	}

}

