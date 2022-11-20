//-----------------------------------------------------------------------
// <copyright file="Button.cs" company="Sphere 10 Software">
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
using System.Windows.Forms;


namespace SourceGrid.Cells.Virtual
{
	/// <summary>
	/// A cell that rappresent a button 
	/// </summary>
	public abstract class Button : CellVirtual
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public Button()
		{
			View = Views.Button.Default;
			AddController(Controllers.MouseInvalidate.Default);
			//NOTE: I don't add the Unselectable controller because the Execute event of the Button it is not fired when the cell is not selected
		}
	}
}

namespace SourceGrid.Cells
{
	/// <summary>
	/// A cell that rappresent a button 
	/// </summary>
	public class Button : Cell
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="p_Value"></param>
		public Button(object p_Value):base(p_Value)
		{
			View = Views.Button.Default;
			AddController(Controllers.MouseInvalidate.Default);
			//NOTE: I don't add the Unselectable controller because the Execute event of the Button it is not fired when the cell is not selected
		}
	}
}
