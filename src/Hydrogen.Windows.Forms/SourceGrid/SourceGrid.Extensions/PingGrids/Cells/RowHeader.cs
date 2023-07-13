// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid.Extensions.PingGrids.Cells;

/// <summary>
/// A cell used as left row selector. Usually used in the DataCell property of a DataGridColumn. If FixedColumns is grater than 0 and the columns are automatically created then the first column is created of this type.
/// </summary>
public class RowHeader : SourceGrid.Cells.Virtual.RowHeader {
	public RowHeader() {
		Model.AddModel(new DataGridRowHeaderModel());
	}
}
