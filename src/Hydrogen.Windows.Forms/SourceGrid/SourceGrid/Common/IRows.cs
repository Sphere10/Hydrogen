//-----------------------------------------------------------------------
// <copyright file="IRows.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Drawing;

namespace SourceGrid
{
	public interface IRows
	{
		bool IsRowVisible(int row);
		
		
		
		void HideRow(int row);
		void ShowRow(int row);
		
		/// <summary>
		/// Use this method to show or hide row
		/// </summary>
		/// <param name="row"></param>
		/// <param name="isVisible"></param>
		void ShowRow(int row, bool isVisible);
	}
}
