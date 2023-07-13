// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Data;

public class DBUniqueConstraintSchema : DBObject {
	public override string Name { get; set; }
	public string[] Columns { get; set; }
	public override string SQL { get; set; }
	public DBKeyType KeyType { get; internal set; }
	public int Position { get; internal set; }
	public bool ColumnsAlsoForeignKey { get; internal set; }
}
