// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public class ItemSizerDecorator<TItem, TObjectSizer> : IItemSizer<TItem> where TObjectSizer : IItemSizer<TItem> {
	internal TObjectSizer Internal;

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

	public virtual long CalculateTotalSize(SerializationContext context, IEnumerable<TItem> items, bool calculateIndividualItems, out long[] itemSizes)
		=> Internal.CalculateTotalSize(context, items, calculateIndividualItems, out itemSizes);

	public virtual long CalculateSize(SerializationContext context, TItem item) => Internal.CalculateSize(context, item);
}
