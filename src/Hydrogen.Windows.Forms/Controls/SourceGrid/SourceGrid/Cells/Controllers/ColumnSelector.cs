//-----------------------------------------------------------------------
// <copyright file="ColumnSelector.cs" company="Sphere 10 Software">
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

namespace SourceGrid.Cells.Controllers
{
	/// <summary>
	/// Summary description for FullColumnSelection.
	/// </summary>
	public class ColumnSelector : ControllerBase
	{
		/// <summary>
		/// Default controller to select all the column
		/// </summary>
		public readonly static ColumnSelector Default = new ColumnSelector();

		public ColumnSelector()
		{
		}

		public override void OnClick(CellContext sender, EventArgs e)
		{
			base.OnClick (sender, e);

			sender.Grid.Selection.SelectColumn(sender.Position.Column, true);
		}
	}
}
