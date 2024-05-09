// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

#define SQLITE_TO_SQLITE
/*#define SQLITE_TO_MSSQL
#define MSSQL_TO_SQLITE
#define MSSQL_TO_MSSQL*/

#if __MOBILE__
#if SQLITE_TO_MSSQL
#undef SQLITE_TO_MSSQL
#endif
#if MSSQL_TO_SQLITE
#undef MSSQL_TO_SQLITE
#endif
#if MSSQL_TO_MSSQL
#undef MSSQL_TO_MSSQL
#endif
#endif

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;
using Hydrogen.Data;
using Tools;
using Object = Tools.Object;
using NUnit.Framework.Legacy;

namespace Hydrogen.NUnit {
	public abstract class DACTestFixture {
		private const string MSSQLServer_Default = "localhost";
		private const string MSSQLUser_Default = "sa";
		private const string MSSQLPassword_Default = "";
		public static object[] DBMS = { DBMSType.Sqlite /*, DBMSType.SQLServer*/ };

		public DACTestFixture() : this(MSSQLServer_Default, MSSQLUser_Default, MSSQLPassword_Default) {
		}

		public DACTestFixture(string mssqlServer, string mssqlUser, string mssqlPassword) {
			MSSQLServer = mssqlServer;
			MSSQLUser = mssqlUser;
			MSSQLPassword = mssqlPassword;
		}

		public string MSSQLServer { get; private set; }
		public string MSSQLUser { get; private set; }
		public string MSSQLPassword { get; private set; }

		protected virtual UnitTestDAC EnterCreateDatabaseScope(DBMSType dbmsType, params TableSpecification[] tables) {
			var result = EnterCreateEmptyDatabaseScope(dbmsType);
			foreach (var table in tables)
				result.CreateTable(table);
			return result;
		}

		protected virtual UnitTestDAC EnterCreateEmptyDatabaseScope(DBMSType dbmsType) {
			var dac = CreateEmptyDatabase(dbmsType);
			return new UnitTestDAC(() => DropDatabase(dac), dac);
		}

		protected virtual IDAC DuplicateDAC(IDAC sourceDAC) {
			switch (sourceDAC.DBMSType) {
				case DBMSType.Sqlite:
					return new SqliteDAC(sourceDAC.ConnectionString, sourceDAC.Log);
					break;
				case DBMSType.SQLServer:
					return new MSSQLDAC(sourceDAC.ConnectionString, sourceDAC.Log);
					break;
				default:
					throw new NotSupportedException(sourceDAC.DBMSType.ToString());
			}
		}

		protected virtual IDAC CreateEmptyDatabase(DBMSType dbmsType, int? sqlitePageSizeHint = null) {

			switch (dbmsType) {
				case DBMSType.Sqlite:
					return Tools.Sqlite.Create(Tools.FileSystem.GenerateTempFilename(), pageSize: sqlitePageSizeHint);
				case DBMSType.SQLServer:
					var dbName = Guid.NewGuid().ToStrictAlphaString().ToUpper();
					MSSQL.CreateDatabase(MSSQLServer, dbName, MSSQLUser, MSSQLPassword);
					return MSSQL.Open(MSSQLServer, dbName, MSSQLUser, MSSQLPassword);
				default:
					throw new NotImplementedException("DBMSTypes {0}".FormatWith(dbmsType));
			}

		}

		protected virtual void DropDatabase(IDAC dac) {
			switch (dac.DBMSType) {
				case DBMSType.Sqlite:
					Tools.Sqlite.Drop(Tools.Sqlite.GetFilePathFromConnectionString(dac.ConnectionString));
					break;
#if !__MOBILE__
				case DBMSType.SQLServer:
					MSSQL.DropDatabase(MSSQLServer, MSSQL.GetDatabaseNameFromConnectionString(dac.ConnectionString), MSSQLUser, MSSQLPassword);
					break;
#endif
				default:
					throw new NotImplementedException("DBMSTypes {0}".FormatWith(dac.DBMSType));
			}
		}

		protected virtual void AssertSameTableRowCount(IDAC source, IDAC dest, string tableName) {
			var sourceCount = source.Count(tableName);
			var destCount = dest.Count(tableName);
			ClassicAssert.AreEqual(sourceCount, destCount);
		}

		// ReSharper disable once InconsistentNaming
		protected virtual void AssertSameTableDataIncludingPK(IDAC source, IDAC dest, string tableName) {
			AssertSameTableDataInternal(source, dest, tableName, true);
		}

		// ReSharper disable once InconsistentNaming
		protected virtual void AssertSameTableDataExcludingPK(IDAC source, IDAC dest, string tableName) {
			AssertSameTableDataInternal(source, dest, tableName, false);
		}

		protected virtual void PrintTableData(IDAC dac, string header, string tableName) {
			Console.WriteLine(header);
			foreach (var row in dac.Select(tableName).Rows.Cast<DataRow>()) {
				Console.WriteLine("\t{0}".FormatWith(row.ItemArray.ToDelimittedString(",\t", "NULL")));
			}
			Console.WriteLine();
		}

		protected virtual void AssertSingleRow(IDAC dac, string tableName, params object[] expectedRows) {
			AssertManyRowsInOrder(dac, tableName, expectedRows);
		}

