// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Data;

public class DBColumnSchema : DBObject {

	public DBColumnSchema() {
		KeyType = DBKeyType.None;
	}

	public override string Name { get; set; }
	public int Position { get; set; }
	public bool IsForeignKey { get; set; }
	public bool IsPrimaryKey { get; set; }
	public DBKeyType KeyType { get; internal set; }
	public bool IsUnique { get; set; }
	public bool IsNullable { get; set; }
	public bool IsAutoIncrement { get; set; }

	public bool UsesSequence {
		get { return !string.IsNullOrEmpty(Sequence); }
	}

	public string Sequence { get; set; }
	public bool CascadesOnUpdate { get; set; }
	public bool CascadesOnDelete { get; set; }
	public string DataType { get; set; }
	public int DataTypeLength { get; set; }
	public int Precision { get; set; }
	public int Scale { get; set; }
	public override string SQL { get; set; }
	public Type Type { get; set; }
	public DBTableSchema Owner { get; set; }
}
