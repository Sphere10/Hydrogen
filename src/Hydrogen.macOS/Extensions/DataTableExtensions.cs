//-----------------------------------------------------------------------
// <copyright file="DataTableExtensions.cs" company="Sphere 10 Software">
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
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Specialized;
using System.Data;
using Hydrogen;

namespace BW.Cranewatch.Data {
	public static class DataTableExtensions {

		/// <summary>
		/// Converts the passed in data table to a CSV-style string.
		/// </summary>
		/// <param name="table">Table to convert</param>
		/// <param name="delimiter">Delimiter used to separate fields</param>
		/// <param name="includeHeader">true - include headers<br/>
		/// false - do not include header column</param>
		/// <returns>Resulting CSV-style string</returns>
		public static string ToCSV(this DataTable table, string delimiter = "," , bool includeHeader = true) {
			StringBuilder result = new StringBuilder();

			if (includeHeader) {
				table.Columns.Cast<DataColumn>().ForEach( column => {
					result.AppendFormat("{0}{1}", column.ColumnName, delimiter);
				});
				result.Remove(--result.Length, 0);
				result.Append(Environment.NewLine);
			}

			table.Rows.Cast<DataRow>().ForEach( row => {
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

	}
}
