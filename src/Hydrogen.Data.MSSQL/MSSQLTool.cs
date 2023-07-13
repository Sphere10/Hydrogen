// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Data.SqlClient;
using Hydrogen;
using Hydrogen.Data;

// ReSharper disable CheckNamespace
namespace Tools;

public static class MSSQL {
	private const string CheckDatabaseExistsQuery = "SELECT CASE WHEN EXISTS(SELECT name FROM sys.databases WHERE name = '{0}') THEN 1 ELSE 0 END";


	public static MSSQLDAC Open(string dataSource, string initialCatalog, string userID = null, string password = null, bool? integratedSecurity = null, string failoverPartner = null, string attachDBFilename = null, bool? persistSecurityInfo = null,
	                            bool? enlist = null, bool? pooling = null, int? minPoolSize = null, int? maxPoolSize = null, bool? multipleActiveResultSets = null, bool? replication = null, TimeSpan? connectTimeout = null, bool? encrypt = null,
	                            bool? trustServerCertificate = null, TimeSpan? loadBalanceTimeout = null, string networkLibrary = null, int? packetSize = null, string typeSystemVersion = null, string applicationName = null,
	                            string currentLanguage = null, string workstationID = null, bool? userInstance = null, string transactionBinding = null, ApplicationIntent? applicationIntent = null, bool? multiSubnetFailover = null,
	                            ILogger logger = null) {
		return new MSSQLDAC(
			CreateConnectionString(dataSource,
				initialCatalog,
				userID,
				password,
				integratedSecurity,
				failoverPartner,
				attachDBFilename,
				persistSecurityInfo,
				enlist,
				pooling,
				minPoolSize,
				maxPoolSize,
				multipleActiveResultSets,
				replication,
				connectTimeout,
				encrypt,
				trustServerCertificate,
				loadBalanceTimeout,
				packetSize,
				typeSystemVersion,
				applicationName,
				currentLanguage,
				workstationID,
				userInstance,
				transactionBinding,
				applicationIntent,
				multiSubnetFailover),
			logger
		);
	}


	public static bool Exists(string server = null, string databaseName = null, string userID = null, string password = null, bool? integratedSecurity = null) {
		if (string.IsNullOrWhiteSpace(server))
			throw new ArgumentNullException("server");

		if (string.IsNullOrWhiteSpace(databaseName))
			throw new ArgumentNullException("databaseName");

		var connString = CreateConnectionString(server, "master", userID, password, integratedSecurity);
		var dac = new MSSQLDAC(connString);
		using (dac.BeginScope()) {
			return dac.ExecuteScalar<bool>(
				CheckDatabaseExistsQuery.FormatWith(databaseName)
			);
		}
	}

	public static bool TestConnectionString(string connectionString) {
		var dac = new MSSQLDAC(connectionString);
		try {
			using (dac.BeginScope()) {
				return true;
			}
		} catch {
			return false;
		}
	}

	public static void CreateDatabase(string server, string databaseName, string username = null, string password = null, bool? useIntegratedSecurity = null, AlreadyExistsPolicy existsPolicy = AlreadyExistsPolicy.Error) {
		if (string.IsNullOrWhiteSpace(server))
			throw new ArgumentNullException("server");

		if (string.IsNullOrWhiteSpace(databaseName))
			throw new ArgumentNullException("databaseName");


		var connString = CreateConnectionString(server, "master", username, password, useIntegratedSecurity);
		var dac = new MSSQLDAC(connString);
		using (dac.BeginScope()) {
			var isAzure = dac.IsAzure();

			var alreadyExists = dac.ExecuteScalar<bool>(CheckDatabaseExistsQuery.FormatWith(databaseName));

			var shouldDrop = false;
			var shouldCreate = false;
			if (alreadyExists) {
				switch (existsPolicy) {
					case AlreadyExistsPolicy.Skip:
						break;
					case AlreadyExistsPolicy.Overwrite:
						shouldDrop = true;
						shouldCreate = true;
						break;
					case AlreadyExistsPolicy.Error:
					default:
						throw new SoftwareException("Database '{0}' already exists on server '{1}'", databaseName, server);
						break;
				}
			} else {
				shouldCreate = true;
			}

			var sqlBuilder = dac.CreateSQLBuilder();
			if (shouldDrop) {
				if (!isAzure) {
					sqlBuilder.Emit("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", databaseName).EndOfStatement(SQLStatementType.DDL);
				}
				sqlBuilder.Emit("DROP DATABASE [{0}]", databaseName).EndOfStatement(SQLStatementType.DDL);
			}
			if (shouldCreate) {
				sqlBuilder
					.Emit("CREATE DATABASE [{0}]", databaseName).EndOfStatement(SQLStatementType.DDL);

				if (!isAzure) {
					sqlBuilder.Emit("ALTER DATABASE [{0}] SET RECOVERY SIMPLE", databaseName).EndOfStatement(SQLStatementType.DDL);
				}
			}
			if (shouldDrop || shouldCreate) {
				sqlBuilder.Statements.ForEach(s => dac.ExecuteNonQuery(s.SQL));
			}
		}
	}

