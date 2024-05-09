// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
//[Parallelizable(ParallelScope.Children)]  // don't parallelize, since will affect producer/consumer threads
public class ProducerConsumerQueueTest {

	//[TearDown]
	//public void PrintDebugMessages() {
	//	File.WriteAllLines("c:/temp/log.txt", Tools.Debugger.Messages);
	//	Tools.Debugger.Messages.Clear();
	//}

	[Test]
	public async Task Simple() {
		using var queue = new ProducerConsumerQueue<string>(10);
		await queue.PutAsync("Hello World!");
		ClassicAssert.AreEqual(1, queue.Count);
		var r = await queue.TakeManyAsync(1);
		ClassicAssert.AreEqual(0, queue.Count);
		ClassicAssert.AreEqual(1, r.Length);
		ClassicAssert.AreEqual("Hello World!", r[0]);
	}

	[Test]
	public async Task Complex_1(
		[Values(1, 1000, 10000)] int totalProduction,
		[Values(1, 100, 1000)] int queueCapacity,
		[Values(1, 10, 100)] int maxProducePerIteration,
		[Values(1, 10, 100)] int maxConsumePerIteration) {
		var expected = Enumerable.Range(0, totalProduction).ToArray();
		var result = new List<int>();
		var @lock = new object();
		var counter = 0;

		using var queue = new ProducerConsumerQueue<int>(queueCapacity);

		void ProduceAction() {
			var RNG = new Random(31337);
			while (counter < totalProduction) {
				//await Task.Delay(10);
				var iterationProduction = new List<int>();
				lock (@lock) {
					var numToProduce = RNG.Next(0, maxProducePerIteration + 1);
					for (var i = 0; i < numToProduce; i++) {
						if (counter == totalProduction)
							break;
						iterationProduction.Add(counter++);
					}
				}
				queue.PutMany(iterationProduction);
			}
		}

		void ConsumeAction() {
			var RNG = new Random(31337);
			while (queue.IsConsumable) {
				var numToConsume = RNG.Next(0, maxConsumePerIteration + 1);
				var consumption = queue.TakeMany(numToConsume);
				result.AddRange(consumption);
			}
		}

		async Task ProduceTask() {
			await Task.WhenAll(Task.Factory.StartNew(ProduceAction));
			queue.FinishedProducing();
		}

		async Task ConsumeTask() {
			await Task.WhenAll(Task.Factory.StartNew(ConsumeAction));
			queue.FinishedConsuming();
		}

		await Task.WhenAll(ProduceTask(), ConsumeTask());
		//Tools.NUnit.Print(result);
		ClassicAssert.AreEqual(expected, result);
	}


	[Test]
	public async Task Complex_2(
		[Values(1, 1000, 10000)] int totalProduction,
		[Values(1, 100, 1000)] int queueCapacity,
		[Values(1, 10, 100)] int maxProducePerIteration,
		[Values(1, 10, 100)] int maxConsumePerIteration,
		[Values(3, 6)] int producerCount,
		[Values(6, 3)] int consumerCount) {
		const int TimeoutSEC = 10;
		var expected = Enumerable.Range(0, totalProduction).ToArray();
		var productions = new List<List<int>>[producerCount];
		var consumptions = new List<int>[consumerCount];
		var result = new SynchronizedExtendedList<int>();
		using var queue = new ProducerConsumerQueue<int>(queueCapacity);

		// create production/consumption buckets
		for (var i = 0; i < producerCount; i++)
			productions[i] = new List<List<int>>();
		for (var i = 0; i < consumerCount; i++)
			consumptions[i] = new List<int>();

		// pre-fill production buckets (we want to test queue not the producers)
		var RNG = new Random(31337);
		var counter = 0;
		var producer = 0;
		while (counter < totalProduction) {
			var production = new List<int>();
			productions[producer].Add(production);
			var numToProduce = RNG.Next(0, maxProducePerIteration + 1);
			for (var i = 0; i < numToProduce; i++) {
				if (counter == totalProduction)
					break;

				production.Add(counter++);
			}
			producer = (producer + 1) % producerCount;
		}

		async Task ProduceAction(int id) {
			var start = DateTime.Now;
			foreach (var production in productions[id])
				await queue.PutManyAsync(production);
		}

		async Task ConsumeAction(int id) {
			var start = DateTime.Now;
			var RNG = new Random(31337 * id);
			while (queue.IsConsumable) {
				var numToConsume = RNG.Next(0, maxConsumePerIteration + 1);
				var consumption = await queue.TakeManyAsync(numToConsume);
				consumptions[id].AddRange(consumption);
			}
		}

		// generate producers/consumers together to prevent a swarm of producers (or consumers) piling up
		// on the semaphore (this I've found is very costly).
		var producers = new Task[producerCount];
		var consumers = new Task[consumerCount];
		for (var i = 0; i < Tools.Values.Max(producerCount, consumerCount); i++) {
			if (i < producerCount)
				producers[i] = ProduceAction(i);
			if (i < consumerCount)
				consumers[i] = ConsumeAction(i);

		}

		async Task ProduceTask() {
			await Task.WhenAll(producers);
			queue.FinishedProducing();
		}

		async Task ConsumeTask() {
			await Task.WhenAll(consumers);
			queue.FinishedConsuming();
			consumptions.ForEach(result.AddRange);
		}

		await Task.WhenAll(ProduceTask(), ConsumeTask()).WithTimeout(TimeSpan.FromSeconds(TimeoutSEC));
		var resultArr = result.OrderBy(x => x).ToArray();
		ClassicAssert.AreEqual(expected, resultArr);
	}


	[Test]
	public void SemaphoreSlim_Bug() {
		for (var i = 0; i < 100; i++)
			Assert.That(() => Complex_2(10000, 1000, 1, 1, 8, 6), Throws.Nothing);
	}

}
