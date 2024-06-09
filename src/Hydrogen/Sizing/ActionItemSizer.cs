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

namespace Hydrogen;

public class ActionItemSizer<T> : IItemSizer<T> {
	private readonly Func<T, long> _sizer;

	public ActionItemSizer(Func<T, long> sizer, bool supportsNull = false) {
		Guard.ArgumentNotNull(sizer, nameof(sizer));
		_sizer = sizer;
		SupportsNull = supportsNull;
	}

	public bool SupportsNull { get; }

	public bool IsConstantSize => false;

	public long ConstantSize => -1;

	public long CalculateTotalSize(SerializationContext context1, IEnumerable<T> items, bool calculateIndividualItems, out long[] itemSizes) {
		var sizes = items.Select(item => { using var context = SerializationContext.New; return CalculateSize(context, item); }).ToArray();
		itemSizes = calculateIndividualItems ? sizes : null;
		return sizes.Sum();
	}

	public long CalculateSize(SerializationContext context, T item) => _sizer(item);
}
