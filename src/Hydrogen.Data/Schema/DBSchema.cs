// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Hydrogen.Data;

public class DBSchema : DBObject {
	private IDictionary<string, DBTableSchema> _tables;
	private DBTableSchema[] _tablesValues;

	public DBSchema() {
		_tables = new Dictionary<string, DBTableSchema>();
		Relations = null;
	}

	public string DatabaseID;

	public int MaxHeight { get; internal set; }

	public override string Name { get; set; }

	public DBTableSchema[] Tables {
		get { return _tablesValues; }
		set {
			_tables.Clear();
			_tables = value.ToDictionary(v => v.Name);
			_tablesValues = _tables.Values.ToArray();
		}
	}

	public DBTriggerSchema[] Triggers { get; set; }

	public DBForeignKeySchema[] Relations { get; internal set; }

	public override string SQL { get; set; }

	public DBTableSchema this[string tableName] {
		get {
			if (!_tables.ContainsKey(tableName))
				throw new Exception(string.Format("Table '{0}' does not exist", tableName));
			return _tables[tableName];
		}
	}

	public bool ContainsTable(string tableName) {
		return _tables.ContainsKey(tableName);
	}

	public IEnumerable<Tuple<DBTableSchema, IEnumerable<DBColumnSchema>>> GetDependencies(DBTableSchema table, IEnumerable<DBColumnSchema> tableColumns, bool fetchTransitiveDependencies = true, bool fetchArtificialOnly = false) {
		return GetDependenciesRecursive(table, tableColumns, fetchTransitiveDependencies: fetchTransitiveDependencies, fetchArtificialOnly: fetchArtificialOnly);
	}

	public DBSchema ApplyArtificialKeyFile(string xmlFile) {
		return ApplyArtificialKeys(Tools.Xml.ReadFromFile<ArtificialKeys>(xmlFile));
	}

	public DBSchema ApplyArtificialKeys(string artificialKeysXML) {
		return ApplyArtificialKeys(Tools.Xml.ReadFromString<ArtificialKeys>(artificialKeysXML));
	}

	public DBSchema ApplyArtificialKeys(ArtificialKeys artificialKeys) {
		if (artificialKeys == null)
			throw new ArgumentNullException("artificialKeys");

		if (artificialKeys.Tables == null)
			throw new ArgumentException("Artificial Keys did not contain any tables", "artificialKeys");

		artificialKeys.ApplyToSchema(this);

		return this;
	}

