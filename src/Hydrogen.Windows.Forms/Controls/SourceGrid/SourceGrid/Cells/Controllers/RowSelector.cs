//-----------------------------------------------------------------------
// <copyright file="RowSelector.cs" company="Sphere 10 Software">
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
	public class RowSelector : ControllerBase
	{
		private readonly bool _raiseOnMouseDown;
		/// <summary>
		/// Default controller to select all the column
		/// </summary>
		public readonly static RowSelector Default = new RowSelector();
		public readonly static RowSelector SelectOnMouseDown = new RowSelector(true);

		public RowSelector()  : this(false){
			
		}

		public RowSelector(bool raiseOnMouseDown) {
			_raiseOnMouseDown = raiseOnMouseDown;
		}


		public override void OnMouseDown(CellContext sender, System.Windows.Forms.MouseEventArgs e) {
			if (_raiseOnMouseDown)
				RaiseEvent(sender);

			base.OnMouseDown(sender, e);
		}

		public override void OnClick(CellContext sender, EventArgs e)
		{
			base.OnClick (sender, e);
			if (!_raiseOnMouseDown)
				RaiseEvent(sender);
		}

		private void RaiseEvent(CellContext sender) {
			sender.Grid.Selection.SelectRow(sender.Position.Row, true);
		}
	}
}
