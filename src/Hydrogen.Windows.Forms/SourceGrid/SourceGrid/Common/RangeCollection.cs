// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace SourceGrid;

/// <summary>
/// A collection of elements of type Range
/// </summary>
[Serializable]
public class RangeCollection : List<CellRange> {
	/// <summary>
	/// Returns true if the specified cell position is present in any range in the current collection.
	/// </summary>
	/// <param name="p_Position"></param>
	/// <returns></returns>
	public bool ContainsCell(Position p_Position) {
		foreach (CellRange range in this) {
			if (range.Contains(p_Position))
				return true;
		}
		return false;
	}
}
