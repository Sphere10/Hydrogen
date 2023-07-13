// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Data;
using NHibernate.Dialect;

namespace Hydrogen.Data.NHibernate;

public class ExtendedMssqlDialect : MsSql2008Dialect {
	protected override void RegisterNumericTypeMappings() {
		base.RegisterNumericTypeMappings();
		RegisterColumnType(DbType.Byte, "SMALLINT");
		RegisterColumnType(DbType.UInt16, "INT");
		RegisterColumnType(DbType.UInt32, "BIGINT");
		RegisterColumnType(DbType.UInt64, "DECIMAL(28)");
	}
}
