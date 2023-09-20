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

	public virtual bool SupportsNull => false;

	public virtual bool IsConstantLength => false;

	public virtual long ConstantLength => -1;

	public virtual long CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes) {
		var sizes = items.Select(item => CalculateSize(item)).ToArray();
		itemSizes = calculateIndividualItems ? sizes.ToArray() : null;
		return sizes.Sum();
	}

	public abstract long CalculateSize(TItem item);

}
