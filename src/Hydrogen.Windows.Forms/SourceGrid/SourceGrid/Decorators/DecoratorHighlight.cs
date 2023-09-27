// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid.Decorators;

public class DecoratorHighlight : DecoratorBase {
	private CellRange mRange = CellRange.Empty;

	/// <summary>
	/// Gets or sets the range to draw
	/// </summary>
	public CellRange Range {
		get { return mRange; }
		set { mRange = value; }
	}


	public override bool IntersectWith(CellRange range) {
		return Range.IntersectsWith(range);
	}

	public override void Draw(RangePaintEventArgs e) {
	}
}
