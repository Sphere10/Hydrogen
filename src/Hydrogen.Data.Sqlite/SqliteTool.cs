// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.IO;
using Hydrogen;
using Hydrogen.Data;
using System.Data.SQLite;

// ReSharper disable CheckNamespace
namespace Tools;

public static class Sqlite {

	public static SqliteDAC Open(string path, string password = null, SqliteJournalMode? journalMode = null, SqliteSyncMode? syncMode = null, ILogger logger = null) {
		return new SqliteDAC(
			CreateConnectionString(path, password, journalMode: journalMode, syncMode: syncMode),
			logger
		);
	}

	public static SqliteDAC Create(string path, string password = null, int? pageSize = null, SqliteJournalMode journalMode = SqliteJournalMode.Default, SqliteSyncMode syncMode = SqliteSyncMode.Normal,
	                               AlreadyExistsPolicy existsPolicy = AlreadyExistsPolicy.Error, ILogger logger = null) {
		var shouldDrop = false;
		var shouldCreate = true;
		if (ExistsByPath(path)) {
			switch (existsPolicy) {
				case AlreadyExistsPolicy.Skip:
					shouldDrop = false;
					shouldCreate = false;
					break;
				case AlreadyExistsPolicy.Overwrite:
					shouldDrop = true;
					shouldCreate = true;
					break;
				case AlreadyExistsPolicy.Error:
					throw new SoftwareException("Unable to create Sqlite database '{0}' as a file by that path already exists", path);

			}
		}
		if (shouldDrop) {
			File.Delete(path);
		}

		if (shouldCreate) {
			var connString = CreateConnectionString(path, password, pageSize, false, journalMode, syncMode);

			// set password, page config since ado.net doesn't do it
			var dac = new SqliteDAC(connString);
			using (var scope = dac.BeginScope(false)) {

				// Set password explicitly since ADO.NET doesnt 
				var builder = dac.CreateSQLBuilder();

				if (pageSize.HasValue)
					builder.Emit("PRAGMA PAGE_SIZE={0}", pageSize.Value).EndOfStatement(SQLStatementType.DDL);

				// VACUUM is necessary on create to ensure file is created (ADO.NET leaves empty stub otherwise)
				builder.Emit("VACUUM").EndOfStatement(SQLStatementType.DDL);
				dac.ExecuteNonQuery(builder.ToString());
			}
		}
		return Open(path, password, journalMode, syncMode, logger);
	}

	public static void Drop(string path, bool throwIfNotExists = true) {
		if (!File.Exists(path)) {
			if (throwIfNotExists) {
				throw new SoftwareException("Unable to drop Sqlite database '{0}' as it did not exist", path);
			}
			return;
		}
		File.Delete(path);
	}
	public static bool ExistsByPath(string path) {
		return File.Exists(path);
	}

	public static bool ExistsByConnectionString(string connectionString) {
		return ExistsByPath(GetFilePathFromConnectionString(connectionString));
	}
	public static string ToConnectionString(string path, string password = null) {
		var sqliteConnectionString = new SQLiteConnectionStringBuilder {
			FailIfMissing = true,
			DataSource = path,
			Password = password
		};
		return sqliteConnectionString.ToString();
	}

	public static string CreateConnectionString(string path, string password = null, int? pageSize = null, bool? failIfMissing = null, SqliteJournalMode? journalMode = null, SqliteSyncMode? syncMode = null) {
		var connectionStringBuilder = new SQLiteConnectionStringBuilder();

		if (!string.IsNullOrWhiteSpace(path))
			connectionStringBuilder.DataSource = path;

		if (!string.IsNullOrEmpty(password))
			connectionStringBuilder.Password = password;

		if (pageSize.HasValue)
			connectionStringBuilder.PageSize = pageSize.Value;

		if (failIfMissing.HasValue)
			connectionStringBuilder.FailIfMissing = failIfMissing.Value;

		if (journalMode.HasValue) {
			connectionStringBuilder.JournalMode = SqliteHelper.Convert(journalMode.Value);
		}

		if (syncMode.HasValue) {
			connectionStringBuilder.SyncMode = SqliteHelper.Convert(syncMode.Value);
		}

		return connectionStringBuilder.ToString();
	}

	public static string GetDatabaseNameFromConnectionString(string sqliteConnectionString) {
		var fileName = GetFilePathFromConnectionString(sqliteConnectionString);
		return Path.GetFileNameWithoutExtension(fileName);
	}

	public static string GetFilePathFromConnectionString(string sqliteConnectionString) {
		var connStringBuilder = new SQLiteConnectionStringBuilder(sqliteConnectionString);
		return connStringBuilder.DataSource;
	}

	public static string GetFilePathFromConnectionString(string connectionString, out string password) {
		var builder = new SQLiteConnectionStringBuilder(connectionString);
		password = builder.Password ?? default;
		return builder.DataSource;
	}

	public static void ClearAllPools() {
		SQLiteConnection.ClearAllPools();
	}
}
