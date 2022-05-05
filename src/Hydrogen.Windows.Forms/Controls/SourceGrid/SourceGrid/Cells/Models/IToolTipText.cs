//-----------------------------------------------------------------------
// <copyright file="IToolTipText.cs" company="Sphere 10 Software">
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

namespace SourceGrid.Cells.Models
{
	/// <summary>
	/// Interface for informations about a tooltiptext
	/// </summary>
	public interface IToolTipText : IModel
	{
		/// <summary>
		/// Get the tooltiptext of the specified cell
		/// </summary>
		/// <param name="cellContext"></param>
		string GetToolTipText(CellContext cellContext);
	}
}
