// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public static class PagedListExtensions {

	public static void ShuffleLeftRemoveAt<T>(this IPagedList<T> pagedList, long index) {
		Guard.ArgumentNotNull(pagedList, nameof(pagedList));
		var count = pagedList.Count;
		Guard.ArgumentGT(count, 0, nameof(pagedList), "List is empty");
		Guard.ArgumentInRange(index, 0, count - 1, nameof(index));
		UpdateOnlyList<T>.ShuffleLeft(pagedList, index + 1, index, count - index - 1, true);
		pagedList.RemoveAt(count - 1);
	}

	public static void ShuffleRightInsert<T>(this IPagedList<T> pagedList, long index, T item) {
		Guard.ArgumentNotNull(pagedList, nameof(pagedList));
		var count = pagedList.Count;
		Guard.ArgumentInRange(index, 0, count, nameof(index));
		pagedList.Add(item); // add item at end increase list by one
		UpdateOnlyList<T>.ShuffleRight(pagedList, index, index+1, count - index, true);
		pagedList.Update(index, item);
	}

}
