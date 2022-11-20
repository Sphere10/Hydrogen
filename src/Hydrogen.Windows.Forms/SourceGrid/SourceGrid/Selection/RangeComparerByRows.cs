//-----------------------------------------------------------------------
// <copyright file="RangeComparerByRows.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Reflection;

namespace SourceGrid.Selection
{
	public class RangeComparerByRows : IComparer<CellRange>
	{
		public int Compare(CellRange x, CellRange y)
		{
			if (x.Start.Row == y.Start.Row)
				return 0;
			if (x.Start.Row > y.Start.Row)
				return 1;
			return -1;
		}
	}
}

