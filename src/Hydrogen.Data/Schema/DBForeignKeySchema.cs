// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.


namespace Hydrogen.Data;

public class DBForeignKeySchema : DBObject {
	public override string Name { get; set; }
	public DBKeyType KeyType { get; internal set; }
	public string ForeignKeyTable { get; set; }
	public string[] ForeignKeyColumns { get; set; }
	public string ReferenceTable { get; set; }
	public string[] ReferenceColumns { get; set; }
	public string[] ReferenceTablePrimaryKey { get; internal set; }
	public bool IsNullable { get; set; }
	public bool ReferenceIsUniqueConstraint { get; set; }
	public bool CascadesOnUpdate { get; set; }
	public bool CascadesOnDelete { get; set; }
	public int Position { get; set; }
	public override string SQL { get; set; }
}
