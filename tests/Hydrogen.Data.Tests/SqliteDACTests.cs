// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Data;
using NUnit.Framework;
using System.IO;
using Hydrogen.Data.Tests.Properties;
using NUnit.Framework.Legacy;


namespace Hydrogen.Data.Tests;

[TestFixture]
public class SqliteDACTests {

	public string DBFile;

	[SetUp]
	public void CreateDatabase() {
		DBFile = Path.GetTempFileName();
		File.WriteAllBytes(DBFile, Resource.TestDatabase);
	}

	[Test]
	public void Connection_CreateOpen() {
		var dac = Tools.Sqlite.Open(DBFile);
		using (var conn = dac.CreateOpenConnection()) {
			ClassicAssert.AreEqual(conn.State, ConnectionState.Open);
		}
	}

	[Test]
	public void Connection_CreateClosed() {
		var dac = Tools.Sqlite.Open(DBFile);
		using (var conn = dac.CreateConnection()) {
			ClassicAssert.AreEqual(conn.State, ConnectionState.Closed);
		}
	}


	[Test]
	public void Guid_ReadWriteConsistency() {
		const string tableDDL = @"CREATE TABLE [Table] (
	ID INTEGER PRIMARY KEY AUTOINCREMENT,
	Data1 UNIQUEIDENTIFIER NOT NULL
)";
		var dbFile = Tools.FileSystem.GenerateTempFilename();
		try {
			SystemLog.RegisterLogger(new ConsoleLogger());
			var guid = Guid.NewGuid();
			SystemLog.Info($"Generated Guid: {guid}");
			var dac = Tools.Sqlite.Create(dbFile, logger: SystemLog.Logger);
			dac.ExecuteNonQuery(tableDDL);
			dac.Insert("Table", new[] { new ColumnValue("Data1", guid) });
			var read = dac.Select("Table", new[] { "Data1" }).Single().Get<Guid>(0);
			ClassicAssert.AreEqual(guid, read);
		} finally {
			if (File.Exists(dbFile))
				File.Delete(dbFile);
		}
	}

	[Test]
	public void Guid_SelectConsistency() {
		const string tableDDL = @"CREATE TABLE [Table] (
	ID INTEGER PRIMARY KEY AUTOINCREMENT,
	Data1 UNIQUEIDENTIFIER NOT NULL
)";
		var dbFile = Tools.FileSystem.GenerateTempFilename();
		try {
			SystemLog.RegisterLogger(new ConsoleLogger());
			var guid = Guid.NewGuid();
			SystemLog.Debug($"Generated Guid: {guid}");

			var dac = Tools.Sqlite.Create(dbFile, logger: SystemLog.Logger);
			dac.ExecuteNonQuery(tableDDL);

			dac.Insert("Table", new[] { new ColumnValue("Data1", guid) });

			var read = dac.Select("Table", new[] { "Data1" }, columnMatches: new[] { new ColumnValue("Data1", guid), }).Single().Get<Guid>(0);
			ClassicAssert.AreEqual(guid, read);

		} finally {
			if (File.Exists(dbFile))
				File.Delete(dbFile);
		}

	}


	[Test]
	public void EmptyGuid_SelectConsistency() {
		const string tableDDL = @"CREATE TABLE [Table] (
	ID INTEGER PRIMARY KEY AUTOINCREMENT,
	Data1 UNIQUEIDENTIFIER NOT NULL
)";
		var dbFile = Tools.FileSystem.GenerateTempFilename();
		try {
			SystemLog.RegisterLogger(new ConsoleLogger());
			var emptyGuid = Guid.Empty;
			SystemLog.Debug($"Empty Guid: {emptyGuid}");

			var dac = Tools.Sqlite.Create(dbFile, logger: SystemLog.Logger);
			dac.ExecuteNonQuery(tableDDL);

			dac.Insert("Table", new[] { new ColumnValue("Data1", emptyGuid) });

			var read = dac.Select("Table", new[] { "Data1" }, columnMatches: new[] { new ColumnValue("Data1", emptyGuid), }).Single().Get<Guid>(0);
			ClassicAssert.AreEqual(emptyGuid, read);

		} finally {
			if (File.Exists(dbFile))
				File.Delete(dbFile);
		}

	}


	[TearDown]
	public void DeleteDatabase() {
		if (File.Exists(DBFile))
			File.Delete(DBFile);
	}

}
