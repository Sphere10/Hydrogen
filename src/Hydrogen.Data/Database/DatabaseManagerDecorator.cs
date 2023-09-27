// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Data;

public class DatabaseManagerDecorator : IDatabaseManager {

	public event EventHandlerEx<DatabaseCreatedEventArgs> DatabaseCreated {
		add => InternalDatabaseManager.DatabaseCreated += value;
		remove => InternalDatabaseManager.DatabaseCreated -= value;
	}

	public event EventHandlerEx<DatabaseSchemasCreatedEventArgs> DatabaseSchemasCreated {
		add => InternalDatabaseManager.DatabaseSchemasCreated += value;
		remove => InternalDatabaseManager.DatabaseSchemasCreated -= value;
	}

	public event EventHandlerEx<string> DatabaseDropped {
		add => InternalDatabaseManager.DatabaseDropped += value;
		remove => InternalDatabaseManager.DatabaseDropped -= value;
	}

	protected DatabaseManagerDecorator(IDatabaseManager internalDatabaseManager) {
		Guard.ArgumentNotNull(internalDatabaseManager, nameof(internalDatabaseManager));
		InternalDatabaseManager = internalDatabaseManager;
		InternalDatabaseManager.DatabaseCreated += args => OnDatabaseCreated(args.ConnectionString);
		InternalDatabaseManager.DatabaseSchemasCreated += args => OnDatabaseSchemasCreated(args.ConnectionString);
		InternalDatabaseManager.DatabaseDropped += OnDatabaseDropped;

	}

	protected IDatabaseManager InternalDatabaseManager { get; }

	public virtual void CreateEmptyDatabase(string connectionString)
		=> InternalDatabaseManager.CreateEmptyDatabase(connectionString);

	public virtual void CreateApplicationDatabase(string connectionString, DatabaseGenerationDataPolicy dataPolicy, string databaseName)
		=> InternalDatabaseManager.CreateApplicationDatabase(connectionString, dataPolicy, databaseName);

	public virtual bool DatabaseExists(string connectionString)
		=> InternalDatabaseManager.DatabaseExists(connectionString);

	public virtual void DropDatabase(string connectionString)
		=> InternalDatabaseManager.DropDatabase(connectionString);

	public virtual string GenerateConnectionString(string server, string database, string username, string password, int? port)
		=> InternalDatabaseManager.GenerateConnectionString(server, database, username, password, port);

	protected virtual void OnDatabaseCreated(string connectionString) {
	}

	protected virtual void OnDatabaseSchemasCreated(string connectionString) {
	}

	protected virtual void OnDatabaseDropped(string connectionString) {
	}
}
