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

// ReSharper disable PossibleInvalidOperationException
public class ConstantLengthItemSizer<TItem> : IItemSizer<TItem> {

	public ConstantLengthItemSizer(long staticSize, bool supportsNull) {
		Guard.ArgumentInRange(staticSize, 0, int.MaxValue, nameof(staticSize));
		ConstantSize = staticSize;
		SupportsNull = supportsNull;
	}

	public virtual bool SupportsNull { get; private set; }

	public bool IsConstantSize => true;

	public long ConstantSize { get; }
	
	public long CalculateTotalSize(SerializationContext context, IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes) {
		return CalculateTotalSize(items.Count(), calculateIndividualItems, out itemSizes);
	}

	public long CalculateTotalSize(long itemsCount, bool calculateIndividualItems, out long[] itemSizes) {
		var val = ConstantSize;
		var size = itemsCount * val;
		itemSizes = calculateIndividualItems ? Tools.Array.Gen(itemsCount, val) : null;
		return size;
	}

	public long CalculateSize(SerializationContext context, TItem item) => ConstantSize;
}