	public void FinishedBuilding() {
		if (!Tables.Any())
			return;

		// Set the global relations
		Relations = Tables.SelectMany(t => t.ForeignKeys).ToArray();

		// Sort the tables in topological order
		Tables = SortTopologically(Tables, false).ToArray();
		for (int i = 0; i < Tables.Length; i++) {
			Tables[i].TopologicalOrder = i;
		}


		// Sort the table unique constraints in order
		Tables.ForEach(table =>
			table.UniqueConstraints = (
				from uniqueConstraint in table.UniqueConstraints
				let minColumnIndex = (from column in table.Columns where column.Name.IsIn(uniqueConstraint.Columns) select column.Position).Min()
				orderby minColumnIndex ascending
				select uniqueConstraint
			).ToArray()
		);

		// Sort the table foreign keys in order
		Tables.ForEach(table =>
			table.ForeignKeys = (
				from foreignKey in table.ForeignKeys
				let minColumnIndex = (from column in table.Columns where column.Name.IsIn(foreignKey.ForeignKeyColumns) select column.Position).Min()
				orderby minColumnIndex ascending
				select foreignKey
			).ToArray()
		);


		// Set convenience properties
		Tables.ForEach(table => {
			table.Owner = this;
			table.Columns = (from col in table.Columns orderby col.Position ascending select col).ToArray();
			table.ColumnIndex = (from col in table.Columns.WithDescriptions() select col).ToDictionary(x => x.Item.Name, x => x.Index);
			if (table.PrimaryKey != null)
				table.PrimaryKeyColumns = (from pk in table.PrimaryKey.ColumnNames select table.Columns[table.ColumnIndex[pk]]).ToArray();
			table.BlobColumns = (from col in table.Columns where col.Type == typeof(byte[]) select col).ToArray();

			#region Set Column owner

			table.Columns.ForEach(c => c.Owner = table);

			#endregion

			#region Set if a primary key is also a foreign key

			if (table.PrimaryKey != null) {
				table.PrimaryKey.ColumnsAlsoForeignKeys = table.PrimaryKeyColumns.Any(c => c.IsForeignKey);
				if (table.PrimaryKey.KeyType.HasFlag(DBKeyType.Artificial))
					table.PrimaryKeyColumns.ForEach(c => c.KeyType = c.KeyType.CopyAndSetFlags(DBKeyType.Artificial));
			}

			#endregion

			#region Set the Sequence/Generator

			if (table.PrimaryKey != null) {
				if (!string.IsNullOrEmpty(table.PrimaryKey.Sequence))
					table.PrimaryKeyColumns.Single().Sequence = table.PrimaryKey.Sequence;
			}

			#endregion

			#region Set if Foreign Key is to a constraint or a primary key

			foreach (var foreignKeyDescription in table.ForeignKeys.WithDescriptions()) {
				foreignKeyDescription.Item.Position = foreignKeyDescription.Index;

				var referenceTable = this[foreignKeyDescription.Item.ReferenceTable];
				var referenceColumns = referenceTable.Columns.Where(c => c.Name.IsIn(foreignKeyDescription.Item.ReferenceColumns));
				var foreignKeyColumns = table.Columns.Where(c => c.Name.IsIn(foreignKeyDescription.Item.ForeignKeyColumns));
				if (referenceColumns.Any(c => !c.IsPrimaryKey))
					foreignKeyDescription.Item.ReferenceIsUniqueConstraint = true;
				else
					foreignKeyDescription.Item.ReferenceIsUniqueConstraint = false;

				foreignKeyDescription.Item.ReferenceTablePrimaryKey = referenceTable.PrimaryKey.ColumnNames;

				if (foreignKeyDescription.Item.KeyType.HasFlag(DBKeyType.Artificial))
					foreignKeyColumns.ForEach(c => c.KeyType = c.KeyType.CopyAndSetFlags(DBKeyType.Artificial));

			}

			#endregion

			#region Set Unique Constraint Properties

			foreach (var uniqueConstraintDescription in table.UniqueConstraints.WithDescriptions()) {
				var uniqueConstraintColumns = table.Columns.Where(c => c.Name.IsIn(uniqueConstraintDescription.Item.Columns));
				uniqueConstraintDescription.Item.Position = uniqueConstraintDescription.Index;
				uniqueConstraintDescription.Item.ColumnsAlsoForeignKey = uniqueConstraintColumns.Any(c => c.IsForeignKey);

				if (uniqueConstraintDescription.Item.KeyType.HasFlag(DBKeyType.Artificial))
					uniqueConstraintColumns.ForEach(c => c.KeyType = c.KeyType.CopyAndSetFlags(DBKeyType.Artificial));
			}

			#endregion

			// Determine the primary key type
			var primaryKey = table.PrimaryKey;
			if (primaryKey != null) {
				if (primaryKey.ColumnNames.Length == 1) {
					var pkCol = table.GetColumn(primaryKey.ColumnNames[0]);
					if (pkCol.IsAutoIncrement)
						primaryKey.KeyType = primaryKey.KeyType.CopyAndSetFlags(DBKeyType.AutoCalculated);
					else if (pkCol.UsesSequence)
						primaryKey.KeyType = primaryKey.KeyType.CopyAndSetFlags(DBKeyType.Sequenced);
					else if (table.PrimaryKeyColumns.Length == 1 && pkCol.Type.IsIntegerNumeric())
						primaryKey.KeyType = primaryKey.KeyType.CopyAndSetFlags(DBKeyType.ManuallyAssignedSingleInt);
				}

				foreach (var primaryKeyColumnName in primaryKey.ColumnNames) {
					var primaryKeyColumn = table.GetColumn(primaryKeyColumnName);
					primaryKeyColumn.KeyType = primaryKeyColumn.KeyType.CopyAndSetFlags(primaryKey.KeyType);
				}
			}

		});

		// Determine the foreign key types and nullability
		Tables.ForEach(table => {
			foreach (var foreignKey in table.ForeignKeys) {
				if (foreignKey.ForeignKeyColumns.Length != foreignKey.ReferenceColumns.Length)
					throw new Exception(string.Format("Foreign key '{0}' in table '{1}' is malformed, inconsistent columns specified for primary key table"));

				////////////// Is this really necessary ?
				if (foreignKey.ReferenceColumns.Length == 1) {
					var pkTable = this[foreignKey.ReferenceTable];
					var pkCol = pkTable.GetColumn(foreignKey.ReferenceColumns[0]);
					if (pkCol.IsAutoIncrement)
						foreignKey.KeyType = foreignKey.KeyType.CopyAndSetFlags(DBKeyType.AutoCalculated);
					else if (pkCol.UsesSequence)
						foreignKey.KeyType = foreignKey.KeyType.CopyAndSetFlags(DBKeyType.Sequenced);
					else if (pkTable.PrimaryKeyColumns.Length == 1 && pkCol.Type.IsIntegerNumeric())
						foreignKey.KeyType = foreignKey.KeyType.CopyAndSetFlags(DBKeyType.ManuallyAssignedSingleInt);
				}
				//////////////

				foreach (var col in foreignKey.ForeignKeyColumns) {
					var foreignKeyColumn = table.GetColumn(col);
					foreignKeyColumn.KeyType = foreignKeyColumn.KeyType.CopyAndSetFlags(foreignKey.KeyType);
				}
				foreignKey.IsNullable = foreignKey.ForeignKeyColumns.All(c => table[c].IsNullable);
			}
		});

		// Now go through and determine the roots
		Tables.ForEach(table => {
			table.KeyRoots =
				table
					.Columns
					.Where(column => column.IsPrimaryKey || column.IsForeignKey)
					.Select(
						column => {
							var keyRoot = FindKeyRootRecursively(table, column);

							if (keyRoot.IsAutoIncrement)
								column.KeyType = column.KeyType.CopyAndSetFlags(DBKeyType.RootIsAutoCalculated);

							if (keyRoot.UsesSequence)
								column.KeyType = column.KeyType.CopyAndSetFlags(DBKeyType.RootIsSequenced);

							if (this[keyRoot.RootTable][keyRoot.RootColumn].KeyType.HasFlag(DBKeyType.ManuallyAssignedSingleInt))
								column.KeyType = column.KeyType.CopyAndSetFlags(DBKeyType.RootIsManuallyAssignedSingleInt);

							return new {
								ColumnName = column.Name,
								KeyRoot = keyRoot
							};
						}
					)
					.ToDictionary(x => x.ColumnName, x => x.KeyRoot);
		});

		// Calculate the height
		Tables.ForEach(t => CalculateHeight(t));

		MaxHeight = Tables.Select(t => t.Height).Max();

	}

