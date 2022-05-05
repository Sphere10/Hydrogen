//-----------------------------------------------------------------------
// <copyright file="ISortableHeader.cs" company="Sphere 10 Software">
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
	/// Summary description for ICellSortableHeader.
	/// </summary>
	public interface ISortableHeader : IModel
	{
		/// <summary>
		/// Returns the current sort status
		/// </summary>
		/// <param name="cellContext"></param>
		/// <returns></returns>
		SortStatus GetSortStatus(CellContext cellContext);

		/// <summary>
		/// Set the current sort mode
		/// </summary>
		/// <param name="cellContext"></param>
		/// <param name="pStyle"></param>
        void SetSortMode(CellContext cellContext, DevAge.Drawing.HeaderSortStyle pStyle);
	}

	public struct SortStatus
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="p_Style">Status of current sort.</param>
        public SortStatus(DevAge.Drawing.HeaderSortStyle p_Style)
		{
			Style = p_Style;
			Comparer = null;
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="p_Style">Status of current sort.</param>
		/// <param name="p_Comparer">Comparer used to sort the column. The comparer will take 2 Cell. If null the default ValueCellComparer is used.</param>
        public SortStatus(DevAge.Drawing.HeaderSortStyle p_Style, System.Collections.IComparer p_Comparer)
            : this(p_Style)
		{
			Comparer = p_Comparer;
		}
        public DevAge.Drawing.HeaderSortStyle Style;

		public System.Collections.IComparer Comparer;
	}
}

