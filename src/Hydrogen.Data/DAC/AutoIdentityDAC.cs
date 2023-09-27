// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.Data;

/// <summary>
/// Insert method will always return the primary key value. It can also generate a primary key for you using a variety of strategies.
/// </summary>
public class AutoIdentityDAC : DACDecorator {

	public AutoIdentityDAC(
		IDAC decoratedDAC,
		AutoIdentityPolicy autoIdentityPolicy = AutoIdentityPolicy.UseDBMSAutoIncrement,
		PrimaryKeyConvention primaryKeyConvention = PrimaryKeyConvention.Default)
		: base(decoratedDAC) {
		DefaultAutoIdentityPolicy = autoIdentityPolicy;
		DefaultPrimaryKeyConvention = primaryKeyConvention;
	}

	public AutoIdentityPolicy DefaultAutoIdentityPolicy { get; set; }

	public PrimaryKeyConvention DefaultPrimaryKeyConvention { get; set; }

	public override long Insert(string tableName, IEnumerable<ColumnValue> setValues) {
		// get the policy, strategy and convention which applies to this table
		// TODO: add ability to override these for specific tables, currently just uses default for all tables
		var autoIdentityPolicy = DefaultAutoIdentityPolicy;
		var primaryKeyConvention = DefaultPrimaryKeyConvention;

		// Determine what needs to be done 
		ColumnValue primarykeyColumn;
		bool needToEnableExplicitID;
		bool needToCalculateID;
		bool hasPrimaryKeyInInsert = DACTool.TryFindPrimaryKeyColumnValue(primaryKeyConvention, tableName, setValues, out primarykeyColumn);


		switch (autoIdentityPolicy) {
			case AutoIdentityPolicy.UseDBMSAutoIncrement:
				needToEnableExplicitID = hasPrimaryKeyInInsert;
				needToCalculateID = false;
				break;
			case AutoIdentityPolicy.CalculateAutoIncrement:
			case AutoIdentityPolicy.CalculateAutoDecrement:
				needToCalculateID = !hasPrimaryKeyInInsert;
				needToEnableExplicitID = true;
				break;
			case AutoIdentityPolicy.None:
			default:
				return base.Insert(tableName, setValues); // Pass through to underlying DAC
		}


		// Start building the SQL 
		var sqlBuilder =
			CreateSQLBuilder()
				.DeclareVariable("NewID", typeof(long));


		if (needToCalculateID) {
			if (!hasPrimaryKeyInInsert) {
				// Add the PK to the insert clause 
				primarykeyColumn = new ColumnValue(
					DACTool.GeneratePrimaryKeyColumnName(primaryKeyConvention, tableName),
					SQLBuilderCommand.Variable("NewID")
				);
				setValues = setValues.Concat(primarykeyColumn);
			}

			// Calculate the PK value
			if (!autoIdentityPolicy.IsIn(AutoIdentityPolicy.CalculateAutoIncrement, AutoIdentityPolicy.CalculateAutoDecrement))
				throw new SoftwareException("Internal Error. Unable to calculate ID for policy '{0}'.", autoIdentityPolicy);

			sqlBuilder
				.AssignVariable(
					"NewID",
					SQLBuilderCommand.Expression(
						typeof(long),
						autoIdentityPolicy == AutoIdentityPolicy.CalculateAutoIncrement
							? "(SELECT MAX(T.ID) FROM (SELECT [{0}] as ID FROM [{1}] UNION SELECT 0 as ID ) T) + 1"
							: "(SELECT MIN(T.ID) FROM (SELECT [{0}] as ID FROM [{1}] UNION SELECT 0 as ID ) T) - 1",
						primarykeyColumn.ColumnName,
						tableName)
				);


		} else if (hasPrimaryKeyInInsert) {
			// ID explicitly given
			sqlBuilder.AssignVariable("NewID", primarykeyColumn.Value);
		}

		// Set the PK value to the variable
		if (hasPrimaryKeyInInsert)
			primarykeyColumn.Value = SQLBuilderCommand.Variable("NewID");

		// Enable explicit ID if necessary
		if (needToEnableExplicitID)
			sqlBuilder.DisableAutoIncrementID(tableName);

		//  Insert the record already
		sqlBuilder.Insert(tableName, setValues);

		// Disable explicit ID if we enabled it 
		if (needToEnableExplicitID)
			sqlBuilder.EnableAutoIncrementID(tableName);

		// If DBMS calculated the ID, then assign our variable to the last identity
		if (!needToCalculateID && !hasPrimaryKeyInInsert)
			sqlBuilder.AssignVariable("NewID", SQLBuilderCommand.LastIdentity(tableName));


		// Select the identity variable
		sqlBuilder
			.Emit("SELECT ")
			.VariableName("NewID")
			.EndOfStatement()
			.End();


		// run the query returning the ID
		var generatedID = base.ExecuteScalar(((Object)sqlBuilder).ToString());

		return (long)generatedID;
	}


	public enum AutoIdentityPolicy {
		/// <summary>
		/// Will pass-through the insert to the underlying DAC, who's return value will match their specification.
		/// </summary>
		None,

		/// <summary>
		/// Will use the DBMS's auto-increment feature if an identity is not given, otherwise will enable explicit identity insert on the table.
		/// </summary>
		UseDBMSAutoIncrement,

		/// <summary>
		/// Will set the identity to the largest identity value + 1 (but not 0) if an identity is not given, otherwise will enable explicit identity insert on the table.
		/// </summary>
		CalculateAutoIncrement,

		/// <summary>
		/// Will set the identity to the smallest identity value - 1 (but not 0) if an identity is not given, otherwise will enable explicit identity insert on the table.
		/// </summary>
		CalculateAutoDecrement
	}


	[Flags]
	public enum PrimaryKeyConvention : long {
		ID = 1 << 0,
		Id = 1 << 1,
		TableNameID = 1 << 2,
		TableNameId = 1 << 3,
		FirstInWhereClause = 1 << 4,
		Numeric = 1 << 5,
		Guid = 1 << 6,
		String = 1 << 7,
		UseDatabaseStructure = 1 << 8,
		Default = ID | Id | Numeric | Guid,
	}
}
