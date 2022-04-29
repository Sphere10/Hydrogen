//-----------------------------------------------------------------------
// <copyright file="PingGridRowHeaderModel.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using DevAge.ComponentModel;
using SourceGrid.Cells.Models;
using SourceGrid.Selection;

namespace SourceGrid.Extensions.PingGrids
{
	public class PingGridRowHeaderModel : IValueModel
	{
		public PingGridRowHeaderModel()
		{
		}
		#region IValueModel Members
		public object GetValue(CellContext cellContext)
		{
			DataGrid dataGrid = (DataGrid)cellContext.Grid;
			if (dataGrid.DataSource != null &&
			    dataGrid.DataSource.AllowNew &&
			    cellContext.Position.Row == (dataGrid.Rows.Count - 1))
				return "*";
			else
				return null;
		}

		public void SetValue(CellContext cellContext, object p_Value)
		{
			throw new ApplicationException("Not supported");
		}
		#endregion
	}

}
