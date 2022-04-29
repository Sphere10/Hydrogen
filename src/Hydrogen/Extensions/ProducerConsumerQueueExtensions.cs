//-----------------------------------------------------------------------
// <copyright file="ProducerConsumerQueueExtensions.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen {

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
}
