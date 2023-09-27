// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid;

public class RowInfoCollectoinHiddenRowCoordinator : StandardHiddenRowCoordinator {
	public RowInfoCollectoinHiddenRowCoordinator(RowInfoCollection rows) : base(rows) {
		// when rows are removed, check if some of them were hidden
		// if yes, inform hidden row coordinator that they were removed
		rows.RowsRemoving += delegate(object sender, IndexRangeEventArgs e) {
			for (int i = 0; i < e.Count; i++) {
				var index = i + e.StartIndex;
				if (rows.IsRowVisible(index) == false)
					base.m_totalHiddenRows -= 1;
			}

			var range = new CellRange(e.StartIndex, 0, e.StartIndex + e.Count, 1);
			base.m_rowMerger.RemoveRange(range);
		};

	}
}
