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

public class DBTableSchema : DBObject {
	public DBTableSchema() {
		Columns = new DBColumnSchema[0];
		ForeignKeys = new DBForeignKeySchema[0];
		UniqueConstraints = new DBUniqueConstraintSchema[0];
		PrimaryKeyColumns = new DBColumnSchema[0];
		BlobColumns = new DBColumnSchema[0];
	}

	public override string Name { get; set; }

	public int TopologicalOrder { get; set; }

	public int Height { get; internal set; }

	internal bool HeightHasBeenCalculated { get; set; }

	public DBColumnSchema[] Columns { get; set; }

	public DBPrimaryKeySchema PrimaryKey { get; set; }

	public DBForeignKeySchema[] ForeignKeys { get; set; }

	public DBUniqueConstraintSchema[] UniqueConstraints { get; set; }

	public DBColumnSchema[] PrimaryKeyColumns { get; internal set; }

	public DBColumnSchema[] BlobColumns { get; internal set; }

	public IDictionary<string, DBKeyRoot> KeyRoots { get; internal set; }

	public override string SQL { get; set; }

	public DBColumnSchema GetColumn(string column) {
		if (!ContainsColumn(column))
			throw new Exception(string.Format("Table '{0}' does not contain column '{1}'", Name, column));
		if (ColumnIndex != null)
			return Columns[ColumnIndex[column]];

		return Columns.Single(c => c.Name == column);
	}

	public IEnumerable<Tuple<DBColumnSchema, DBColumnSchema>> GetColumnBindings(IEnumerable<DBColumnSchema> columns) {
		var bindings = new List<Tuple<DBColumnSchema, DBColumnSchema>>();
		foreach (var column in columns) {
			DBColumnSchema reference = null;
			var foreignKeysInvolvingColumn = ForeignKeys.Where(fk => column.Name.IsIn(fk.ForeignKeyColumns));
			if (foreignKeysInvolvingColumn.Any()) {
				var foreignKey = foreignKeysInvolvingColumn.Single();
				var index = foreignKey.ForeignKeyColumns.EnumeratedIndexOf(column.Name);
				var referenceColumnName = foreignKey.ReferenceColumns[index];
				var referenceTable = this.Owner[foreignKey.ReferenceTable];
				reference = referenceTable[referenceColumnName];
			}
			bindings.Add(Tuple.Create(column, reference));
		}
		return bindings;
	}

	public IEnumerable<string> GetForeignKeyTables() {
		return ForeignKeys.Select(foreignKey => foreignKey.ReferenceTable).Distinct();
	}

	public bool ContainsColumn(string columnName) {
		if (ColumnIndex != null)
			return ColumnIndex.ContainsKey(columnName);

		return Columns.Any(c => c.Name == columnName);
	}

	public DBColumnSchema this[string columnName] {
		get { return GetColumn(columnName); }
	}

	internal IDictionary<string, int> ColumnIndex { get; set; }

	public DBSchema Owner { get; internal set; }

}
