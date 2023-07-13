// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using SourceGrid.Cells.Models;

namespace SourceGrid.Extensions.PingGrids;

public class PingGridRowHeaderModel : IValueModel {
	public PingGridRowHeaderModel() {
	}

	#region IValueModel Members

	public object GetValue(CellContext cellContext) {
		DataGrid dataGrid = (DataGrid)cellContext.Grid;
		if (dataGrid.DataSource != null &&
		    dataGrid.DataSource.AllowNew &&
		    cellContext.Position.Row == (dataGrid.Rows.Count - 1))
			return "*";
		else
			return null;
	}

	public void SetValue(CellContext cellContext, object p_Value) {
		throw new ApplicationException("Not supported");
	}

	#endregion

}
