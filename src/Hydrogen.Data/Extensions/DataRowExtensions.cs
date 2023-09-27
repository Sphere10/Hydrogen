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
using System.Data;

namespace Hydrogen.Data;

// TODO: there is useful code  https://github.com/dotnet/corefx/tree/master/src/System.Data.DataSetExtensions/src/System/Data


public static class DataRowExtensions {

	public static T Get<T>(this DataRow row, string columnName) {
		return row.Get<T>(row.Table.Columns[columnName]);
	}

	public static T Get<T>(this DataRow row, int columnIndex) {
		return row.Get<T>(row.Table.Columns[columnIndex]);
	}

	public static T Get<T>(this DataRow row, DataColumn column) {
		if (column == null)
			throw new ArgumentNullException("column");

		if (column.DataType != null && typeof(T).IsAssignableFrom(column.DataType)) {
			return row[column] != DBNull.Value ? (T)row[column] : default(T);
		}

		return Tools.Parser.Parse<T>(ObjectToString(row[column]));
	}

	public static IEnumerable<T> GetMany<T>(this DataRow row, params string[] columnNames) {
		if (columnNames == null)
			throw new ArgumentNullException("columnNames");

		return row.GetMany<T>(columnNames.AsEnumerable());
	}

	public static IEnumerable<T> GetMany<T>(this DataRow row, IEnumerable<string> columnNames) {
		if (columnNames == null)
			throw new ArgumentNullException("columnNames");

		return columnNames.Select(columnName => Get<T>(row, columnName));
	}

	private static string ObjectToString(object o) {
		if (o == null || o == DBNull.Value) {
			return string.Empty;
		}
		return Tools.Object.ToSQLString(o).Trim('\'');
	}
}
