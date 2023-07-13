// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Data;
using System.Linq;

namespace Hydrogen.Data;

public static class SQLBuilderExtensions {

	public static ISQLBuilder Insert(this ISQLBuilder @this, DataRow dataRow) {
		var primaryKeyCols = dataRow.Table.PrimaryKey;
		var allCols = dataRow.Table.Columns.Cast<DataColumn>();
		return
			@this.Insert(
				dataRow.Table.TableName,
				(dataRow.Table.HasAutoIncrementPrimaryKey() ? allCols.Except(primaryKeyCols) : allCols)
				.Select(c => new ColumnValue(c.ColumnName, dataRow[c])));
	}

	public static ISQLBuilder Update(this ISQLBuilder @this, DataRow dataRow) {

		var primaryKeyCols = dataRow.Table.PrimaryKey;
		var allCols = dataRow.Table.Columns.Cast<DataColumn>();
		return
			@this.Update(
				dataRow.Table.TableName,
				allCols.Except(primaryKeyCols).Select(c => new ColumnValue(c.ColumnName, dataRow[c])),
				matchColumns: primaryKeyCols.Select(c => new ColumnValue(c.ColumnName, dataRow[c])));
	}

}
