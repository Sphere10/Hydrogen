// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Data;

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
