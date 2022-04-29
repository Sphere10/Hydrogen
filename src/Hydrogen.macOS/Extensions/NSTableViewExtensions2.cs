//-----------------------------------------------------------------------
// <copyright file="NSTableViewExtensions.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Data;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Drawing;
using Hydrogen;

namespace Hydrogen.Data {
	public static class NSTableViewExtensions
	{

		public static void SetDataSourceEx(this NSTableView tableView, DataTable dataTable) {
			tableView.RemoveAllColumns();
			//dataTable.Columns[0].ColumnMapping = MappingType.Hidden;
			dataTable.Columns.Cast<DataColumn>().ForEach(c => {
				var col = new NSTableColumn();
				col.HeaderCell.Title = c.ColumnName;
				col.HeaderToolTip = c.ColumnName;
				col.Hidden = c.ColumnMapping == MappingType.Hidden;
				tableView.AddColumn(col);
			});
			tableView.SizeLastColumnToFit();
			tableView.DataSource = new ADOTableViewDataSource(dataTable);
			tableView.ReloadData();
		}
	

	}

}




