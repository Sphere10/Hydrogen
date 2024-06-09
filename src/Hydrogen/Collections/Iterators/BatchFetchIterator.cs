// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public class BatchFetchIterator<T> : IEnumerable<T> {
	private readonly IEnumerable<Func<IEnumerable<T>>> _batches;

	public BatchFetchIterator(int batchSize, int totalItems, Func<int, int, IEnumerable<T>> batchFetcher) {
		var numBatches = (int)Math.Ceiling((totalItems / (float)batchSize));
		_batches =
			Enumerable
				.Range(0, numBatches)
				.Select(batch => new Func<IEnumerable<T>>(
					() => {
						var startIndex = batch * batchSize;
						var readCount = (totalItems - startIndex).ClipTo(0, batchSize);
						return batchFetcher(startIndex, readCount);
					}
				));
	}

	public BatchFetchIterator(IEnumerable<Func<IEnumerable<T>>> batches) {
		_batches = batches;
	}

	public IEnumerator<T> GetEnumerator() {
		foreach (var batchFetcher in _batches) {
			var batchItems = batchFetcher();
			if (!batchItems.Any())
				yield break;
			foreach (var item in batchItems)
				yield return item;
		}
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return GetEnumerator();
	}
}
