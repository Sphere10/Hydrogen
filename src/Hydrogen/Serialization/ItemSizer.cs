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

public abstract class ItemSizer<TItem> : IItemSizer<TItem> {

	public bool IsStaticSize => false;

	public int StaticSize => -1;

	public virtual int CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out int[] itemSizes) {
		var sizes = items.Select(CalculateSize).ToArray();
		itemSizes = calculateIndividualItems ? sizes.ToArray() : null;
		return sizes.Sum();
	}

	public abstract int CalculateSize(TItem item);

}
