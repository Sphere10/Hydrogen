// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen.DApp.Core.Consensus;

/// <summary>
/// A Hydrogen Operation references a rule (method), arguments (method arguments), entities (list of entities it
/// </summary>
public class Operation {

	public int Rule { get; set; }

}


public class Transaction {
	public byte[][] Arguments { get; set; }

	public byte[][] Entities { get; set; }

	public IEnumerable<byte[]> Signatures { get; set; }

}


public class Entity {
	public byte[] Object { get; }
}
