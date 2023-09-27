// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Hydrogen.Data;

public static class DataReaderExtensions {

	public static DataTable ToDataTable(this IDataReader reader, string tableName = "Table") {
		if (reader.FieldCount == 0)
			return new DataTable();

		var schemaTable = reader.GetSchemaTable();
		var dataTable = new DataTable(tableName);
		for (var i = 0; i < schemaTable.Rows.Count; i++) {
			var dataRow = schemaTable.Rows[i];
			var columnName = dataRow["ColumnName"].ToString();
			var columnType = reader.GetFieldType(i);
			var column = new DataColumn(columnName, columnType);
			dataTable.Columns.Add(column);
		}

		while (reader.Read()) {
			var dataRow = dataTable.NewRow();
			for (var i = 0; i < reader.FieldCount; i++)
				dataRow[i] = reader.GetValue(i);

			dataTable.Rows.Add(dataRow);
			dataRow.AcceptChanges();
		}

		return dataTable;
	}

	public static DataTable[] ToDataTables(this IDataReader reader, string tableName = "Table") {
		var results = new List<DataTable>();
		do {
			results.Add(reader.ToDataTable());
		} while (reader.NextResult());
		return results.Where(dt => dt.Columns.Count > 0).ToArray();
	}


}
