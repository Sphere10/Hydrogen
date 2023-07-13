// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Drawing;
using System.Windows.Forms;
using SourceGrid;

namespace Hydrogen.Windows.Forms;

internal class CustomSortHeaderCellController : SourceGrid.Cells.Controllers.ControllerBase {
	private readonly CrudGrid _crudGrid;

	public CustomSortHeaderCellController(CrudGrid crudGrid) {
		_crudGrid = crudGrid;
	}


	public override void OnMouseUp(CellContext sender, MouseEventArgs e) {
		base.OnMouseUp(sender, e);
		var cellRect = sender.Grid.PositionToRectangle(sender.Position);
		var mousePoint = new Point(e.X, e.Y);
		var cell = sender.Grid.PositionAtPoint(e.Location);
		var col = cell.Column;
		var row = cell.Row;

		var _logicalBorder = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(Color.Black, 4));
		float distanceFromBorder;
		switch (_logicalBorder.GetPointPartType(cellRect, mousePoint, out distanceFromBorder)) {
			case DevAge.Drawing.RectanglePartType.ContentArea:
				_crudGrid._grid_SortColumnPressed(sender.Position.Column);
				break;

		}
	}

}
