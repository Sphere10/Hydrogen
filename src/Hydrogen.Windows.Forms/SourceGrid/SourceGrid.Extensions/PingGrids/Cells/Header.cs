// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using SourceGrid.Cells.Models;

namespace SourceGrid.Extensions.PingGrids.Cells;

/// <summary>
/// A cell used for the top/left cell when using DataGridRowHeader.
/// </summary>
public class Header : SourceGrid.Cells.Virtual.Header {
	public Header() {
		Model.AddModel(new NullValueModel());
	}
}
