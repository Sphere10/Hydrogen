// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public abstract class ItemSizer<TItem> : IItemSizer<TItem> {

	public virtual bool SupportsNull => false;

	public virtual bool IsConstantSize => false;

	public virtual long ConstantSize => -1;

	public virtual long CalculateTotalSize(SerializationContext context, IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes) {
		var sizes = items.Select(item => CalculateSize(context, item)).ToArray();
		itemSizes = calculateIndividualItems ? sizes.ToArray() : null;
		return sizes.Sum();
	}

	public abstract long CalculateSize(SerializationContext context, TItem item);

}