	public IEnumerable<string> GetDependencyList(string table) {
		var hashSet = new HashSet<string>();
		FindDependencyRecurisvely(table, hashSet);
		return hashSet;
	}

	private void FindDependencyRecurisvely(string table, HashSet<string> existingDependencies) {
		var dependencies = this[table].GetForeignKeyTables();
		var newDependencies = dependencies.Except(existingDependencies).ToArray();
		existingDependencies.AddRange(dependencies);
		foreach (var dependency in newDependencies) {
			FindDependencyRecurisvely(dependency, existingDependencies);
		}
	}

	private static IEnumerable<DBTableSchema> SortTopologically(IEnumerable<DBTableSchema> tables, bool treatNullableForeignKeysAsDepedency) {
		var tableLookup = tables.ToLookup(table => table.Name);
		return TopologicalSorter.TopologicalSort(
			tables,
			table => (
				from foreignKey in table.ForeignKeys
				where treatNullableForeignKeysAsDepedency || !foreignKey.IsNullable
				select tableLookup[foreignKey.ReferenceTable]
			).Unpartition()
		);

	}

	public IEnumerable<IEnumerable<DBTableSchema>> GetTableRoutes(DBTableSchema fromTable, DBTableSchema toTable) {
		var paths = new List<IEnumerable<DBTableSchema>>();
		FindPathsRecursive(fromTable, toTable, Enumerable.Empty<DBTableSchema>(), paths.Add);
		return paths;
	}

