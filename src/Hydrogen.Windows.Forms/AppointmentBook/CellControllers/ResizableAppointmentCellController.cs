// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;
using System.Windows.Forms;
using SourceGrid;

namespace Hydrogen.Windows.Forms.AppointmentBook;

internal class ResizableAppointmentCellController : BaseCellController {
	private readonly DevAge.Drawing.RectangleBorder _logicalBorder;
	private bool _resizing;

	internal ResizableAppointmentCellController(AppointmentBook owner, bool resizeTopBorderAllowed, bool resizeBottomBorderAllowed) : base(owner) {
		if (!resizeTopBorderAllowed && !resizeBottomBorderAllowed) {
			throw new ArgumentException("resizeTopBorderAllowed, resizeBottomBorderAllowed", "At least 1 argument must be true");
		}
		_logicalBorder = new DevAge.Drawing.RectangleBorder(new DevAge.Drawing.BorderLine(Color.Black, 5));
		_resizing = false;
		ResizeBorder1 = resizeTopBorderAllowed ? ResizedAppointmentBorder.Top : ResizedAppointmentBorder.Bottom;
		ResizeBorder2 = resizeBottomBorderAllowed ? ResizedAppointmentBorder.Bottom : ResizedAppointmentBorder.Top;
	}

	public new AppointmentBook Owner {
		get { return base.Owner as AppointmentBook; }
		set { base.Owner = value as AppointmentBook; }
	}

	public ResizedAppointmentBorder ResizeBorder1 { get; set; }

	public ResizedAppointmentBorder ResizeBorder2 { get; set; }

	public override void OnMouseDown(CellContext sender, MouseEventArgs e) {
		var cellRect = sender.Grid.PositionToRectangle(sender.Position);
		var mousePoint = new Point(e.X, e.Y);
		var cell = sender.Grid.PositionAtPoint(e.Location);
		var col = cell.Column;
		var row = cell.Row;
		Owner.TransformGridToModel(ref col, ref row);
		float distanceFromBorder;
		var initiateResize = false;
		var resizeBorderUsed = ResizedAppointmentBorder.Top;
		switch (_logicalBorder.GetPointPartType(cellRect, mousePoint, out distanceFromBorder)) {
			case DevAge.Drawing.RectanglePartType.TopBorder:
				if (ResizeBorder1 == ResizedAppointmentBorder.Top || ResizeBorder2 == ResizedAppointmentBorder.Top) {
					initiateResize = true;
					resizeBorderUsed = ResizedAppointmentBorder.Top;
				}
				break;
			case DevAge.Drawing.RectanglePartType.BottomBorder:
				if (ResizeBorder1 == ResizedAppointmentBorder.Bottom || ResizeBorder2 == ResizedAppointmentBorder.Bottom) {
					initiateResize = true;
					resizeBorderUsed = ResizedAppointmentBorder.Bottom;
				}
				break;
			default:
				base.OnMouseDown(sender, e);
				break;
		}
		if (initiateResize) {
			var appointmentViewModelAtColRow = Owner.ViewModel.GetAppointmentBlockAt(col, row);
			if (appointmentViewModelAtColRow != null && Owner.SelectedAppointment != appointmentViewModelAtColRow.AppointmentDataObject) {
				Owner.SelectAppointment(appointmentViewModelAtColRow.AppointmentDataObject);
			}
			_resizing = true;
			Owner.FireAppointmentResizingStarted(col, row, resizeBorderUsed);
			CurrentCursor = Cursors.HSplit;
		}
	}

	public override void OnMouseUp(CellContext sender, MouseEventArgs e) {
		if (_resizing) {
			_resizing = false;
			CurrentCursor = Cursors.Default;
			var cell = sender.Grid.PositionAtPoint(e.Location);
			var col = cell.Column;
			var row = cell.Row;
			Owner.TransformGridToModel(ref col, ref row);
			Owner.FireAppointmentResizingFinished();
		} else {
			base.OnMouseUp(sender, e);
		}
	}

	public override void OnMouseMove(CellContext sender, MouseEventArgs e) {

		if (_resizing) {
			var cell = sender.Grid.PositionAtPoint(e.Location);
			var col = cell.Column;
			var row = cell.Row;
			Owner.TransformGridToModel(ref col, ref row);
			if (Owner.CanSelect(col, row)) {
				Owner.FireAppointmentResizing(col, row);
			}
			return;
		}

		var cellRect = sender.Grid.PositionToRectangle(sender.Position);
		if (cellRect.IsEmpty)
			return;

		var mousePoint = new Point(e.X, e.Y);

		float distanceFromBorder;
		switch (_logicalBorder.GetPointPartType(cellRect, mousePoint, out distanceFromBorder)) {
			case DevAge.Drawing.RectanglePartType.TopBorder:
				if (ResizeBorder1 == ResizedAppointmentBorder.Top || ResizeBorder2 == ResizedAppointmentBorder.Top)
					CurrentCursor = Cursors.HSplit;
				break;
			case DevAge.Drawing.RectanglePartType.BottomBorder:
				if (ResizeBorder1 == ResizedAppointmentBorder.Bottom || ResizeBorder2 == ResizedAppointmentBorder.Bottom)
					CurrentCursor = Cursors.HSplit;
				break;
			default:
				CurrentCursor = Cursors.Default;
				base.OnMouseMove(sender, e);
				break;
		}
	}

	public override void OnMouseLeave(CellContext sender, EventArgs e) {
		if (!_resizing) {
			CurrentCursor = Cursors.Default;
			base.OnMouseLeave(sender, e);
		}
	}

}
