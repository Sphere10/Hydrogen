// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Data;

public abstract class DatabaseManagerBase : IDatabaseManager {

	public event EventHandlerEx<DatabaseCreatedEventArgs> DatabaseCreated;
	public event EventHandlerEx<DatabaseSchemasCreatedEventArgs> DatabaseSchemasCreated;
	public event EventHandlerEx<string> DatabaseDropped;

	public abstract string GenerateConnectionString(string server, string database, string username, string password, int? port);

	public abstract bool DatabaseExists(string connectionString);

	public abstract void CreateApplicationDatabase(string connectionString, DatabaseGenerationDataPolicy dataPolicy, string databaseName);

	public void CreateEmptyDatabase(string connectionString) {
		CreateEmptyDatabaseInternal(connectionString);
		NotifyDatabaseCreated(connectionString);
	}

	public void DropDatabase(string connectionString) {
		DropDatabaseInternal(connectionString);
		NotifyDatabaseDropped(connectionString);
	}

	protected abstract void CreateEmptyDatabaseInternal(string connectionString);

	protected abstract void DropDatabaseInternal(string connectionString);


	protected virtual void OnDatabaseCreated(string connectionString) {
	}

	protected virtual void OnDatabaseSchemasCreated(string connectionString) {
	}


	protected virtual void OnDatabaseDropped(string connectionString) {
	}

	protected void NotifyDatabaseCreated(string connectionString) {
		OnDatabaseCreated(connectionString);
		DatabaseCreated?.Invoke(new DatabaseCreatedEventArgs { ConnectionString = connectionString });
	}

	protected void NotifyDatabaseSchemasCreated(string connectionString) {
		OnDatabaseSchemasCreated(connectionString);
		DatabaseSchemasCreated?.Invoke(new DatabaseSchemasCreatedEventArgs { ConnectionString = connectionString });
	}

	protected void NotifyDatabaseDropped(string connectionString) {
		OnDatabaseDropped(connectionString);
		DatabaseDropped?.Invoke(connectionString);
	}

}
