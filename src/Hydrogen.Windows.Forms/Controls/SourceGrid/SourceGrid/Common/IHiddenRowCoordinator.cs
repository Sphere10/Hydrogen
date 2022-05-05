//-----------------------------------------------------------------------
// <copyright file="IHiddenRowCoordinator.cs" company="Sphere 10 Software">
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

namespace SourceGrid
{
	public interface IHiddenRowCoordinator
	{
		/// <summary>
		/// Returns a sequence of row indexes which are visible only.
		/// Correctly handles invisible rows.
		/// </summary>
		/// <param name="rowIndex"></param>
		/// <param name="numberOfRowsToProduce">How many visible rows to return</param>
		/// <returns>Can return less rows than requested. This might occur
		/// if you request to return visible rows in the end of the grid,
		/// and all the rows would be hidden. In that case no indexes would be returned
		/// at all, even though specific amount of rows was requested</returns>
		IEnumerable<int> LoopVisibleRows(int rowIndex, int numberOfRowsToProduce);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="scrollBarValue">The value of the vertical scroll bar. Note that
		/// this does not directly relate to row number. If there are no hidden rows at all,
		/// then scroll bar value directly relates to row index number. However,
		/// if some rows are hidden, then this value is different.
		/// Basically, it says how many visible rows must be scrolled down</param>
		/// <returns></returns>
		int ConvertScrollbarValueToRowIndex(int scrollBarValue);
		
		int GetTotalHiddenRows();
		
	}
}
