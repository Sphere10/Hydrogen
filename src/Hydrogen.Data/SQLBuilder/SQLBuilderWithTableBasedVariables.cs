// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Hydrogen.Data;

public class SQLBuilderWithTableBasedVariables : SQLBuilderDecorator {
	private IDictionary<string, Tuple<int, VariableStorageClass>> _variableDeclarations;
	private IDictionary<VariableStorageClass, Tuple<int, string>> _columnLookup;

	public SQLBuilderWithTableBasedVariables(ISQLBuilder internalBuilder, string variableTableName = null, bool alreadyCreated = false) : base(internalBuilder) {
		VariableTableName = variableTableName ?? "VAR" + Tools.Text.ToBase32(Guid.NewGuid().ToByteArray().ToASCIIString()); // "VAR7BHBHPURK64WYPK3PA2TT6K3U2";
		VariableTableHasBeenCreated = alreadyCreated;
		DontDropOnEnd = alreadyCreated;
		_variableDeclarations = new Dictionary<string, Tuple<int, VariableStorageClass>>();
		_columnLookup =
			Enum
				.GetValues(typeof(VariableStorageClass))
				.Cast<VariableStorageClass>()
				.OrderBy(x => x)
				.WithDescriptions()
				.Select(val =>
					new {
						StorageClass = val.Item,
						Order = val.Index,
						ColumnName = val.Item.GetAttributes<DescriptionAttribute>().First().Description
					}
				)
				.ToDictionary(x => x.StorageClass, x => Tuple.Create(x.Order, x.ColumnName));

	}

	public string VariableTableName { get; internal set; }

	public bool VariableTableHasBeenCreated { get; set; }

	protected bool DontDropOnEnd { get; set; }

	public override ISQLBuilder End() {
		if (VariableTableHasBeenCreated && !DontDropOnEnd)
			Emit("DROP TABLE ").TableName(VariableTableName, TableType.Temporary).EndOfStatement();
		else if (VariableTableHasBeenCreated)
			Emit("DELETE FROM ").TableName(VariableTableName, TableType.Temporary).EndOfStatement();
		return this;
	}

	public override ISQLBuilder CreateBuilder() {
		var builder = new SQLBuilderWithTableBasedVariables(DecoratedBuilder.CreateBuilder());
		builder._variableDeclarations = this._variableDeclarations;
		builder._columnLookup = this._columnLookup;
		builder.VariableTableName = this.VariableTableName;
		builder.VariableTableHasBeenCreated = this.VariableTableHasBeenCreated;
		return builder;
	}

	public override ISQLBuilder DeclareVariable(string variableName, Type type) {
		if (_variableDeclarations.ContainsKey(variableName))
			throw new SoftwareException("A variable with name '{0}' has already been declared", variableName);

		if (!VariableTableHasBeenCreated) {
			CreateVariableTempTable(VariableTableName);
		}

		if (VariableDeclarationCount == 0) {
			Emit("DELETE FROM ").TableName(VariableTableName, TableType.Temporary).EndOfStatement();
		}

		var variableID = VariableDeclarationCount++;
		Insert(this.QuickString("{0}", SQLBuilderCommand.TableName(VariableTableName, TableType.Temporary)), new[] { new ColumnValue("ID", variableID) });
		_variableDeclarations.Add(variableName, Tuple.Create(variableID, TypeToStorageClass(type)));
		return this;
	}

	public override ISQLBuilder AssignVariable(string variableName, object value) {
		if (!_variableDeclarations.ContainsKey(variableName))
			throw new SoftwareException("A variable with name '{0}' has not been declared", variableName);

		var variable = _variableDeclarations[variableName];
		var variableID = variable.Item1;
		var variableStorageClass = variable.Item2;


		Update(
			this.QuickString("{0}", SQLBuilderCommand.TableName(VariableTableName, TableType.Temporary)),
			new[] {
				new ColumnValue("IntValue", variableStorageClass == VariableStorageClass.Integer ? value : null),
				new ColumnValue("TextValue", variableStorageClass == VariableStorageClass.Text ? value : null),
				new ColumnValue("DateValue", variableStorageClass == VariableStorageClass.DateTime ? value : null),
				new ColumnValue("UuidValue", variableStorageClass == VariableStorageClass.Uuid ? value : null),
				new ColumnValue("BlobValue", variableStorageClass == VariableStorageClass.Blob ? value : null)
			},
			matchColumns:
			new[] {
				new ColumnValue("ID", variableID)
			}
		);

		return this;
	}

