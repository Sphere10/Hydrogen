// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Windows.Forms;
using SourceGrid.Cells.Controllers;

namespace Hydrogen.Windows.Forms.AppointmentBook;

internal class BaseCellController : SourceGrid.Cells.Controllers.ControllerBase {
	private Cursor _previousCursor;
	private MouseCursor _cursor;

	internal BaseCellController(BaseAppointmentBook owner) {
		Owner = owner;
		_previousCursor = null;
		_cursor = null;
	}

	protected BaseAppointmentBook Owner { get; set; }

	protected Cursor CurrentCursor {
		get { return Owner.Cursor; }
		set { Owner.Cursor = value; }
	}

	public override void OnFocusEntering(SourceGrid.CellContext sender, System.ComponentModel.CancelEventArgs e) {
		base.OnFocusEntering(sender, e);
		e.Cancel = !CanReceiveFocus(sender, e);
	}

	#region Focus

	public override bool CanReceiveFocus(SourceGrid.CellContext sender, EventArgs e) {
		return false;
	}

	#endregion

	#region Editing

	public override void OnEditStarting(SourceGrid.CellContext sender, CancelEventArgs e) {
		e.Cancel = true;
	}

	#endregion

	#region Keyboard

	public override void OnKeyDown(SourceGrid.CellContext sender, KeyEventArgs e) {
		base.OnKeyDown(sender, e);
	}

	public override void OnKeyUp(SourceGrid.CellContext sender, KeyEventArgs e) {
		base.OnKeyUp(sender, e);
	}

	public override void OnKeyPress(SourceGrid.CellContext sender, KeyPressEventArgs e) {
		base.OnKeyPress(sender, e);
	}

	#endregion

	#region Mouse

	public override void OnMouseDown(SourceGrid.CellContext sender, MouseEventArgs e) {
		var position = Owner._grid.PositionAtPoint(e.Location);
		int col = position.Column;
		int row = position.Row;
		Owner.TransformGridToModel(ref col, ref row);
		Owner.FireMouseDown(col, row, e);
	}

	public override void OnMouseUp(SourceGrid.CellContext sender, MouseEventArgs e) {
		var position = Owner._grid.PositionAtPoint(e.Location);
		int col = position.Column;
		int row = position.Row;
		Owner.TransformGridToModel(ref col, ref row);
		Owner.FireMouseUp(col, row, e);
	}

	public override void OnMouseEnter(SourceGrid.CellContext sender, EventArgs e) {
		int col = sender.Position.Column;
		int row = sender.Position.Row;
		Owner.TransformGridToModel(ref col, ref row);
		Owner.FireMouseEnter(col, row);
	}

	public override void OnMouseLeave(SourceGrid.CellContext sender, EventArgs e) {
		int col = sender.Position.Column;
		int row = sender.Position.Row;
		Owner.TransformGridToModel(ref col, ref row);
		Owner.FireMouseLeave(col, row);
	}


	public override void OnMouseMove(SourceGrid.CellContext sender, MouseEventArgs e) {
		var position = Owner._grid.PositionAtPoint(e.Location);
		int col = position.Column;
		int row = position.Row;
		Owner.TransformGridToModel(ref col, ref row);
		Owner.FireMouseMove(col, row, e);
	}

	public override void OnClick(SourceGrid.CellContext sender, EventArgs e) {
		int col = sender.Position.Column;
		int row = sender.Position.Row;
		Owner.TransformGridToModel(ref col, ref row);
		Owner.FireClick(col, row);
	}

	public override void OnDoubleClick(SourceGrid.CellContext sender, EventArgs e) {
		int col = sender.Position.Column;
		int row = sender.Position.Row;
		Owner.TransformGridToModel(ref col, ref row);
		Owner.FireDoubleClick(col, row);
	}

	public override void OnDragDrop(SourceGrid.CellContext sender, DragEventArgs e) {
		base.OnDragDrop(sender, e);
	}

	public override void OnDragEnter(SourceGrid.CellContext sender, DragEventArgs e) {
		base.OnDragEnter(sender, e);
	}

	public override void OnDragOver(SourceGrid.CellContext sender, DragEventArgs e) {
		base.OnDragOver(sender, e);
	}

	public override void OnDragLeave(SourceGrid.CellContext sender, EventArgs e) {
		base.OnDragLeave(sender, e);
	}

	#endregion

}
