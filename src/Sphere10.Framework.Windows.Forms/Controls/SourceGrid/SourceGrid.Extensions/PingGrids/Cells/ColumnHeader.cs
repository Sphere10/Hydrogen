//-----------------------------------------------------------------------
// <copyright file="ColumnHeader.cs" company="Sphere 10 Software">
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

namespace SourceGrid.Extensions.PingGrids.Cells
{
	/// <summary>
	/// A cell header used for the columns. Usually used in the HeaderCell property of a DataGridColumn.
	/// </summary>
	public class ColumnHeader : SourceGrid.Cells.Virtual.ColumnHeader
	{
	    public ColumnHeader(string pCaption)
	    {
	        Model.AddModel(new SourceGrid.Cells.Models.ValueModel(pCaption));
	    }
	}
}
