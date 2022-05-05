using Hydrogen;
using Hydrogen.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.Data {

    public interface IDatabaseManager {

        event EventHandlerEx<DatabaseCreatedEventArgs> DatabaseCreated;
        event EventHandlerEx<string> DatabaseDropped;

        string GenerateConnectionString(string server, string database, string username, string password, int? port);
        bool DatabaseExists(string connectionString);
        void DropDatabase(string connectionString);
        void CreateEmptyDatabase(string connectionString);
        void CreateApplicationDatabase(string connectionString, DatabaseGenerationDataPolicy dataPolicy, string databaseName);
    }
}
