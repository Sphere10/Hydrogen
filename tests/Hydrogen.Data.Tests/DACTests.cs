//-----------------------------------------------------------------------
// <copyright file="DACTests.cs" company="Sphere 10 Software">
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

using NUnit.Framework;
using Sphere10.Framework.Data;
using Sphere10.Framework.NUnit;
using Sphere10.Framework.UnitTests.DAC;

namespace Sphere10.Framework.UnitTests {

    [TestFixture]
    public class DACTests : DACTestFixture {

        [Test(Description = "Client pushes an inserted row")]
        [TestCaseSource("DBMS")]
        public void Insert(DBMSType dbmsType) {
            using (var dac = EnterCreateDatabaseScope(dbmsType, TestTables.BasicTable)) {
                dac.Insert("BasicTable", new[] {new ColumnValue("ID", 1),  new ColumnValue( "Text", "Hello") });
            }
        }

    }

}
