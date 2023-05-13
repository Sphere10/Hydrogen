// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;

namespace Hydrogen {

	// ReSharper disable PossibleInvalidOperationException

	public class StaticSizeItemSizer<TItem> : IItemSizer<TItem> {

		public StaticSizeItemSizer(int staticSize) {
			Guard.ArgumentInRange(staticSize, 0, int.MaxValue, nameof(staticSize));
			StaticSize = staticSize;
		}

		public bool IsStaticSize => true;

		public int StaticSize { get; }

		public int CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out int[] itemSizes) {
			return CalculateTotalSize(items.Count(), calculateIndividualItems, out itemSizes);
		}

		public int CalculateTotalSize(int itemsCount, bool calculateIndividualItems, out int[] itemSizes) {
			var val = StaticSize;
			var size = itemsCount * val;
			itemSizes = calculateIndividualItems ? Tools.Array.Gen(itemsCount, val) : null;
			return size;
		}

		public int CalculateSize(TItem item) => StaticSize;
	}

}