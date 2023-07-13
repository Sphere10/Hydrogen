// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using DevAge.Drawing;

namespace Hydrogen.Windows.Forms.AppointmentBook;

/// <summary>
/// Customized View to draw a rounded background
/// </summary>
internal class EmptyCellView : SourceGrid.Cells.Views.Cell {
	public EmptyCellView(BaseAppointmentBook parent) {
		Border = new RectangleBorder {
			Top = BorderLine.NoBorder,
			Left = BorderLine.NoBorder,
			Right = BorderLine.NoBorder,
			Bottom = parent.HorizontalBorderLine
		};
	}
}
