// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using Hydrogen.NUnit;

namespace Hydrogen.Data.Tests;

[TestFixture]
public class DACTests : DACTestFixture {

	[Test(Description = "Client pushes an inserted row")]
	[TestCaseSource("DBMS")]
	public void Insert(DBMSType dbmsType) {
		using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
			dac.Insert("BasicTable", new[] { new ColumnValue("ID", 1), new ColumnValue("Text", "Hello") });
		}
	}

}
