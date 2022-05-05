using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hydrogen;
using Hydrogen.Data;

namespace Hydrogen.Data {

	public abstract class DatabaseManagerBase : IDatabaseManager {

        public event EventHandlerEx<DatabaseCreatedEventArgs> DatabaseCreated;
        public event EventHandlerEx<string> DatabaseDropped;

        public abstract string GenerateConnectionString(string server, string database, string username, string password, int? port);
        public abstract bool DatabaseExists(string connectionString);
        public abstract void CreateApplicationDatabase(string connectionString, DatabaseGenerationDataPolicy dataPolicy, string databaseName);

        public void CreateEmptyDatabase(string connectionString) {
            CreateEmptyDatabaseInternal(connectionString);
            NotifyDatabaseCreated(connectionString, true);
        }

        public void DropDatabase(string connectionString) {
            DropDatabaseInternal(connectionString);
            NotifyDatabaseDropped(connectionString);
        }
     
        protected abstract void CreateEmptyDatabaseInternal(string connectionString);

        protected abstract void DropDatabaseInternal(string connectionString);


        protected virtual void OnDatabaseCreated(string connectionString, bool createdEmptyDatabase) {
        }

        protected virtual void OnDatabaseDropped(string connectionString) {
        }

        protected void NotifyDatabaseCreated(string connectionString, bool createdEmptyDatabase) {
            OnDatabaseCreated(connectionString, createdEmptyDatabase);
            DatabaseCreated?.Invoke(new DatabaseCreatedEventArgs { ConnectionString = connectionString, CreatedEmptyDatabase = createdEmptyDatabase });
        }

        protected void NotifyDatabaseDropped(string connectionString) {
            OnDatabaseDropped(connectionString);
            DatabaseDropped?.Invoke(connectionString);
        }

    }
}
