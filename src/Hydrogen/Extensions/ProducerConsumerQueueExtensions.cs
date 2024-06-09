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

public static class ProducerConsumerQueueExtensions {
	public static void Put<T>(this ProducerConsumerQueue<T> producerConsumer, T item) {
		producerConsumer.PutMany(new[] { item });
	}

	public static async Task PutAsync<T>(this ProducerConsumerQueue<T> producerConsumer, T item) {
		await Task.Run(() => producerConsumer.Put(item));
	}

	public static async Task PutManyAsync<T>(this ProducerConsumerQueue<T> producerConsumer, IEnumerable<T> items) {
		await Task.Run(() => producerConsumer.PutMany(items));
	}

	public static async Task<T> TakeAsync<T>(this ProducerConsumerQueue<T> producerConsumer) {
		return await Task.Run(() => producerConsumer.Take());
	}

	public static async Task<T[]> TakeManyAsync<T>(this ProducerConsumerQueue<T> producerConsumer, int maxItems) {
		return await Task.Run(() => producerConsumer.TakeMany(maxItems));
	}

	public static async Task<T[]> TakeBySizeAsync<T>(this ProducerConsumerQueue<T> producerConsumer, int maxSize, bool orFirstItem = false) {
		return await Task.Run(() => producerConsumer.TakeBySize(maxSize));
	}

	public static async Task<T[]> TakeAllAsync<T>(this ProducerConsumerQueue<T> producerConsumer) {
		return await Task.Run(() => producerConsumer.TakeAll());
	}

}
