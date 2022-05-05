//-----------------------------------------------------------------------
// <copyright file="ValueCellComparer.cs" company="Sphere 10 Software">
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
using System.Collections;

namespace SourceGrid
{
	/// <summary>
	/// A comparer for the Cell class. (Not for CellVirtual). Using the value of the cell.
	/// </summary>
	public class ValueCellComparer : IComparer
	{
		public virtual System.Int32 Compare ( System.Object x , System.Object y )
		{
			//Cell object
			if (x==null && y==null)
				return 0;
			if (x==null)
				return -1;
			if (y==null)
				return 1;
	
			if (x is IComparable)
			{
				if (x.GetType().Equals(y.GetType()) == false)
					return -1;
				return ((IComparable)x).CompareTo(y);
			}
			if (y is IComparable)
			{
				if (x.GetType().Equals(y.GetType()) == false)
					return -1;
				return (-1* ((IComparable)y).CompareTo(x));
			}
	
			//Cell.Value object
			object vx = ((Cells.ICell)x).Value;
			object vy = ((Cells.ICell)y).Value;
			if (vx==null && vy==null)
				return 0;
			if (vx==null)
				return -1;
			if (vy==null)
				return 1;
	
			if (vx is IComparable)
			{
				if (vx.GetType().Equals(vy.GetType()) == false)
					return -1;
				return ((IComparable)vx).CompareTo(vy);
			}
			if (vy is IComparable)
			{
				if (vx.GetType().Equals(vy.GetType()) == false)
					return -1;
				return (-1* ((IComparable)vy).CompareTo(vx));
			}
	
			throw new ArgumentException("Invalid cell object, no IComparable interface found");
		}
	}
}
