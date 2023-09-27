// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Data.Common;
using NHibernate.Driver;
using NHibernate.SqlTypes;

namespace Hydrogen.Data.NHibernate;

public class ExtendedSql2008ClientDriver : Sql2008ClientDriver {
	protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType) {
		if (Equals(sqlType, SqlTypeFactory.Byte)) sqlType = SqlTypeFactory.Int16;
		if (Equals(sqlType, SqlTypeFactory.UInt16)) sqlType = SqlTypeFactory.Int32;
		if (Equals(sqlType, SqlTypeFactory.UInt32)) sqlType = SqlTypeFactory.Int64;
		if (Equals(sqlType, SqlTypeFactory.UInt64)) sqlType = SqlTypeFactory.Decimal;
		base.InitializeParameter(dbParam, name, sqlType);
	}
}
