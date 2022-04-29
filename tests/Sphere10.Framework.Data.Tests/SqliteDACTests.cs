//-----------------------------------------------------------------------
// <copyright file="SqliteDACTests.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Transactions;
using Sphere10.Framework;
using Sphere10.Framework.Data;
using Sphere10.Framework;

namespace Sphere10.Framework.UnitTests {

    [TestFixture]
    public class SqliteDACTests {

        public string DBFile;

        [SetUp]
        public void CreateDatabase() {
            DBFile = Path.GetTempFileName();
            File.WriteAllBytes(DBFile, Sphere10.Framework.UnitTests.Resource.TestSqliteDatabase);
        }

        [Test]
        public void Connection_CreateOpen() {
            var dac = Tools.Sqlite.Open(DBFile);
            using (var conn = dac.CreateOpenConnection()) {
                Assert.AreEqual(conn.State, ConnectionState.Open);
            } 
        }

        [Test]
        public void Connection_CreateClosed() {
            var dac = Tools.Sqlite.Open(DBFile);
            using (var conn = dac.CreateConnection()) {
                Assert.AreEqual(conn.State, ConnectionState.Closed);
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
                dac.Insert("Table", new[] {new ColumnValue("Data1", guid)});
                var read = dac.Select("Table", new[] {"Data1"}).Single().Get<Guid>(0);
                Assert.AreEqual(guid, read);
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
                Assert.AreEqual(guid, read);

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
                Assert.AreEqual(emptyGuid, read);

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

}
