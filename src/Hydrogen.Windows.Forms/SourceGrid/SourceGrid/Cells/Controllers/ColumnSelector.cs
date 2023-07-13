// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace SourceGrid.Cells.Controllers;

/// <summary>
/// Summary description for FullColumnSelection.
/// </summary>
public class ColumnSelector : ControllerBase {
	/// <summary>
	/// Default controller to select all the column
	/// </summary>
	public readonly static ColumnSelector Default = new ColumnSelector();

	public ColumnSelector() {
	}

	public override void OnClick(CellContext sender, EventArgs e) {
		base.OnClick(sender, e);

		sender.Grid.Selection.SelectColumn(sender.Position.Column, true);
	}
}
