//-----------------------------------------------------------------------
// <copyright file="RowInfoCollectoinHiddenRowCoordinator.cs" company="Sphere 10 Software">
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

namespace SourceGrid
{
	public class RowInfoCollectoinHiddenRowCoordinator : StandardHiddenRowCoordinator
	{
		public RowInfoCollectoinHiddenRowCoordinator(RowInfoCollection rows) : base(rows)
		{
			// when rows are removed, check if some of them were hidden
			// if yes, inform hidden row coordinator that they were removed
			rows.RowsRemoving += delegate(object sender, IndexRangeEventArgs e)
			{
				for (int i = 0 ; i < e.Count; i ++)
				{
					var index = i + e.StartIndex;
					if (rows.IsRowVisible(index) == false)
						base.m_totalHiddenRows -= 1;
				}
				
				var range = new CellRange(e.StartIndex, 0, e.StartIndex + e.Count, 1);
				base.m_rowMerger.RemoveRange(range);
			};
			
		}
	}
}
