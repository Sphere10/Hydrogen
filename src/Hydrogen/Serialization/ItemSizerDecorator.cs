// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public class ItemSizerDecorator<TItem, TObjectSizer> : IItemSizer<TItem> where TObjectSizer : IItemSizer<TItem> {
	protected readonly TObjectSizer Internal;

	public ItemSizerDecorator(TObjectSizer internalSizer) {
		Internal = internalSizer;
	}

	public virtual bool SupportsNull => Internal.SupportsNull;

	public virtual bool IsConstantLength => Internal.IsConstantLength;

	public virtual long ConstantLength => Internal.ConstantLength;

	public virtual long CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes)
		=> Internal.CalculateTotalSize(items, calculateIndividualItems, out itemSizes);

	public virtual long CalculateSize(TItem item) => Internal.CalculateSize(item);
}
