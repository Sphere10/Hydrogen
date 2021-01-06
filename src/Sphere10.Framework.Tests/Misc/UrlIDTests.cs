//-----------------------------------------------------------------------
// <copyright file="UrlIDTests.cs" company="Sphere 10 Software">
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
using NUnit.Framework;

namespace Sphere10.Framework.UnitTests {

    [TestFixture]
    public class UrlIDTests {


        [Test]
        public async Task PermiateAll() {
            var permutes = new HashSet<uint>();

            using (var queue = new ProducerConsumerQueue<uint>(uint.MaxValue)) {
                var permuteTask = new Task(() => {
                    for (long i = uint.MinValue; i <10000000; i++) {
                        queue.Put(UrlID.PermuteId((uint)i));
                        if (i%1000000 == 0)
                            System.Console.WriteLine("Processed {0}", i);

                    }
                    queue.FinishedProducing();                    
                });

                var checkTask = new Task(() => {
                    while (!queue.HasFinishedProducing) {
                        var toCheck = queue.Take();
                        Assert.AreEqual(false, permutes.Contains(toCheck), "Value {0} clashed".FormatWith(toCheck));
                        permutes.Add(toCheck);
                    }
                });

                permuteTask.Start();
                checkTask.Start();
                await Task.WhenAll(permuteTask, checkTask);
            }
        }

    }

}
