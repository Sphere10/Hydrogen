//-----------------------------------------------------------------------
// <copyright file="ColumnsSimpleBase.cs" company="Sphere 10 Software">
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
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

namespace SourceGrid
{
	/// <summary>
	/// This class implements a RowsBase class using always the same Height for all rows. Using this class you must only implement the Count method.
	/// </summary>
	public abstract class ColumnsSimpleBase : ColumnsBase
	{
		public ColumnsSimpleBase(GridVirtual grid):base(grid)
		{
			mColumnWidth = grid.DefaultWidth;
		}
	
		private int mColumnWidth;
		public int ColumnWidth
		{
			get{return mColumnWidth;}
			set
			{
				if (mColumnWidth != value)
				{
					mColumnWidth = value;
					PerformLayout();
				}
			}
		}
	
		public override int GetWidth(int column)
		{
			return ColumnWidth;
		}
		public override void SetWidth(int column, int width)
		{
			ColumnWidth = width;
		}
		public override bool IsColumnVisible(int column)
		{
			return true;
		}
		public override void HideColumn(int column)
		{
			throw new NotSupportedException("ColumnsSimpleBase does not support column hiding");
		}
		public override void ShowColumn(int column)
		{
			return;
		}
	}
}
