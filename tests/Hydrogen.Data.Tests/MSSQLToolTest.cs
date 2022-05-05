//-----------------------------------------------------------------------
// <copyright file="MSSQLToolTest.cs" company="Sphere 10 Software">
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
using Hydrogen;
using Hydrogen.Data;

namespace Hydrogen.Data.Tests {

    [Ignore("Fails on Github")]
    [TestFixture]
    public class MSSQLToolTest {

        private const string Server = "localhost";
        private const string Username = "sa";
        private const string Password = "";

        [Test]
        public void Exists_False() {
            Assert.IsFalse(Tools.MSSQL.Exists(Server, "__!__No_Database_Should_Ever_be_Called_This__!", Username, Password));
        }

        [Test]
        public void Create() {
            var dbName = Guid.NewGuid().ToStrictAlphaString();
            try {
                Tools.MSSQL.CreateDatabase(Server, dbName, Username, Password);
                Assert.IsTrue(Tools.MSSQL.Exists(Server, dbName, Username, Password));
            } finally {
                Tools.MSSQL.DropDatabase(Server, dbName, Username, Password);
            }
        }

        [Test]
        public void Create_SkipIfExisting() {
            var dbName = Guid.NewGuid().ToStrictAlphaString();
            try {
                Tools.MSSQL.CreateDatabase(Server, dbName, Username, Password);
                Assert.DoesNotThrow(() => Tools.MSSQL.CreateDatabase(Server, dbName, Username, Password, existsPolicy: AlreadyExistsPolicy.Skip));
            } finally {
                Tools.MSSQL.DropDatabase(Server, dbName, Username, Password);
            }
        }


        [Test]
        public void Create_Overwrite() {
            var dbName = Guid.NewGuid().ToStrictAlphaString();
            try {
                Tools.MSSQL.CreateDatabase(Server, dbName, Username, Password);
                Assert.DoesNotThrow(() => Tools.MSSQL.CreateDatabase(Server, dbName, Username, Password, existsPolicy: AlreadyExistsPolicy.Overwrite));
            } finally {
                Tools.MSSQL.DropDatabase(Server, dbName, Username, Password);
            }
        }

        [Test]
        public void Create_DuplicateError() {
            var dbName = Guid.NewGuid().ToStrictAlphaString();
            try {
                Tools.MSSQL.CreateDatabase(Server, dbName, Username, Password);
                Assert.Catch<Exception>(() => Tools.MSSQL.CreateDatabase(Server, dbName, Username, Password));
            } finally {
                Tools.MSSQL.DropDatabase(Server, dbName, Username, Password);
            }
        }

        [Test]
        public void Drop() {
            var dbName = Guid.NewGuid().ToStrictAlphaString();
            Tools.MSSQL.CreateDatabase(Server, dbName, Username, Password);
            Assert.IsTrue(Tools.MSSQL.Exists(Server, dbName, Username, Password));
            Tools.MSSQL.DropDatabase(Server, dbName, Username, Password);
            Assert.IsFalse(Tools.MSSQL.Exists(Server, dbName, Username, Password));
        }

        [Test]
        public void Drop_Throws() {
            Assert.Catch<Exception>(() => Tools.MSSQL.DropDatabase(Server, "__should_not_exist!!@@!!", Username, Password));
        }

        [Test]
        public void Drop_NoThrows() {
            Assert.DoesNotThrow(() => Tools.MSSQL.DropDatabase(Server, "__should_not_exist!!@@!!", Username, Password, throwIfNotExists: false));
        }

    }

}
