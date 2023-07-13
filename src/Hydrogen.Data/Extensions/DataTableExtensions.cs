// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Hydrogen.Data;

public static class DataTableExtensions {

	public static IEnumerable<DataRow> AsEnumerable(this DataTable table) {
		for (int i = 0; i < table.Rows.Count; i++) {
			yield return table.Rows[i];
		}
	}

	//public static DataTable SortDataTable(DataTable table, params string[] columns) {
	//          if (columns.Length == 0) {
	//              return table;
	//          }

	//          var firstColumn = columns.First();

	//          var result = table.AsEnumerable().OrderBy(r => r[firstColumn]);

	//          foreach (var columnName in columns.Skip(1)) {
	//              result = result.ThenBy(r => r[columnName]);
	//          }

	//	// HS 2019-02-19 - changed needs testing (TODO)
	//         return result.AsDataView().ToTable();
	//}


	/// <summary>
	/// Adds an object entity into the DataTable as a row. 
	/// </summary>
	/// <typeparam name="T">Type of object.</typeparam>
	/// <param name="dataTable">The data table.</param>
	/// <param name="entity">The entity.</param>
	/// <remarks>Entity must have properties with the same type and name as the columns in this DataTable.</remarks>
	public static void AddEntity<T>(this DataTable dataTable, T entity) {
		DataRow dataRow = dataTable.NewRow();
		foreach (DataColumn col in dataTable.Columns) {
			dataRow[col.ColumnName] = Tools.Reflection.GetPropertyValue(entity, col.ColumnName) ?? DBNull.Value;
		}
		dataTable.Rows.Add(dataRow);
	}

	public static bool HasAutoIncrementPrimaryKey(this DataTable dataTable) {
		return dataTable.PrimaryKey.Length == 1 && dataTable.PrimaryKey[0].AutoIncrement;
	}

	public static DataColumn MakeColumnPrimaryKey(this DataTable dataTable, string columnName) {
		return dataTable.MakeColumnPrimaryKey(dataTable.Columns[columnName]);
	}

	public static DataColumn MakeColumnPrimaryKey(this DataTable dataTable, DataColumn col) {
		dataTable.PrimaryKey = Tools.Array.Concat<DataColumn>(dataTable.PrimaryKey, new[] { col });
		return col;
	}

	public static void SetDateTimeMode(this DataTable dataTable, DataSetDateTime mode) {
		foreach (DataColumn column in dataTable.Columns)
			if (column.DataType == typeof(DateTime))
				column.DateTimeMode = mode;
	}

	public static IEnumerable<DataColumn> GetForeignKeyColumns(this DataTable dataTable) {
		return (
				from c in dataTable.Constraints.Cast<Constraint>()
				where c is ForeignKeyConstraint
				select ((ForeignKeyConstraint)c).Columns
			)
			.Cast<IEnumerable<DataColumn>>()
			.Unpartition()
			.Distinct();
	}

	public static DataRow Single(this DataTable dataTable) {
		Guard.ArgumentNotNull(dataTable, "dataTable");
		if (dataTable.Rows.Count != 1)
			throw new SoftwareException("DataTable has {0} rows", dataTable.Rows.Count);

		return dataTable.Rows[0];
	}

	public static DataRow SingleOrDefault(this DataTable dataTable) {
		Guard.ArgumentNotNull(dataTable, "dataTable");
		if (dataTable.Rows.Count > 1)
			throw new SoftwareException("DataTable has more than 1 row");

		if (dataTable.Rows.Count == 0)
			return null;

		return dataTable.Rows[0];
	}

	public static DataTable ToDataTable<T>(this IEnumerable<T> sequence, Func<T, IEnumerable<DataTableCellInfo>> rowGenerator) {
		var table = new DataTable();
		var dataRows = new List<IEnumerable<DataTableCellInfo>>();
		sequence.ForEach(datum => dataRows.Add(rowGenerator(datum)));

		if (dataRows.Count > 0) {
			// create columns
			dataRows[0].ForEach(r => {
				table.Columns.Add(r.ColumnName);
				var col = table.Columns[r.ColumnName];
				if (!r.ColumnVisible) {
					col.ColumnMapping = MappingType.Hidden;
				}
			});
		}

		dataRows.ForEach(dataRow => {
			var row = table.NewRow();
			dataRow.ForEach(c => row[c.ColumnName] = c.CellValue);
			table.Rows.Add(row);
		});
		return table;
	}

	/// <summary>
	/// Converts the passed in data table to a CSV-style string.
	/// </summary>
	/// <param name="table">Table to convert</param>
	/// <param name="delimiter">Delimiter used to separate fields</param>
	/// <param name="includeHeader">true - include headers<br/>
	/// false - do not include header column</param>
	/// <returns>Resulting CSV-style string</returns>
	public static string ToCSV(this DataTable table, string delimiter = ",", bool includeHeader = true) {
		var result = new StringBuilder();

		if (includeHeader) {
			table.Columns.Cast<DataColumn>().ForEach(column => { result.AppendFormat("{0}{1}", column.ColumnName.EscapeCSV(delimiter), delimiter); });
			result.Remove(--result.Length, 0);
			result.Append(Environment.NewLine);
		}

		table.Rows.Cast<DataRow>().ForEach(row => {
			row.ItemArray.ForEach(item => {
				if (!(item is System.DBNull)) {
					result.Append(item.ToString().EscapeCSV());
				}
				result.Append(delimiter);
			});
			result.Remove(--result.Length, 0);
			result.Append(Environment.NewLine);
		});

		return result.ToString();
	}
	public static string ToPrintableString() {
		throw new NotImplementedException();
	}

}
