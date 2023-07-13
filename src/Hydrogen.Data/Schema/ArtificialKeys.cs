// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;


namespace Hydrogen.Data;

//<ArtificialKeys>
//    <Table name = "Table1">
//        <PrimaryKey name="PK1" autoIncrement="true" sequence="generatorName">
//            <Column name="ID"/>
//        </PrimaryKey>

//        <ForeignKey name="FK1" referenceTable="Table2">
//            <Column name="A" references="U">
//            <Column name="B" references="V">
//            <Column name="C" references="W">
//        </ForeignKey>

//        <UniqueConstraint name="UC1">
//            <Column>X</Column>
//            <Column>Y</Column>
//        </UniqueConstraint>
//    </Table>

//    <Table name="Table2">
//        <PrimaryKey name="PK1" sequence="Sequence1">
//            <Column name="A"/>
//        </PrimaryKey>
//    </Table>

//</ArtificialKeys>


[XmlRoot("ArtificialKeys")]
public class ArtificialKeys {

	[XmlElement("Table")] public Table[] Tables { get; set; }


	public class Table {

		[XmlAttribute("name")] public string Name { get; set; }

		[XmlElement("PrimaryKey")] public PrimaryKey PrimaryKey { get; set; }

		[XmlElement("ForeignKey")] public ForeignKey[] ForeignKeys { get; set; }

		[XmlElement("UniqueConstraint")] public UniqueConstraint[] UniqueConstraints { get; set; }

	}


	public class PrimaryKey {

		[XmlAttribute("name")] public string Name { get; set; }

		[XmlAttribute("sequence")] public string Sequence { get; set; }

		[XmlAttribute("autoIncrement")] public bool AutoIncrement { get; set; }

		[XmlElement("Column")] public Column[] Columns { get; set; }

	}


	public class ForeignKey {
		[XmlAttribute("name")] public string Name { get; set; }

		[XmlAttribute("referenceTable")] public string ReferenceTable { get; set; }

		[XmlElement("Column")] public Column[] Columns { get; set; }

	}


	public class UniqueConstraint {
		[XmlAttribute("name")] public string Name { get; set; }
		[XmlElement("Column")] public Column[] Columns { get; set; }

	}


	public class Column {

		[XmlAttribute("name")] public string Name { get; set; }

		[XmlAttribute("references")] public string References { get; set; }

	}


	public static ArtificialKeys LoadFromFile(string path) {
		return Tools.Xml.ReadFromFile<ArtificialKeys>(path);
	}

