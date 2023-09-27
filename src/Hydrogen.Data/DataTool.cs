// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Hydrogen;
using System.Reflection;
using Hydrogen.Data;
using Hydrogen.Data.Csv;

// ReSharper disable CheckNamespace
namespace Tools;

public static partial class Data {

	public static readonly System.DateTime SQLDateTimeMinValue = new System.DateTime(1753, 1, 1);
	public static readonly System.DateTime SQLDateTimeMaxValue = new System.DateTime(9999, 12, 31);

	public static DataTable ReadCsv(string filename, bool hasHeaders) {
		using (var streamReader = new StreamReader(filename))
			return ReadCsv(streamReader, hasHeaders);
	}

	public static DataTable ReadCsv(StreamReader stream, bool hasHeaders) {
		using (var csvReader = new CsvReader(stream, hasHeaders)) {
			return csvReader.ToDataTable();
		}
	}


	public static DataTable CreateDataTable(IEnumerable<DataTableCellInfo> colSpecs) {
		var table = new DataTable();
		// create columns
		colSpecs.ForEach(c => {
			table.Columns.Add(c.ColumnName);
			var col = table.Columns[c.ColumnName];
			if (!c.ColumnVisible) {
				col.ColumnMapping = MappingType.Hidden;
			}
		});
		return table;
	}


	/// <summary>
	/// Creates a DataTable with columns matching the properties of the given entity.
	/// </summary>
	/// <typeparam name="T">The LinqToSQL type</typeparam>
	/// <returns>A suitably spec'd empty DataTable.</returns>
	public static DataTable CreateDataTableForType<T>() {
		var dataTable = new DataTable();
		foreach (PropertyInfo pi in typeof(T).GetProperties()) {
			Type fieldType = pi.PropertyType;
			if (fieldType.IsNullable()) {
				fieldType = Nullable.GetUnderlyingType(fieldType);
			}
			dataTable.Columns.Add(pi.Name, fieldType);
		}
		return dataTable;
	}
}
