//-----------------------------------------------------------------------
// <copyright file="SQLBuilderExtensions.cs" company="Sphere 10 Software">
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

using System.Data;
using System.Linq;

namespace Sphere10.Framework.Data {

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

}
