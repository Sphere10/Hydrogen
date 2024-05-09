// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Data.Tests;

[Ignore("Fails on Github")]
[TestFixture]
public class MSSQLToolTest {

	private const string Server = "localhost";
	private const string Username = "sa";
	private const string Password = "";

	[Test]
	public void Exists_False() {
		ClassicAssert.IsFalse(Tools.MSSQL.Exists(Server, "__!__No_Database_Should_Ever_be_Called_This__!", Username, Password));
	}

	[Test]
	public void Create() {
		var dbName = Guid.NewGuid().ToStrictAlphaString();
		try {
			Tools.MSSQL.CreateDatabase(Server, dbName, Username, Password);
			ClassicAssert.IsTrue(Tools.MSSQL.Exists(Server, dbName, Username, Password));
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
		ClassicAssert.IsTrue(Tools.MSSQL.Exists(Server, dbName, Username, Password));
		Tools.MSSQL.DropDatabase(Server, dbName, Username, Password);
		ClassicAssert.IsFalse(Tools.MSSQL.Exists(Server, dbName, Username, Password));
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
