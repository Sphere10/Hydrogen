// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Data.Sqlite;

public class SqliteDatabaseManager : DatabaseManagerBase {

	public override string GenerateConnectionString(string server, string database, string username, string password, int? port) {
		Guard.Argument(string.IsNullOrEmpty(server), nameof(server), "Cannot specify server in Sqlite Connection");
		Guard.Argument(string.IsNullOrEmpty(username), nameof(username), "Cannot specify username in Sqlite Connection");
		Guard.Argument(port == null, nameof(port), "Cannot specify port in Sqlite Connection");
		Guard.FileExists(database);
		return Tools.Sqlite.CreateConnectionString(database, password);
	}

	public override bool DatabaseExists(string connectionString)
		=> Tools.Sqlite.ExistsByPath(Tools.Sqlite.GetFilePathFromConnectionString(connectionString));

	public override void CreateApplicationDatabase(string connectionString, DatabaseGenerationDataPolicy dataPolicy, string databaseName) {
		throw new NotSupportedException();
	}

	protected override void DropDatabaseInternal(string connectionString) {
		Tools.Sqlite.Drop(Tools.Sqlite.GetFilePathFromConnectionString(connectionString));
	}

	protected override void CreateEmptyDatabaseInternal(string connectionString) {
		var sqliteSB = new System.Data.SQLite.SQLiteConnectionStringBuilder(connectionString);
		Tools.Sqlite.Create(sqliteSB.DataSource, sqliteSB.Password, 32768, SqliteJournalMode.Default, SqliteSyncMode.Normal, AlreadyExistsPolicy.Error);
	}

}
