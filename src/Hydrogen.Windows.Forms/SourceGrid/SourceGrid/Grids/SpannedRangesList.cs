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
/// A is simple List of Ranges.
/// Uses simple iterating over list to find
/// required range
/// </summary>
public class SpannedRangesList : List<CellRange>, ISpannedRangesCollection {
	public void Update(CellRange oldRange, CellRange newRange) {
		int index = base.IndexOf(oldRange);
		if (index < 0)
			throw new RangeNotFoundException();
		this[index] = newRange;
	}

	public void Redim(int rowCount, int colCount) {
		// just do nothing, nothing needed
	}

	public new void Remove(CellRange range) {
		int index = base.IndexOf(range);
		if (index < 0)
			throw new RangeNotFoundException();
		base.RemoveAt(index);
	}


	/// <summary>
	/// Returns first intersecting region
	/// </summary>
	/// <param name="pos"></param>
	public CellRange? GetFirstIntersectedRange(Position pos) {
		for (int i = 0; i < this.Count; i++) {
			var range = this[i];
			if (range.Contains(pos))
				return range;
		}
		return null;
	}

	public List<CellRange> GetRanges(CellRange range) {
		var result = new List<CellRange>();
		for (int i = 0; i < this.Count; i++) {
			var r = this[i];
			if (r.Contains(range))
				result.Add(r);
		}
		return result;
	}

	public CellRange? FindRangeWithStart(Position start) {
		for (int i = 0; i < this.Count; i++) {
			var range = this[i];
			if (range.Start.Equals(start))
				return range;
		}
		return null;
	}
}
