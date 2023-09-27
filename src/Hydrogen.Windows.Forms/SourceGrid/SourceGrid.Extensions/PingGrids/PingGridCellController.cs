// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel;
using SourceGrid.Cells.Controllers;

namespace SourceGrid.Extensions.PingGrids;

#region Controller

/// <summary>
/// Notify PingGrid of value editing
/// </summary>
public class PingGridCellController : ControllerBase {
	public override void OnValueChanging(CellContext sender, ValueChangeEventArgs e) {
		base.OnValueChanging(sender, e);

	}

	public override void OnEditStarting(CellContext sender, CancelEventArgs e) {
		base.OnEditStarting(sender, e);

	}
}

#endregion
