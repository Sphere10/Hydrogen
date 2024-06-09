// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen;

public static class IAsyncEnumerableExtensions {

	public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable) {
		var list = new List<T>();
		await foreach (var item in asyncEnumerable)
			list.Add(item);
		return list;
	}

	public static async Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
		=> (await asyncEnumerable.ToListAsync()).ToArray();
}
