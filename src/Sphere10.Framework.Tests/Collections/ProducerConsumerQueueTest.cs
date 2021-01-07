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
        public async Task Complex_1() {
            var expected = Enumerable.Range(0, 1000).ToArray();
            var result = new List<int>();
            var @lock = new object();
            var counter = 0;

            var putManyCounter = 0;
            var takeManyCounter = 0;

            using (var queue = new ProducerConsumerQueue<int>(10)) {

                Func<string, Task> produceAction = async (string name) => {
                    while (counter < 1000) {
                        //await Task.Delay(10);
                        var localProduction = new List<int>();
                        lock (@lock) {
                            var numToProduce = Tools.Maths.RNG.Next(1, 10);
                            for (var i = 0; i < numToProduce; i++) {
                                if (counter == 1000)
                                    break;
                                localProduction.Add(counter++);
                            }
                        }
                        await queue.PutManyAsync(localProduction);
                    }
                };

                Func<string, Task> consumeAction = async (string name) => {
                    while (!queue.HasFinishedProducing) {
                        //await Task.Delay(10);
                        var numToConsume = Tools.Maths.RNG.Next(1, 10);
                        var consumption = await queue.TakeManyAsync(numToConsume);
                        result.AddRange(consumption);
                    }
                };

                Func<Task> produceTask = async () => {
                    await Task.WhenAll(produceAction("Producer 1"));
                    queue.FinishedProducing();
                };

                Func<Task> consumeTask = async () => {
                    await Task.WhenAll(consumeAction("Consumer 1"));
                    queue.FinishedConsuming();
                };

                await Task.WhenAll(produceTask(), consumeTask());
                Tools.NUnit.Print(result);
                Assert.AreEqual(expected, result);

            }
        }


        [Test]
        public async Task Complex_2() {
            var expected = Enumerable.Range(0, 1000).ToArray();
            var result = new SynchronizedExtendedList<int>();
            var @lock = new object();
            var counter = 0;

            using (var queue = new ProducerConsumerQueue<int>(10)) {

				async Task ProduceAction(string name) {
					while (counter < 1000) {
						//await Task.Delay(10);
						var localProduction = new List<int>();
						lock (@lock) {
							var numToProduce = Tools.Maths.RNG.Next(1, 10);
							for (var i = 0; i < numToProduce; i++) {
								if (counter == 1000)
									break;

								localProduction.Add(counter++);
							}
						}
						await queue.PutManyAsync(localProduction);
					}
				}

				async Task ConsumeAction(string name) {
					if(!queue.HasFinishedProducing) {
                        var xxx = 1;
					}

                    while (!queue.HasFinishedProducing || queue.Count > 0) {
						//await Task.Delay(10);
						var numToConsume = Tools.Maths.RNG.Next(1, 10);
						var consumption = await queue.TakeManyAsync(numToConsume);
						result.AddRange(consumption);
					}
				}

				var producers = new[] {
                    ProduceAction("Producer 1"),
                    ProduceAction("Producer 2"),
                    ProduceAction("Producer 3"),
                    ProduceAction("Producer 4"),
                    ProduceAction("Producer 5"),
                };

                var consumers = new[] {
                    ConsumeAction("Consumer 1"),
                    ConsumeAction("Consumer 2"),
                    ConsumeAction("Consumer 3"),         
                };

                Func<Task> produceTask = async () => {
                    await Task.WhenAll(producers);
                    queue.FinishedProducing();
                };

                Func<Task> consumeTask = async () => {
                    await Task.WhenAll(consumers);
                    queue.FinishedConsuming();
                };

                await Task.WhenAll(produceTask(), consumeTask());

                Tools.NUnit.Print(result);
                //result.Sort();
                var resultArr = result.OrderBy(x => x).ToArray();
                Assert.AreEqual(expected, resultArr);

            }
        }

    }

}
