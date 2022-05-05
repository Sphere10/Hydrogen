//-----------------------------------------------------------------------
// <copyright file="RangeCollection.cs" company="Sphere 10 Software">
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
	/// <summary>
	/// A collection of elements of type Range
	/// </summary>
	[Serializable]
	public class RangeCollection : List<CellRange>
	{
		/// <summary>
		/// Returns true if the specified cell position is present in any range in the current collection.
		/// </summary>
		/// <param name="p_Position"></param>
		/// <returns></returns>
		public bool ContainsCell(Position p_Position)
		{
			foreach(CellRange range in this)
			{
				if ( range.Contains(p_Position) )
					return true;
			}
			return false;
		}
	}
}
