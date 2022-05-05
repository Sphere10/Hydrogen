﻿using System;
using System.Data.SqlClient;

namespace Hydrogen.Data {


	public class MSSQLDatabaseManager : DatabaseManagerBase {

		public override string GenerateConnectionString(string server, string database, string username, string password, int? port) {
			return Tools.MSSQL.CreateConnectionString(server: server, initialCatalog: database, userID: username, password: password, port: port);
		}

		public override bool DatabaseExists(string connectionString) {
			var mssqlSB = new SqlConnectionStringBuilder(connectionString);
			return Tools.MSSQL.Exists(
					mssqlSB.DataSource,
					mssqlSB.InitialCatalog,
					userID: mssqlSB.IntegratedSecurity ? null : mssqlSB.UserID,
					password: mssqlSB.IntegratedSecurity ? null : mssqlSB.Password,
					integratedSecurity: mssqlSB.IntegratedSecurity
			);
		}

		public override void CreateApplicationDatabase(string connectionString, DatabaseGenerationDataPolicy dataPolicy, string databaseName) {
			throw new NotSupportedException();
		}

		protected override void DropDatabaseInternal(string connectionString) {
			var mssqlSB = new SqlConnectionStringBuilder(connectionString);
			Tools.MSSQL.DropDatabase(
				mssqlSB.DataSource,
				mssqlSB.InitialCatalog,
				username: mssqlSB.IntegratedSecurity ? null : mssqlSB.UserID,
				password: mssqlSB.IntegratedSecurity ? null : mssqlSB.Password,
				useIntegratedSecurity: mssqlSB.IntegratedSecurity
		);
		}

		protected override void CreateEmptyDatabaseInternal(string connectionString) {
			var mssqlSB = new SqlConnectionStringBuilder(connectionString);
			Tools.MSSQL.CreateDatabase(
				mssqlSB.DataSource,
				mssqlSB.InitialCatalog,
				username: mssqlSB.IntegratedSecurity ? null : mssqlSB.UserID,
				password: mssqlSB.IntegratedSecurity ? null : mssqlSB.Password,
				useIntegratedSecurity: mssqlSB.IntegratedSecurity
			);
		}

	}
}