		protected virtual void AssertManyRows(IDAC dac, string tableName, params object[][] expectedRows) {
			AssertRowsInternal(dac, tableName, expectedRows, true);
		}

		protected virtual void AssertManyRowsInOrder(IDAC dac, string tableName, params object[][] expectedRows) {
			AssertRowsInternal(dac, tableName, expectedRows, false);
		}

		private void AssertRowsInternal(IDAC dac, string tableName, IEnumerable<IEnumerable<object>> expectedRows, bool anyOrder) {
			var tableRows = dac.Select(tableName).Rows.Cast<DataRow>().Select(r => r.ItemArray as IEnumerable<object>);
			if (anyOrder) {
				expectedRows = expectedRows.OrderByAll();
				tableRows = tableRows.OrderByAll();
			}
			AssertEx.AssertSame2DArrays(expectedRows, tableRows, tableName);
		}

		private void AssertSameTableDataInternal(IDAC source, IDAC dest, string tableName, bool primaryKeyMustMatch) {
			var sourceTableSchema = source.GetSchemaCached()[tableName];
			var destTableSchema = dest.GetSchemaCached()[tableName];

			ClassicAssert.AreNotEqual(DBKeyType.None, sourceTableSchema.PrimaryKey.KeyType, "Tables without primary keys are not supported");
			ClassicAssert.AreEqual(sourceTableSchema.PrimaryKey.KeyType, destTableSchema.PrimaryKey.KeyType, "Table primary key types do not match");

			var sourceData = source.Select(tableName).Rows.Cast<DataRow>().Select(r => r.ItemArray.Select(Object.SanitizeObject).ToArray()).ToArray();
			var destData = dest.Select(tableName).Rows.Cast<DataRow>().Select(r => r.ItemArray.Select(Object.SanitizeObject).ToArray()).ToArray();

			var primaryKeysNotInDest = new IEnumerable<object>[0];
			var primaryKeysNotInSource = new IEnumerable<object>[0];

			// Validate data by primary keys
			if (primaryKeyMustMatch) {
				// Gather data into primary key look up 
				var sourceDataDict = sourceTableSchema.ConvertDataToMultiKeyDictionary(sourceData);
				var destDataDict = destTableSchema.ConvertDataToMultiKeyDictionary(destData);

				// Validate that primary keys map exactly
				primaryKeysNotInDest = sourceDataDict.Keys.Except(destDataDict.Keys, new EnumerableEqualityComparer<object>()).ToArray();
				primaryKeysNotInSource = destDataDict.Keys.Except(sourceDataDict.Keys, new EnumerableEqualityComparer<object>()).ToArray();

				// Print data missing from dest (by PK)
				if (primaryKeysNotInDest.Any()) {
					Console.WriteLine("Primary key missing from dest:");
					foreach (var key in primaryKeysNotInDest.Select(k => k.ToArray())) {
						Console.WriteLine("\tPK:({0}) Data:{1}".FormatWith(key.ToDelimittedString(", "), sourceDataDict[key].ToDelimittedString(", ")));
					}
				}
				Console.WriteLine();

				// Print data missing from source (by PK)
				if (primaryKeysNotInSource.Any()) {
					Console.WriteLine("Primary key missing from source:");
					foreach (var key in primaryKeysNotInSource.Select(k => k.ToArray())) {
						Console.WriteLine("\tPK:({0}) Data:{1}".FormatWith(key.ToDelimittedString(", "), sourceDataDict[key].ToDelimittedString(", ")));
					}
				}
				Console.WriteLine();
			}

			var primaryKeyColCount = sourceTableSchema.PrimaryKeyColumns.Length;
			var sourceDataToCompare = primaryKeyMustMatch ? sourceData : sourceData.Select(rowArr => rowArr.Skip(primaryKeyColCount).ToArray()).ToArray();
			var destDataToCompare = primaryKeyMustMatch ? destData : destData.Select(rowArr => rowArr.Skip(primaryKeyColCount).ToArray()).ToArray();

			// print missing data
			var rowsNotInDestByValue = sourceDataToCompare.Except(destDataToCompare, new EnumerableEqualityComparer<object>()).ToArray();
			var rowsNotInSourceByValue = destDataToCompare.Except(sourceDataToCompare, new EnumerableEqualityComparer<object>()).ToArray();

			// Print data missing from dest 
			if (rowsNotInDestByValue.Any()) {
				Console.WriteLine(Tools.NUnit.Convert2DArrayToString("Data missing from Dest", rowsNotInDestByValue));
			}
			Console.WriteLine();

			// Print data missing from source
			if (rowsNotInSourceByValue.Any()) {
				Console.WriteLine(Tools.NUnit.Convert2DArrayToString("Data missing from Source", rowsNotInSourceByValue));
			}
			Console.WriteLine();

			// Assert primary keys are present in both
			if (primaryKeyMustMatch) {
				Tools.NUnit.IsEmpty(primaryKeysNotInDest, "Dest missing primary keys");
				Tools.NUnit.IsEmpty(primaryKeysNotInSource, "Source missing primary keys");
			}

			Tools.NUnit.IsEmpty(rowsNotInDestByValue, "Dest missing data");
			Tools.NUnit.IsEmpty(rowsNotInSourceByValue, "Source missing data");
		}


	}
}
