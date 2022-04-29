namespace Sphere10.Framework.Data {
    public class DatabaseManagerDecorator : IDatabaseManager {

        public event EventHandlerEx<DatabaseCreatedEventArgs> DatabaseCreated { add => InternalDatabaseManager.DatabaseCreated += value; remove => InternalDatabaseManager.DatabaseCreated -= value; }
        public event EventHandlerEx<string> DatabaseDropped { add => InternalDatabaseManager.DatabaseDropped += value; remove => InternalDatabaseManager.DatabaseDropped -= value; }

        protected DatabaseManagerDecorator(IDatabaseManager internalDatabaseManager) {
            Guard.ArgumentNotNull(internalDatabaseManager, nameof(internalDatabaseManager));
            InternalDatabaseManager = internalDatabaseManager;
            InternalDatabaseManager.DatabaseCreated += args => OnDatabaseCreated(args.ConnectionString, args.CreatedEmptyDatabase);
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

        protected virtual void OnDatabaseCreated(string connectionString, bool createdEmptyDatabase) {
        }

        protected virtual void OnDatabaseDropped(string connectionString) {
        }

    }
}

