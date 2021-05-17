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

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Sphere10.Framework;

namespace Sphere10.Framework.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
    public class ProducerConsumerQueueTest {

        [Test]
        public async Task Simple() {
            using (var queue = new ProducerConsumerQueue<string>(10)) {
                await queue.PutAsync("Hello World!");
                Assert.AreEqual(1, queue.Count);
                var r = await queue.TakeManyAsync(1);
                Assert.AreEqual(0, queue.Count);
                Assert.AreEqual(1, r.Length);
                Assert.AreEqual("Hello World!", r[0]);
            }
        }
        
        [Test]
		[Ignore("broken")]
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

            async Task ProduceAction() {
	            while (counter < totalProduction) {
		            //await Task.Delay(10);
		            var iterationProduction = new List<int>();
		            lock (@lock) {
			            var numToProduce = RNG.Next(0, maxProducePerIteration+1);
			            for (var i = 0; i < numToProduce; i++) {
				            if (counter == totalProduction)
					            break;
				            iterationProduction.Add(counter++);
			            }
					}
		            await queue.PutManyAsync(iterationProduction);
				}
            }

            async Task ConsumeAction() {
	            while (queue.IsConsumable) {
		            // await Task.Delay(10);
		            var numToConsume = RNG.Next(0, maxConsumePerIteration);
		            var consumption = await queue.TakeManyAsync(numToConsume);
		            result.AddRange(consumption);
	            }
            }

            async Task ProduceTask() {
	            await Task.WhenAll(ProduceAction());
	            queue.FinishedProducing();
            }

            async Task ConsumeTask() {
	            await Task.WhenAll(ConsumeAction());
	            queue.FinishedConsuming();
            }

            await Task.WhenAll(ProduceTask(), ConsumeTask());
            //Tools.NUnit.Print(result);
            Assert.AreEqual(expected, result);
        }

        [Repeat(100)]
		[Test]
        public async Task Complex_2() {
	        var RNG = new Random(31337);
			var expected = Enumerable.Range(0, 1000).ToArray();
            var result = new SynchronizedExtendedList<int>();
            var @lock = new object();
            var counter = 0;

            using var queue = new ProducerConsumerQueue<int>(10);

            async Task ProduceAction() {
	            while (counter < 1000) {
		            //await Task.Delay(10);
		            var localProduction = new List<int>();
		            lock (@lock) {
			            var numToProduce = RNG.Next(1, 10);
			            for (var i = 0; i < numToProduce; i++) {
				            if (counter == 1000)
					            break;

				            localProduction.Add(counter++);
			            }
		            }
		            await queue.PutManyAsync(localProduction);
	            }
            }

            async Task ConsumeAction() {
	            while (queue.IsConsumable) {
		            //await Task.Delay(10);
		            var numToConsume = RNG.Next(1, 10);
		            var consumption = await queue.TakeManyAsync(numToConsume);
		            result.AddRange(consumption);
	            }
            }

            var producers = new[] {
	            ProduceAction(),
	            ProduceAction(),
	            ProduceAction(),
	            ProduceAction(),
	            ProduceAction(),
            };

            var consumers = new[] {
	            ConsumeAction(),
	            ConsumeAction(),
	            ConsumeAction(),         
            };

            async Task ProduceTask() {
	            await Task.WhenAll(producers);
	            queue.FinishedProducing();
            }

            async Task ConsumeTask() {
	            await Task.WhenAll(consumers);
	            queue.FinishedConsuming();
            }

            await Task.WhenAll(ProduceTask(), ConsumeTask());

        //    Tools.NUnit.Print(result);
            //result.Sort();
            var resultArr = result.OrderBy(x => x).ToArray();
            Assert.AreEqual(expected, resultArr);
        }

    }

}
