// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid;

/// <summary>
/// This class implements a RowsBase class using always the same Height for all rows. Using this class you must only implement the Count method.
/// </summary>
public abstract class RowsSimpleBase : RowsBase {
	public RowsSimpleBase(GridVirtual grid) : base(grid) {
		mRowHeight = grid.DefaultHeight;
	}

	private int mRowHeight;

	public int RowHeight {
		get { return mRowHeight; }
		set {
			if (mRowHeight != value) {
				mRowHeight = value;
				PerformLayout();
			}
		}
	}

	public override int GetHeight(int row) {
		return RowHeight;
	}
	public override void SetHeight(int row, int height) {
		RowHeight = height;
	}
}
