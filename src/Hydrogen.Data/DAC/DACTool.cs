// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.Data;

public static class DACTool {

	public static string GeneratePrimaryKeyColumnName(AutoIdentityDAC.PrimaryKeyConvention strategy, string tableName) {
		if (strategy.HasFlag(AutoIdentityDAC.PrimaryKeyConvention.ID))
			return "ID";

		if (strategy.HasFlag(AutoIdentityDAC.PrimaryKeyConvention.Id))
			return "Id";

		if (strategy.HasFlag(AutoIdentityDAC.PrimaryKeyConvention.TableNameID))
			return "{0}ID".FormatWith(tableName);

		if (strategy.HasFlag(AutoIdentityDAC.PrimaryKeyConvention.TableNameId))
			"{0}Id".FormatWith(tableName);


		throw new SoftwareException("Unable to generate primary key column name with given convention.");
	}

	public static bool TryFindPrimaryKeyColumnValue(AutoIdentityDAC.PrimaryKeyConvention convention, string tableName, IEnumerable<ColumnValue> clause, out ColumnValue primaryKeyColumnValue) {
		if (!convention.HasAnyFlags(AutoIdentityDAC.PrimaryKeyConvention.Numeric, AutoIdentityDAC.PrimaryKeyConvention.Guid, AutoIdentityDAC.PrimaryKeyConvention.String))
			throw new ArgumentException("Primary key convention does not specify a datatype");

		if (!convention.HasAnyFlags(AutoIdentityDAC.PrimaryKeyConvention.ID,
			    AutoIdentityDAC.PrimaryKeyConvention.Id,
			    AutoIdentityDAC.PrimaryKeyConvention.TableNameID,
			    AutoIdentityDAC.PrimaryKeyConvention.TableNameId,
			    AutoIdentityDAC.PrimaryKeyConvention.UseDatabaseStructure))
			throw new ArgumentException("Primary key convention does not specify a naming convention");

		var clauseHash = clause.ToDictionary(val => val.ColumnName);
		primaryKeyColumnValue = null;
		if (convention.HasFlag(AutoIdentityDAC.PrimaryKeyConvention.ID) && clauseHash.ContainsKey("ID")) {
			primaryKeyColumnValue = clauseHash["ID"];
		} else if (convention.HasFlag(AutoIdentityDAC.PrimaryKeyConvention.Id) && clauseHash.ContainsKey("Id")) {
			primaryKeyColumnValue = clauseHash["ID"];
		} else if (convention.HasFlag(AutoIdentityDAC.PrimaryKeyConvention.TableNameID) && clauseHash.ContainsKey("{0}ID".FormatWith(tableName))) {
			primaryKeyColumnValue = clauseHash["ID"];
		} else if (convention.HasFlag(AutoIdentityDAC.PrimaryKeyConvention.TableNameId) && clauseHash.ContainsKey("Id".FormatWith(tableName))) {
			primaryKeyColumnValue = clauseHash["ID"];
		}

		if (primaryKeyColumnValue == null)
			return false; // Id not specified in whereClause (do nothing)

		if (convention.HasFlag(AutoIdentityDAC.PrimaryKeyConvention.FirstInWhereClause) && clause.ElementAt(0) != primaryKeyColumnValue)
			return false; // Id was specified but not first in where clause (do nothing)

		if (!convention.HasFlag(AutoIdentityDAC.PrimaryKeyConvention.Numeric) && primaryKeyColumnValue.Value.GetType().IsNumeric())
			return false;

		if (!convention.HasFlag(AutoIdentityDAC.PrimaryKeyConvention.Guid) && primaryKeyColumnValue.Value is Guid)
			return false;

		if (!convention.HasFlag(AutoIdentityDAC.PrimaryKeyConvention.String) && primaryKeyColumnValue.Value is string)
			return false;

		return true;
	}
}
