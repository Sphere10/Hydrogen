// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

// ReSharper disable PossibleInvalidOperationException
public class StaticSizeItemSizer<TItem> : IItemSizer<TItem> {

	public StaticSizeItemSizer(long staticSize) {
		Guard.ArgumentInRange(staticSize, 0, int.MaxValue, nameof(staticSize));
		StaticSize = staticSize;
	}

	public bool IsStaticSize => true;

	public long StaticSize { get; }

	public long CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes) {
		return CalculateTotalSize(items.Count(), calculateIndividualItems, out itemSizes);
	}

	public long CalculateTotalSize(long itemsCount, bool calculateIndividualItems, out long[] itemSizes) {
		var val = StaticSize;
		var size = itemsCount * val;
		itemSizes = calculateIndividualItems ? Tools.Array.Gen(itemsCount, val) : null;
		return size;
	}

	public long CalculateSize(TItem item) => StaticSize;
}