	public static ArtificialKeys LoadFromString(string xml) {
		return Tools.Xml.ReadFromString<ArtificialKeys>(xml);
	}
	internal void ApplyToSchema(DBSchema schema) {
		if (Tables == null)
			throw new SoftwareException("Artificial keys did not specify in tables");

		foreach (var table in Tables) {

			if (!schema.ContainsTable(table.Name))
				throw new SoftwareException("Artificial Keys cannot be applied as table '{0}' does not exist", table.Name);

			var tableSchema = schema[table.Name];


			if (table.PrimaryKey != null) {
				// Remove primary key if present (auto override)
				tableSchema.PrimaryKey = null;

				if (tableSchema.PrimaryKey != null)
					throw new SoftwareException("Artificial primary key '{0}' cannot be applied to table '{1}' as it already contains a primary key", table.PrimaryKey.Name, tableSchema.Name);

				if (!table.PrimaryKey.Columns.Any(c => tableSchema.ContainsColumn(c.Name)))
					throw new SoftwareException("Artificial primary key '{0}' cannot be applied to table '{1}' as the specified columns '{2}' do not exist",
						table.PrimaryKey.Name,
						tableSchema.Name,
						table.PrimaryKey.Columns.First(c => !tableSchema.ContainsColumn(c.Name)).Name);

				if (!string.IsNullOrEmpty(table.PrimaryKey.Sequence) && table.PrimaryKey.Columns.Count() != 1) {
					throw new SoftwareException("Artificial primary key '{0}' cannot be applied to table '{1}' as a sequence can only apply to a single primary key column", table.PrimaryKey.Name, tableSchema.Name);
				}

				if (table.PrimaryKey.AutoIncrement && table.PrimaryKey.Columns.Count() != 1) {
					throw new SoftwareException("Artificial primary key '{0}' cannot be applied to table '{1}' as autoincrement can only apply to a single primary key column", table.PrimaryKey.Name, tableSchema.Name);
				}

				if ((!string.IsNullOrEmpty(table.PrimaryKey.Sequence) || table.PrimaryKey.AutoIncrement) && !tableSchema.GetColumn(table.PrimaryKey.Columns[0].Name).Type.IsIntegerNumeric()) {
					throw new SoftwareException("Artificial primary key '{0}' with sequence and/or autoincrement cannot be applied to table '{1}' as it's primary key is not an integer-based type", table.PrimaryKey.Name, tableSchema.Name);
				}

				tableSchema.PrimaryKey = new DBPrimaryKeySchema {
					Name = table.PrimaryKey.Name,
					Sequence = table.PrimaryKey.Sequence,
					ColumnNames = (from c in table.PrimaryKey.Columns select c.Name).ToArray(),
					KeyType = DBKeyType.Artificial | (table.PrimaryKey.AutoIncrement ? DBKeyType.AutoCalculated : !string.IsNullOrEmpty(table.PrimaryKey.Sequence) ? DBKeyType.Sequenced : DBKeyType.None),
					SQL = Tools.Xml.WriteToString(this)
				};

				tableSchema.PrimaryKeyColumns = (from pk in tableSchema.PrimaryKey.ColumnNames select tableSchema.GetColumn(pk)).ToArray();

				tableSchema.PrimaryKeyColumns.ForEach(pkCol => pkCol.IsPrimaryKey = true);
				if (table.PrimaryKey.AutoIncrement) {

					tableSchema.PrimaryKeyColumns[0].IsAutoIncrement = true;
				}

				if (!string.IsNullOrEmpty(table.PrimaryKey.Sequence))
					tableSchema.PrimaryKeyColumns[0].Sequence = table.PrimaryKey.Sequence;

			}

			if (table.UniqueConstraints != null && table.UniqueConstraints.Length > 0) {
				var schemaUniqueKeys = tableSchema.UniqueConstraints.ToDictionary(z => z.Name);
				var newUniqueKeys = new List<DBUniqueConstraintSchema>();
				foreach (var uniqueKey in table.UniqueConstraints) {
					if (schemaUniqueKeys.ContainsKey(uniqueKey.Name))
						throw new SoftwareException("Artificial unique constraint '{0}' cannot be applied to table '{1}' as a unique constraint with that name already exists", uniqueKey.Name, tableSchema.Name);

					if (!uniqueKey.Columns.Any(c => tableSchema.ContainsColumn(c.Name)))
						throw new SoftwareException("Artificial unique constraint '{0}' cannot be applied to table '{1}' as it references non-existent column(s)", uniqueKey.Name, tableSchema.Name);

					var ucCols = tableSchema.Columns.Where(c => c.Name.IsIn(uniqueKey.Columns.Select(x => x.Name))).ToArray();
					ucCols.ForEach(ucCol => ucCol.IsUnique = true);
					newUniqueKeys.Add(
						new DBUniqueConstraintSchema {
							Name = uniqueKey.Name,
							Columns = ucCols.Select(c => c.Name).ToArray(),
							KeyType = DBKeyType.Artificial,
							SQL = Tools.Xml.WriteToString(uniqueKey)
						}
					);
				}
				tableSchema.UniqueConstraints = tableSchema.UniqueConstraints.Concat(newUniqueKeys).ToArray();
			}
		}

		// Foreign keys are processed after all primary/unique keys (for complex dependency graphs)
		foreach (var table in Tables) {
			var tableSchema = schema[table.Name];
			if (table.ForeignKeys != null && table.ForeignKeys.Length > 0) {
				var schemaForeignKeys = tableSchema.ForeignKeys.ToDictionary(z => z.Name);
				var newForeignKeys = new List<DBForeignKeySchema>();
				foreach (var foreignKey in table.ForeignKeys) {
					if (schemaForeignKeys.ContainsKey(foreignKey.Name))
						throw new SoftwareException("Artificial foreign key '{0}' cannot be applied to table '{1}' as it already contains a foreign key by that name", foreignKey.Name, tableSchema.Name);

					if (!foreignKey.Columns.Any(c => tableSchema.ContainsColumn(c.Name)))
						throw new SoftwareException("Artificial foreign key '{0}' cannot be applied to table '{1}' as it references a non-existent column '{2}'",
							foreignKey.Name,
							tableSchema.Name,
							foreignKey.Columns.First(c => !tableSchema.ContainsColumn(c.Name)).Name);

					if (string.IsNullOrEmpty(foreignKey.ReferenceTable))
						throw new SoftwareException("Artificial foreign key '{0}' cannot be applied to table '{1}' as it did not specify a reference table", foreignKey.Name, tableSchema.Name);

					if (!schema.ContainsTable(foreignKey.ReferenceTable))
						throw new SoftwareException("Artificial foreign key '{0}' cannot be applied to table '{1}' as it referenced a non-existant reference table '{2}'", foreignKey.Name, tableSchema.Name, foreignKey.ReferenceTable);

					var primaryKeyTable = schema[foreignKey.ReferenceTable];
					if (!foreignKey.Columns.Any(c => primaryKeyTable.ContainsColumn(c.Name)))
						throw new SoftwareException("Artificial foreign key '{0}' cannot be applied to table '{1}' as it links to a non-existant column '{2}'",
							foreignKey.Name,
							tableSchema.Name,
							foreignKey.Columns.First(c => !primaryKeyTable.ContainsColumn(c.Name)).Name);

					var referenceColumnsInPK = primaryKeyTable.PrimaryKey != null && primaryKeyTable.PrimaryKey.ColumnNames.ContainSameElements(foreignKey.Columns.Select(c => c.Name));
					var referenceColumnsInUC = primaryKeyTable.UniqueConstraints.Any(uc => uc.Columns.ContainSameElements(foreignKey.Columns.Select(c => c.Name)));

					if (!referenceColumnsInPK && !referenceColumnsInUC)
						throw new SoftwareException("Artificial foreign key '{0}' cannot be applied to table '{1}' as the specified reference columns are not primary or unique keys in table '{2}'",
							foreignKey.Name,
							tableSchema.Name,
							foreignKey.ReferenceTable);

					if (foreignKey.Columns.Any(c => string.IsNullOrEmpty(c.References)))
						throw new SoftwareException("Artificial foreign key '{0}' cannot be applied to table '{1}' as some column(s) did not link to a primary key table column", foreignKey.Name, tableSchema.Name);

					var fcCols = tableSchema.Columns.Where(c => c.Name.IsIn(foreignKey.Columns.Select(x => x.Name))).ToArray();
					fcCols.ForEach(ucCol => ucCol.IsForeignKey = true);
					newForeignKeys.Add(
						new DBForeignKeySchema {
							Name = foreignKey.Name,
							ForeignKeyTable = tableSchema.Name,
							ForeignKeyColumns = (from c in foreignKey.Columns select c.Name).ToArray(),
							ReferenceTable = foreignKey.ReferenceTable,
							ReferenceColumns = (from c in foreignKey.Columns select c.References).ToArray(),
							KeyType = DBKeyType.Artificial,
							SQL = Tools.Xml.WriteToString(foreignKey)
						}
					);
				}
				tableSchema.ForeignKeys = newForeignKeys.Concat(tableSchema.ForeignKeys).ToArray();
			}


		}
	}
}