	public override ISQLBuilder VariableName(string variableName) {
		if (!_variableDeclarations.ContainsKey(variableName))
			throw new SoftwareException("A variable with name '{0}' has not been declared", variableName);

		var variable = _variableDeclarations[variableName];
		var variableID = variable.Item1;
		var variableStorageClass = variable.Item2;

		string column;
		switch (variableStorageClass) {
			case VariableStorageClass.Integer:
				column = "IntValue";
				break;
			case VariableStorageClass.DateTime:
				column = "DateValue";
				break;
			case VariableStorageClass.Text:
				column = "TextValue";
				break;
			case VariableStorageClass.Uuid:
				column = "UuidValue";
				break;

			case VariableStorageClass.Blob:
				column = "BlobValue";
				break;
			default:
				throw new NotImplementedException("Unknown VariableStorageClass " + variableStorageClass.ToString());
		}

		return
			Emit(
				"(SELECT {0} FROM {1} WHERE {2} = {3})",
				SQLBuilderCommand.ColumnName(column),
				SQLBuilderCommand.TableName(VariableTableName, TableType.Temporary),
				SQLBuilderCommand.ColumnName("ID"),
				SQLBuilderCommand.Literal(variableID)
			);
	}


	public virtual ISQLBuilder CreateVariableTempTable(string tableName) {
		CreateTable(
			new TableSpecification {
				Name = VariableTableName,
				Type = TableType.Temporary,
				PrimaryKey = new PrimaryKeySpecification { Columns = new[] { "ID" } },
				Columns =
					new[] {
						new ColumnSpecification {
							Name = "ID",
							Type = typeof(int),
							Nullable = false
						}
					}.Concat(
						from storageClass in _columnLookup.Keys
						let storageColumn = _columnLookup[storageClass]
						orderby storageColumn.Item1
						let columnName = storageColumn.Item2
						select new ColumnSpecification {
							Name = columnName,
							Type = StorageClassToType(storageClass),
							Nullable = true
						}
					)
			}
		);
		VariableTableHasBeenCreated = true;
		return this;

	}

	protected virtual VariableStorageClass TypeToStorageClass(Type type) {
		var storageClass = VariableStorageClass.Integer;
		if (type.IsIntegerNumeric()) {
			storageClass = VariableStorageClass.Integer;
		} else {
			TypeSwitch.ForType(
				type,
				TypeSwitch.Case<Guid>(() => storageClass = VariableStorageClass.Uuid),
				TypeSwitch.Case<string>(() => storageClass = VariableStorageClass.Text),
				TypeSwitch.Case<DateTime>(() => storageClass = VariableStorageClass.DateTime),
				TypeSwitch.Case<byte[]>(() => storageClass = VariableStorageClass.Blob),
				TypeSwitch.Default(() => { throw new SoftwareException("Unable to map type to variable streams class '{0}'", type.Name); })
			);
		}
		return storageClass;
	}

	protected virtual Type StorageClassToType(VariableStorageClass storageClass) {
		switch (storageClass) {
			case VariableStorageClass.Integer:
				return typeof(long);
			case VariableStorageClass.Blob:
				return typeof(byte[]);
			case VariableStorageClass.DateTime:
				return typeof(DateTime);
			case VariableStorageClass.Text:
				return typeof(string);
			case VariableStorageClass.Uuid:
				return typeof(Guid);
		}
		throw new Exception("Internal error {2A405A15-44B9-4444-A940-87E679FC637C}");
	}


	protected enum VariableStorageClass {
		[Description("IntValue")] Integer,

		[Description("TextValue")] Text,

		[Description("DateValue")] DateTime,

		[Description("UuidValue")] Uuid,

		[Description("BlobValue")] Blob
	}

}
