// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Drawing;
using System.Windows.Forms;
using DevAge.Drawing;

namespace Hydrogen.Windows.Forms.AppointmentBook;

/// <summary>
/// Customized View to draw a rounded background
/// </summary>
internal class CellView : SourceGrid.Cells.Views.Cell {
	public CellView(BaseAppointmentBook parent, CellViewModel cellDisplay, bool selected = false) {
		TextAlignment = DevAge.Drawing.ContentAlignment.MiddleLeft;
		Border = DevAge.Drawing.RectangleBorder.NoBorder;
		base.BackColor = cellDisplay.BackColor;
		base.ForeColor = cellDisplay.TextColor;
		base.Font = new Font(base.Font ?? Control.DefaultFont, cellDisplay.FontStyle);
		base.Padding = new DevAge.Drawing.Padding(selected ? 0 : 1);
		var border = new RectangleBorder {
			Top = BorderLine.NoBorder,
			Left = BorderLine.NoBorder,
			Right = BorderLine.NoBorder,
			Bottom = BorderLine.NoBorder
		};

		if (cellDisplay.Traits.HasFlag(CellTraits.Filled)) {
			border.Left = selected ? parent.SelectedAppointmentBorderLine : parent.AppointmentBorderLine;
			border.Right = selected ? parent.SelectedAppointmentBorderLine : parent.AppointmentBorderLine;
		}
		if (cellDisplay.Traits.HasFlag(CellTraits.Top)) {
			border.Top = selected ? parent.SelectedAppointmentBorderLine : parent.AppointmentBorderLine;
		}
		if (cellDisplay.Traits.HasFlag(CellTraits.Bottom)) {
			border.Bottom = selected ? parent.SelectedAppointmentBorderLine : parent.AppointmentBorderLine;
		}

		base.Border = border;
	}

}
