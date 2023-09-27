// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid.Selection;

public class DisabledSelection : SelectionBase {
	public override bool IsSelectedColumn(int column) {
		return false;
	}

	public override void SelectRow(int row, bool @select) {
	}

	public override bool IsSelectedCell(Position position) {
		return false;
	}

	public override bool IsEmpty() {
		return true;
	}

	public override RangeRegion GetSelectionRegion() {
		return new RangeRegion();
	}

	public override bool IntersectsWith(CellRange rng) {
		return false;
	}

	public override void SelectRange(CellRange range, bool @select) {
	}

	protected override void OnResetSelection() {
	}

	public override void SelectCell(Position position, bool @select) {
	}

	public override bool IsSelectedRange(CellRange range) {
		return false;
	}

	public override void SelectColumn(int column, bool @select) {
	}

	public override bool IsSelectedRow(int row) {
		return false;
	}
}