	private IEnumerable<Tuple<DBTableSchema, IEnumerable<DBColumnSchema>>> GetDependenciesRecursive(DBTableSchema table, IEnumerable<DBColumnSchema> tableColumns, HashSet<string> visitedTables = null, bool fetchTransitiveDependencies = true,
	                                                                                                bool fetchArtificialOnly = false) {

		#region Argument Validation

		if (table == null)
			throw new ArgumentNullException("table");

		if (tableColumns == null)
			throw new ArgumentNullException("tableColumns");

		if (!tableColumns.Any())
			throw new ArgumentException("No columns specified", "tableColumns");

		#endregion

		if (visitedTables == null)
			visitedTables = new HashSet<string>();

		if (visitedTables.Contains(table.Name))
			return Enumerable.Empty<Tuple<DBTableSchema, IEnumerable<DBColumnSchema>>>();

		visitedTables.Add(table.Name);

		var keyColumnNames = tableColumns.Select(c => c.Name);

		var dependencies = (
			from foreignKey in Relations
			where
				fetchArtificialOnly
					? foreignKey.KeyType.HasFlag(DBKeyType.Artificial)
					: true &&
					  foreignKey.ReferenceTable == table.Name && foreignKey.ReferenceColumns.ContainSameElements(keyColumnNames)
			let foreignKeyTable = this[foreignKey.ForeignKeyTable]
			select Tuple.Create(foreignKeyTable, from fkCol in foreignKey.ForeignKeyColumns select foreignKeyTable[fkCol])
		);

		if (fetchTransitiveDependencies) {
			// Fetch the transitive dependencies
			//	 e.g 	A.ColID -> B.ID -> table.ID

			foreach (var dependency in dependencies.ToArray())
				dependencies = dependencies.Concat(GetDependenciesRecursive(dependency.Item1, dependency.Item2, visitedTables));

		}
		return dependencies;
	}

	private DBKeyRoot FindKeyRootRecursively(DBTableSchema table, DBColumnSchema keyColumn) {
		if (!(keyColumn.IsPrimaryKey || keyColumn.IsForeignKey || keyColumn.IsUnique))
			throw new ArgumentException("Column is not a key", "keyColumn");

		if ((keyColumn.IsPrimaryKey || keyColumn.IsUnique) && !keyColumn.IsForeignKey)
			return new DBKeyRoot {
				RootColumn = keyColumn.Name,
				RootTable = table.Name,
				IsAutoIncrement = keyColumn.IsAutoIncrement,
				UsesSequence = keyColumn.UsesSequence
			};

		Debug.Assert(keyColumn.IsForeignKey);

		// Since this is not a root primary key, recursively call the linked to primary key
		foreach (var foreignKey in table.ForeignKeys) {
			for (int i = 0; i < foreignKey.ForeignKeyColumns.Length; i++) {
				if (foreignKey.ForeignKeyColumns[i] == keyColumn.Name) {
					var tableName = foreignKey.ReferenceTable;
					var columnName = foreignKey.ReferenceColumns[i];
					var pkTable = this[tableName];
					var pkColumn = pkTable.GetColumn(columnName);

					// If it's a foreign key to itself (stupid but possible), then it is the root
					if (table.Name == pkTable.Name && keyColumn.Name == pkColumn.Name)
						return new DBKeyRoot {
							RootColumn = keyColumn.Name,
							RootTable = table.Name,
							IsAutoIncrement = keyColumn.IsAutoIncrement,
							UsesSequence = keyColumn.UsesSequence
						};

					return FindKeyRootRecursively(pkTable, pkColumn);
				}
			}
		}
		throw new Exception(string.Format("Unable to find key root for {0}.{1}", table.Name, keyColumn.Name));
	}

	private void FindPathsRecursive(DBTableSchema from, DBTableSchema to, IEnumerable<DBTableSchema> currentPath, Action<IEnumerable<DBTableSchema>> notifyFoundPath) {

		if (currentPath.Contains(from))
			return; // cyclic path, ignore

		currentPath = currentPath.Concat(from);

		if (from == to)
			notifyFoundPath(currentPath);

		foreach (var foreignKey in from.ForeignKeys) {
			FindPathsRecursive(this[foreignKey.ReferenceTable], to, currentPath, notifyFoundPath);
		}
	}

	private int CalculateHeight(DBTableSchema table, HashSet<DBTableSchema> visitedTables = null) {
		if (visitedTables == null)
			visitedTables = new HashSet<DBTableSchema>();

		if (visitedTables.Contains(table))
			return 0;

		visitedTables.Add(table);


		if (!table.HeightHasBeenCalculated) {
			table.Height = !table.ForeignKeys.Any() ? 1 : table.ForeignKeys.Select(fk => CalculateHeight(this[fk.ReferenceTable], visitedTables)).Max() + 1;
			table.HeightHasBeenCalculated = true;
		}
		return table.Height;
	}
}