	public static void DropDatabase(string server, string databaseName, string username = null, string password = null, bool? useIntegratedSecurity = null, bool throwIfNotExists = true) {
		if (string.IsNullOrWhiteSpace(server))
			throw new ArgumentNullException("server");

		if (string.IsNullOrWhiteSpace(databaseName))
			throw new ArgumentNullException("databaseName");


		var connString = CreateConnectionString(server, "master", username, password, useIntegratedSecurity);
		var dac = new MSSQLDAC(connString);
		using (dac.BeginScope()) {
			var exists = dac.ExecuteScalar<bool>(
				CheckDatabaseExistsQuery.FormatWith(databaseName)
			);

			if (exists) {
				var sqlBuilder = dac.CreateSQLBuilder();
				if (!dac.IsAzure()) {
					sqlBuilder.Emit("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", databaseName).EndOfStatement(SQLStatementType.DDL);
				}
				sqlBuilder.Emit("DROP DATABASE [{0}]", databaseName).EndOfStatement(SQLStatementType.DDL);
				sqlBuilder.Statements.ForEach(s => dac.ExecuteNonQuery(s.SQL));
			} else if (throwIfNotExists) {
				throw new SoftwareException("Unable to drop database '{0}' as it did not exist", databaseName);
			}

		}
	}

	public static string CreateConnectionString(string server = null, string initialCatalog = null, string userID = null, string password = null, bool? integratedSecurity = null, string failoverPartner = null, string attachDBFilename = null,
	                                            bool? persistSecurityInfo = null, bool? enlist = null, bool? pooling = null, int? minPoolSize = null, int? maxPoolSize = null, bool? multipleActiveResultSets = null, bool? replication = null,
	                                            TimeSpan? connectTimeout = null, bool? encrypt = null, bool? trustServerCertificate = null, TimeSpan? loadBalanceTimeout = null, int? packetSize = null, string typeSystemVersion = null,
	                                            string applicationName = null, string currentLanguage = null, string workstationID = null, bool? userInstance = null, string transactionBinding = null, ApplicationIntent? applicationIntent = null,
	                                            bool? multiSubnetFailover = null, int? port = null) {
		var connectionString = new SqlConnectionStringBuilder();
		if (!String.IsNullOrWhiteSpace(server))
			connectionString.DataSource = server;

		if (port.HasValue)
			connectionString.DataSource += ", " + port.Value.ToString();

		if (!String.IsNullOrWhiteSpace(failoverPartner))
			connectionString.FailoverPartner = failoverPartner;

		if (!String.IsNullOrWhiteSpace(attachDBFilename))
			connectionString.AttachDBFilename = attachDBFilename;

		if (!String.IsNullOrWhiteSpace(initialCatalog))
			connectionString.InitialCatalog = initialCatalog;

		if (integratedSecurity != null) {
			connectionString.IntegratedSecurity = integratedSecurity.Value;
		}

		if (!String.IsNullOrWhiteSpace(userID))
			connectionString.UserID = userID;

		if (!String.IsNullOrWhiteSpace(password))
			connectionString.Password = password;

		if (persistSecurityInfo != null)
			connectionString.PersistSecurityInfo = persistSecurityInfo.Value;

		if (enlist != null)
			connectionString.Enlist = enlist.Value;

		if (pooling != null)
			connectionString.Pooling = pooling.Value;

		if (minPoolSize != null)
			connectionString.MinPoolSize = minPoolSize.Value;

		if (maxPoolSize != null)
			connectionString.MaxPoolSize = maxPoolSize.Value;

		if (multipleActiveResultSets != null)
			connectionString.MultipleActiveResultSets = multipleActiveResultSets.Value;

		if (replication != null)
			connectionString.Replication = replication.Value;

		if (connectTimeout != null)
			connectionString.ConnectTimeout = (int)Math.Round(connectTimeout.Value.TotalSeconds, 0);

		if (encrypt != null)
			connectionString.Encrypt = encrypt.Value;

		if (trustServerCertificate != null)
			connectionString.TrustServerCertificate = trustServerCertificate.Value;

		if (loadBalanceTimeout != null)
			connectionString.LoadBalanceTimeout = (int)Math.Round(loadBalanceTimeout.Value.TotalSeconds, 0);

		if (packetSize != null)
			connectionString.PacketSize = packetSize.Value;

		if (!String.IsNullOrWhiteSpace(typeSystemVersion))
			connectionString.TypeSystemVersion = typeSystemVersion;

		if (!String.IsNullOrWhiteSpace(applicationName))
			connectionString.ApplicationName = applicationName;

		if (!String.IsNullOrWhiteSpace(currentLanguage))
			connectionString.CurrentLanguage = currentLanguage;

		if (!String.IsNullOrWhiteSpace(workstationID))
			connectionString.WorkstationID = workstationID;

		if (userInstance != null)
			connectionString.UserInstance = userInstance.Value;

		if (!String.IsNullOrWhiteSpace(transactionBinding))
			connectionString.TransactionBinding = transactionBinding;

		if (applicationIntent != null)
			connectionString.ApplicationIntent = applicationIntent.Value;

		if (multiSubnetFailover != null)
			connectionString.MultiSubnetFailover = multiSubnetFailover.Value;

		return connectionString.ToString();
	}


	public static string GetDatabaseNameFromConnectionString(string mssqlConnectionString) {
		var connString = new SqlConnectionStringBuilder(mssqlConnectionString);
		return connString.InitialCatalog;
	}

	public static void ClearAllPools() {
		SqlConnection.ClearAllPools();
	}
}
