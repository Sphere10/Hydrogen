//-----------------------------------------------------------------------
// <copyright file="Cell.cs" company="Sphere 10 Software">
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
using SourceGrid.Cells;
using SourceGrid.Cells.Virtual;

namespace SourceGrid.Extensions.PingGrids.Cells
{
	public class Cell : CellVirtual
	{
	        public Cell()
		{
	            Model.AddModel(new PingGridValueModel());
		}
	
	        public static ICellVirtual Create(Type type, bool editable)
	        {
	            ICellVirtual cell;
	
	            if (type == typeof(bool))
	                cell = new SourceGrid.Extensions.PingGrids.Cells.CheckBox();
	            else
	            {
	                cell = new SourceGrid.Extensions.PingGrids.Cells.Cell();
	                cell.Editor = SourceGrid.Cells.Editors.Factory.Create(type);
	            }
	
	            if (cell.Editor != null) //Can be null for special DataType like Object
	            {
	                //The columns now support always DbNull values because the validation is done at row level by the DataTable itself.
	                cell.Editor.AllowNull = true;
	                cell.Editor.EnableEdit = editable;
	            }
	
	            return cell;
	        }
	}
}
