// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid.Cells.Controllers;

/// <summary>
/// ColumnFocus controller overrides the OnFocusEntering method and set the Focus on the Column. This controller is usually used on the Column Header when the Sort is disabled.
/// </summary>
public class ColumnFocus : ControllerBase {
	/// <summary>
	/// Default controller to select all the column
	/// </summary>
	public readonly static ColumnFocus Default = new ColumnFocus();

	public ColumnFocus() {
	}

	//Non uso questo evento altrimenti non verrebbero applicate le regole di selezione del tasto Shift
//		public override void OnClick(PositionEventArgs e)
//		{
//			base.OnClick (e);
//
//			sender.Grid.Columns[sender.Position.Column].Focus();
//		}

	public override void OnFocusEntering(CellContext sender, System.ComponentModel.CancelEventArgs e) {
		base.OnFocusEntering(sender, e);

		sender.Grid.Selection.FocusColumn(sender.Position.Column);
	}
}
