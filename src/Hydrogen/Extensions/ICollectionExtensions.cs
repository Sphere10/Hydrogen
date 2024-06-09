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

public static class ICollectionExtensions {


	public static void AddRangeSequentially<T>(this ICollection<T> collection, IEnumerable<T> items) {
		items.Update(collection.Add);
	}

	/// <summary>
	/// Removes all the items from the collection matching the given predicate. 
	/// </summary>
	/// <typeparam name="T">The generic type</typeparam>
	/// <param name="collection">The collection</param>
	/// <param name="predicate">The predicate</param>
	/// <returns>The removed items</returns>
	public static IEnumerable<T> RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> predicate) {
		var items = collection.Where(predicate).Select(x => x).ToArray();
		foreach (var item in items)
			collection.Remove(item);
		return items;
	}

}
