// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class ActionItemSizer<T> : IItemSizer<T> {
	private readonly Func<T, long> _sizer;

	public ActionItemSizer(Func<T, long> sizer) {
		Guard.ArgumentNotNull(sizer, nameof(sizer));
		_sizer = sizer;
	}

	public bool IsStaticSize => false;

	public long StaticSize => -1;

	public long CalculateTotalSize(IEnumerable<T> items, bool calculateIndividualItems, out long[] itemSizes) {
		var sizes = items.Select(item => CalculateSize(item)).ToArray();
		itemSizes = calculateIndividualItems ? sizes : null;
		return sizes.Sum();
	}

	public long CalculateSize(T item) => _sizer(item);
}
