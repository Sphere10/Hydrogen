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


public interface IItemSizer {
	
	Type ItemType { get; }

	bool SupportsNull { get; }

	bool IsConstantLength { get; }

	long ConstantLength { get; }

	long CalculateTotalSize(IEnumerable<object> items, bool calculateIndividualItems, out long[] itemSizes);

	long CalculateSize(object item);
}

public interface IItemSizer<in T> : IItemSizer  {

	Type IItemSizer.ItemType => typeof(T);
	
	long CalculateTotalSize(IEnumerable<T> items, bool calculateIndividualItems, out long[] itemSizes);

	long CalculateSize(T item);

	long IItemSizer.CalculateTotalSize(IEnumerable<object> items, bool calculateIndividualItems, out long[] itemSizes)
		=> CalculateTotalSize(items.Cast<T>(), calculateIndividualItems, out itemSizes);

	long IItemSizer.CalculateSize(object item)
		=> CalculateSize((T)item);
}
