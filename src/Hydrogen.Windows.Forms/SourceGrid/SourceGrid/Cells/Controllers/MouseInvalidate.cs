// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;

namespace SourceGrid.Cells.Controllers;

/// <summary>
/// A behavior that invalidate the cell on mouse events
/// </summary>
public class MouseInvalidate : ControllerBase {
	/// <summary>
	/// Default implementation.
	/// </summary>
	public readonly static MouseInvalidate Default = new MouseInvalidate();

	/// <summary>
	/// Constructor
	/// </summary>
	public MouseInvalidate() {
	}

	public override void OnMouseDown(CellContext sender, MouseEventArgs e) {
		base.OnMouseDown(sender, e);

		sender.Grid.InvalidateCell(sender.Position);
	}

	public override void OnMouseUp(CellContext sender, MouseEventArgs e) {
		base.OnMouseUp(sender, e);

		sender.Grid.InvalidateCell(sender.Position);
	}

	public override void OnMouseEnter(CellContext sender, EventArgs e) {
		base.OnMouseEnter(sender, e);

		sender.Grid.InvalidateCell(sender.Position);
	}


	public override void OnMouseLeave(CellContext sender, EventArgs e) {
		base.OnMouseLeave(sender, e);

		sender.Grid.InvalidateCell(sender.Position);
	}
}
