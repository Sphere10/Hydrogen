//-----------------------------------------------------------------------
// <copyright file="ProducerConsumerQueueTest.cs" company="Sphere 10 Software">
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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Tracing.Parsers;

namespace Sphere10.Framework.Tests {

	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
    public class ProducerConsumerQueueTest {



		//[TearDown]
		//public void PrintDebuggerMessages() {
		//	Tools.Debugger.Messages.TakeLast(1000).ForEach(Console.WriteLine);
		//	var p_waitingSem = Tools.Debugger.ProducersWaitingSemaphore.Select(x => x?.ToString()).ToDelimittedString(", ");
		//	var p_insideSem = Tools.Debugger.ProducersInsideSemaphore.Select(x => x?.ToString()).ToDelimittedString(", ");
		//	var p_waitinglock = Tools.Debugger.ProducersWaitingLock.Select(x => x?.ToString()).ToDelimittedString(", ");
		//	var p_insideLock = Tools.Debugger.ProducersInsideLock.Select(x => x?.ToString()).ToDelimittedString(", ");
		//	var c_waitingSem = Tools.Debugger.ConsumersWaitingSemaphore.Select(x => x?.ToString()).ToDelimittedString(", ");
		//	var c_insideSem = Tools.Debugger.ConsumersInsideSemaphore.Select(x => x?.ToString()).ToDelimittedString(", ");
		//	var c_waitinglock = Tools.Debugger.ConsumersWaitingLock.Select(x => x?.ToString()).ToDelimittedString(", ");
		//	var c_insideLock = Tools.Debugger.ConsumersInsideLock.Select(x => x?.ToString()).ToDelimittedString(", ");
		//	Console.WriteLine($"Producer Threads - Waiting Semaphore: {p_waitingSem}, Inside Semaphore: {p_insideSem}, Waiting Lock: {p_waitinglock}, Inside Lock: {p_insideLock}");
		//	Console.WriteLine($"Consumer Threads - Waiting Semaphore: {c_waitingSem}, Inside Semaphore: {c_insideSem}, Waiting Lock: {c_waitinglock}, Inside Lock: {c_insideLock}");
		//}

		[Test]
        public async Task Simple() {
	        using var queue = new ProducerConsumerQueue<string>(10);
	        await queue.PutAsync("Hello World!");
	        Assert.AreEqual(1, queue.Count);
	        var r = await queue.TakeManyAsync(1);
	        Assert.AreEqual(0, queue.Count);
	        Assert.AreEqual(1, r.Length);
	        Assert.AreEqual("Hello World!", r[0]);
        }
        
        [Test]
		public async Task Complex_1(
	        [Values(1, 1000, 10000)] int totalProduction,
	        [Values(1, 100, 1000)] int queueCapacity,
			[Values(1, 10, 100)] int maxProducePerIteration,
			[Values(1, 10, 100)]  int maxConsumePerIteration) {
	        var RNG = new Random(31337);
            var expected = Enumerable.Range(0, totalProduction).ToArray();
            var result = new List<int>();
            var @lock = new object();
            var counter = 0;

            using var queue = new ProducerConsumerQueue<int>(queueCapacity);

            void ProduceAction() {
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
				while (queue.IsConsumable) {
					// await Task.Delay(10);
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
            Assert.AreEqual(expected, result);
        }

        
		[Test]
        public async Task Complex_2(
	        [Values(1, 1000, 10000)] int totalProduction,
	        [Values(1, 100, 1000)] int queueCapacity,
	        [Values(1, 10, 100)] int maxProducePerIteration,
	        [Values(1, 10, 100)] int maxConsumePerIteration) {
	        const int TimeoutSEC = 10;
	        var RNG = new Random(31337);
			var expected = Enumerable.Range(0, totalProduction).ToArray();
            var result = new SynchronizedExtendedList<int>();
            var @lock = new object();
            var counter = 0;

            using var queue = new ProducerConsumerQueue<int>(queueCapacity);
			async Task ProduceAction(int id) {
				while (counter < totalProduction) {
					var localProduction = new List<int>();
					lock (@lock) {
			            var numToProduce = RNG.Next(0, maxProducePerIteration+1);
			            for (var i = 0; i < numToProduce; i++) {
				            if (counter == totalProduction)
					            break;

				            localProduction.Add(counter++);
			            }
		            }
					await queue.PutManyAsync(localProduction);
	            }
			}

            async Task ConsumeAction(int id) {
				while (queue.IsConsumable) {
		            var numToConsume = RNG.Next(0, maxConsumePerIteration+1);
		            var consumption = await queue.TakeManyAsync(numToConsume);
		            result.AddRange(consumption);
	            }
			}

            var producers = new[] {
	            ProduceAction(1),
				ProduceAction(2),
				ProduceAction(3),
				ProduceAction(4),
				ProduceAction(5),
				ProduceAction(6),
				ProduceAction(7),
				ProduceAction(8),
			};

            var consumers = new[] {
	            ConsumeAction(1),
				ConsumeAction(2),
				ConsumeAction(3),
				ConsumeAction(4),
				ConsumeAction(5),
				ConsumeAction(6),
			};

            async Task ProduceTask() {
				await Task.WhenAll(producers);
				queue.FinishedProducing();
			}

            async Task ConsumeTask() {
				await Task.WhenAll(consumers);
				queue.FinishedConsuming();
			}
            
            await Task.WhenAll(ProduceTask(), ConsumeTask()).WithTimeout(TimeSpan.FromSeconds(TimeoutSEC));
			var resultArr = result.OrderBy(x => x).ToArray();
            Assert.AreEqual(expected, resultArr);
        }


		[Test]
		public void SemaphoreSlim_Bug() {
			for (var i = 0; i < 100; i++)
				Assert.That(() => Complex_2(10000, 1000, 1, 1), Throws.Nothing);
		}

	}

}
