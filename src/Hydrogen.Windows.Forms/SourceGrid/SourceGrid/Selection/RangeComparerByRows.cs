// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace SourceGrid.Selection;

public class RangeComparerByRows : IComparer<CellRange> {
	public int Compare(CellRange x, CellRange y) {
		if (x.Start.Row == y.Start.Row)
			return 0;
		if (x.Start.Row > y.Start.Row)
			return 1;
		return -1;
	}
}
