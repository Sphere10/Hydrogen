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

public static class ILookupExtensions {

	public static IDictionary<TKey, TAggregatedValue> AggregateValue<TKey, TValue, TAggregatedValue>(this ILookup<TKey, TValue> lookup, TAggregatedValue seed, Func<TAggregatedValue, TValue, TAggregatedValue> valueAggregator)
		=> lookup.ToDictionary(x => x.Key, x => x.Aggregate(seed, valueAggregator));

	public static ILookup<TKey, TValueOut> Transform<TKey, TValue, TValueOut>(
		this ILookup<TKey, TValue> lookup,
		Func<TValue, TValueOut> selector) {
		// NOTE: does not call method below since it is not tested and DataSync uses this!
		return lookup.SelectMany(g => g,
			(g, v) => new KeyValuePair<TKey, TValueOut>(g.Key, selector(v))).ToLookup(kvp => kvp.Key, kvp => kvp.Value);
	}


	public static ILookup<TKeyOut, TValueOut> Transform<TKey, TValue, TKeyOut, TValueOut>(
		this ILookup<TKey, TValue> lookup,
		Func<TKey, TKeyOut> keyTransformer,
		Func<TValue, TValueOut> valueTransformer) {

		var lookupEx = new ExtendedLookup<TKeyOut, TValueOut>();
		foreach (var grouping in lookup)
			lookupEx.AddRange(keyTransformer(grouping.Key), grouping.Select(valueTransformer));

		return lookupEx;
	}
}
