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
	protected TObjectSizer Internal;

	// This is a special purpose constructor needed for late binding of internal serializer
	// (needed by SerializerFactory when assembling serializers dynamically)
	internal ItemSizerDecorator() {
		Internal = default;
	}

	public ItemSizerDecorator(TObjectSizer internalSizer) {
		Guard.ArgumentNotNull(internalSizer, nameof(internalSizer));
		Internal = internalSizer;
	}

	public virtual bool SupportsNull => Internal.SupportsNull;

	public virtual bool IsConstantSize => Internal.IsConstantSize;

	public virtual long ConstantSize => Internal.ConstantSize;

	public virtual long CalculateTotalSize(IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes)
		=> Internal.CalculateTotalSize(items, calculateIndividualItems, out itemSizes);

	public virtual long CalculateSize(SerializationContext context, TItem item) => Internal.CalculateSize(context, item);
}
