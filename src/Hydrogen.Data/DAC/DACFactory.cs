// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Data;

public static class DACFactory {

	public static IDAC CreateDAC(DBMSType dbmsType, string connectionString, ILogger logger = null) {
		switch (dbmsType) {

			case DBMSType.SQLServer:
				return (IDAC)TypeActivator.Activate("Hydrogen.Data.MSSQLDAC", "Hydrogen.Data.MSSQL", connectionString, logger);

			case DBMSType.Sqlite:
				return (IDAC)TypeActivator.Activate("Hydrogen.Data.SqliteDAC", "Hydrogen.Data.Sqlite", connectionString, logger);

			case DBMSType.Firebird:
			case DBMSType.FirebirdFile:
				return (IDAC)TypeActivator.Activate("Hydrogen.Data.FirebirdDAC", "Hydrogen.Data.Firebird", connectionString, logger);
			default:
				throw new NotSupportedException(dbmsType.ToString());
		}
	}
}
