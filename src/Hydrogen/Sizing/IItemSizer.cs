// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hydrogen;


public interface IItemSizer {
	
	Type ItemType { get; }

	bool SupportsNull { get; }

	bool IsConstantSize { get; }

	long ConstantSize { get; }

	long PackedCalculateTotalSize(SerializationContext context, IEnumerable<object> items, bool calculateIndividualItems, out long[] itemSizes);

	long PackedCalculateSize(SerializationContext context, object item);
}

public interface IItemSizer<in T> : IItemSizer  {

	Type IItemSizer.ItemType => typeof(T);
	
	long CalculateTotalSize(SerializationContext context, IEnumerable<T> items, bool calculateIndividualItems, out long[] itemSizes);

	long CalculateSize(SerializationContext context, T item);

	long IItemSizer.PackedCalculateTotalSize(SerializationContext context, IEnumerable<object> items, bool calculateIndividualItems, out long[] itemSizes)
		=> CalculateTotalSize(context, items.Cast<T>(), calculateIndividualItems, out itemSizes);

	long IItemSizer.PackedCalculateSize(SerializationContext context, object item)
		=> CalculateSize(context, (T)item);

}


public static class IItemSizerExtensions {

	public static long CalculateTotalSize<T>(this IItemSizer<T> sizer, IEnumerable<T> items, bool calculateIndividualItems, out long[] itemSizes) {
		using var context = SerializationContext.New;
		return sizer.CalculateTotalSize(context, items, calculateIndividualItems, out itemSizes);
	}

	public static long CalculateSize<TItem>(this IItemSizer<TItem> sizer, TItem item) {
		using var context = SerializationContext.New;
		return sizer.CalculateSize(context, item);
	}

	public static long PackedCalculateSize(this IItemSizer sizer, object item) {
		using var context = SerializationContext.New;
		return sizer.PackedCalculateSize(context, item);
	}
}