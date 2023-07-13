// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace SourceGrid;

/// <summary>
/// This interface helps work with spanned ranges collection
/// 
/// There are two implementations at the moment.
/// One is SpannedRangesList,
/// another is QuadTree implementation, which is much faster.
/// Look at unit tests for speed comparisons:
/// TestSpannedCellRnages_Performance: TestBoth.
/// </summary>
public interface ISpannedRangesCollection {
	/// <summary>
	/// Returns the number of ranges contained
	/// </summary>
	int Count { get; }

	void Add(CellRange range);

	/// <summary>
	/// Searches for an old range. If finds, updates 
	/// found region. Else throws RangeNotFoundException
	/// </summary>
	void Update(CellRange oldRange, CellRange newRange);

	/// <summary>
	/// Increase size up to specified values.
	/// Note that shrinking is not possible
	/// </summary>
	/// <param name="rowCount"></param>
	/// <param name="colCount"></param>
	void Redim(int rowCount, int colCount);

	/// <summary>
	/// If does not find, throws RangeNotFoundException
	/// </summary>
	void Remove(CellRange range);

	CellRange? GetFirstIntersectedRange(Position pos);

	List<CellRange> GetRanges(CellRange range);

	/// <summary>
	/// Returns range which has exactly the same start position
	/// as indicated
	/// </summary>
	/// <param name="start"></param>
	/// <returns></returns>
	CellRange? FindRangeWithStart(Position start);

	CellRange[] ToArray();
}
