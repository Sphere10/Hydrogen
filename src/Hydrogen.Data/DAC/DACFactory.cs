//-----------------------------------------------------------------------
// <copyright file="DACFactory.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Data {

    public static class DACFactory {

        public static IDAC CreateDAC(DBMSType dbmsType, string connectionString, ILogger logger = null) {
            switch (dbmsType) {

                case DBMSType.SQLServer:
					return (IDAC)TypeActivator.Create("Sphere10.Framework.Data.MSSQLDAC", "Sphere10.Framework.Data.MSSQL", connectionString, logger);

				case DBMSType.Sqlite:
                    return (IDAC) TypeActivator.Create("Sphere10.Framework.Data.SqliteDAC", "Sphere10.Framework.Data.Sqlite", connectionString, logger);

                case DBMSType.Firebird:
                case DBMSType.FirebirdFile:
                    return (IDAC) TypeActivator.Create("Sphere10.Framework.Data.FirebirdDAC", "Sphere10.Framework.Data.Firebird", connectionString, logger);
                default:
                    throw new NotSupportedException(dbmsType.ToString());
            }
        }
    }
}
