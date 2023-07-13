// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid.Decorators;

public class DecoratorSelection : DecoratorBase {
	public DecoratorSelection(Selection.SelectionBase selection) {
		mSelection = selection;
	}

	private Selection.SelectionBase mSelection;

	public override bool IntersectWith(CellRange range) {
		return mSelection.IntersectsWith(range);
	}

	public override void Draw(RangePaintEventArgs e) {
		RangeRegion region = mSelection.GetSelectionRegion();

		if (region.IsEmpty())
			return;

		// get visible range for scrollable area
		CellRange visibleScrollabeRange = mSelection.Grid.RangeAtAreaExpanded(CellPositionType.Scrollable);


		System.Drawing.Brush brush = e.GraphicsCache.BrushsCache.GetBrush(mSelection.BackColor);

		CellContext focusContext = new CellContext(e.Grid, mSelection.ActivePosition);
		// get focus rectangle
		// clipped to visible range
		CellRange focusClippedRange = visibleScrollabeRange.Intersect(new CellRange(mSelection.ActivePosition, mSelection.ActivePosition));
		System.Drawing.Rectangle focusRect = e.Grid.PositionToRectangle(focusClippedRange.Start);

		//Draw each selection range
		foreach (CellRange rangeToLoop in region) {
			// intersect given range with visible range
			// this way we ensure we don't loop through thousands
			// of rows to calculate rectToDraw
			CellRange rng = visibleScrollabeRange.Intersect(rangeToLoop);


			System.Drawing.Rectangle rectToDraw = e.Grid.RangeToRectangle(rng);
			if (rectToDraw == System.Drawing.Rectangle.Empty)
				continue;

			System.Drawing.Region regionToDraw = new System.Drawing.Region(rectToDraw);

			if (rectToDraw.IntersectsWith(focusRect))
				regionToDraw.Exclude(focusRect);

			e.GraphicsCache.Graphics.FillRegion(brush, regionToDraw);

			//Draw the border only if there isn't a editing cell
			// and is the range that contains the focus or there is a single range
			if (rng.Contains(mSelection.ActivePosition) || region.Count == 1) {
				if (focusContext.IsEditing() == false)
					mSelection.Border.Draw(e.GraphicsCache, rectToDraw);
			}
		}

		//Draw Focus
		System.Drawing.Brush brushFocus = e.GraphicsCache.BrushsCache.GetBrush(mSelection.FocusBackColor);
		e.GraphicsCache.Graphics.FillRectangle(brushFocus, focusRect);
	}
}
