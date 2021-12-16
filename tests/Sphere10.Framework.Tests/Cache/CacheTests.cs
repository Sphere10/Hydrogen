//-----------------------------------------------------------------------
// <copyright file="CacheTests.cs" company="Sphere 10 Software">
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
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
    public class CacheTests {

        [Test]
        public void MaxCapacity_1() {
            var cache = new ActionCache<int, int>( x => x, sizeEstimator: x => 1, maxCapacity: 1, reapStrategy: CacheReapPolicy.LeastUsed );
            var val = cache[0];
            Assert.AreEqual(1, cache.ItemCount);
			val = cache[1];
			Assert.AreEqual(1, cache.ItemCount);
        }

		[Test]
		public void MaxCapacity_2() {
			var cache = new ActionCache<int, int>(x => x, sizeEstimator: x => 1, maxCapacity: 3, reapStrategy: CacheReapPolicy.LeastUsed);
			var val = cache[0];
			val = cache[1];
			val = cache[2];
			Assert.AreEqual(3, cache.ItemCount);
            for (var i = 3; i < 1000; i++) {
                val = cache[i];
                Assert.AreEqual(3, cache.ItemCount);
            }
		}


		[Test]
		public void ItemRemoved_1() {
            var removed = new List<int>();
			var cache = new ActionCache<int, int>(x => x, sizeEstimator: x => 1, maxCapacity: 1, reapStrategy: CacheReapPolicy.LeastUsed);
            cache.ItemRemoved += (i, item) => removed.Add(item.Value);
			
            var val = cache[0];
			Assert.AreEqual(0, removed.Count);

			val = cache[1];
			Assert.AreEqual(1, removed.Count);
			Assert.AreEqual(0, removed[0]);
        }

		[Test]
		public void ItemRemoved_2() {
			var removed = new List<int>();
			var cache = new ActionCache<int, int>(x => x, sizeEstimator: x => 1, maxCapacity: 1, reapStrategy: CacheReapPolicy.LeastUsed);
			cache.ItemRemoved += (i, item) => removed.Add(item.Value);

            for(var i = 0; i < 1000; i++) {
                var val = cache[i];
			}

			Assert.AreEqual(999, removed.Count);
            removed.Sort();
            Assert.AreEqual(Enumerable.Range(0, 999).ToArray(), removed.ToArray());
		}

        [Test]
        public void BulkTest_Simple_1() {
            var cache = new BulkFetchActionCache<int, string>(
                () => new Dictionary<int, string>() {
                    {1, "one"}, {2, "two"}, {3, "three"}
                });
           Assert.AreEqual("one", cache[1]); 
           Assert.AreEqual(new [] { "one", "two", "three"}, cache.CachedItems.Select(x => x.Value).ToArray());
        }

		[Test]
        public void ExpirationTest_Simple_1() {
            var val = "first";
            var cache = new ActionCache<int, string>(
                (x) => val,
                reapStrategy: CacheReapPolicy.LeastUsed,
                expirationStrategy: ExpirationPolicy.SinceFetchedTime,
                expirationDuration: TimeSpan.FromMilliseconds(100)
            );      
            Assert.AreEqual("first", cache[1]);
            val = "second";
            Assert.AreEqual("first", cache[1]);
            Assert.AreEqual("first", cache[1]);
            Thread.Sleep(111);          
            Assert.AreEqual("second", cache[1]);
            Assert.AreEqual("second", cache[1]);
            Assert.AreEqual("second", cache[1]);   
        }

        [Test]
        public void SizeTest_Simple_1() {
            var cache = new ActionCache<int, string>(
                (x) => x.ToString(),
                reapStrategy: CacheReapPolicy.LeastUsed,
                expirationStrategy: ExpirationPolicy.SinceFetchedTime,
                expirationDuration: TimeSpan.FromMilliseconds(100),
                sizeEstimator: long.Parse,
                maxCapacity: 100
            );

            Assert.Throws<InvalidOperationException>(() => { var x = cache[101]; });
            Assert.AreEqual("98", cache[98]);
            Assert.AreEqual(1, cache.InternalStorage.Count);
            Assert.AreEqual("2", cache[2]);
            Assert.AreEqual(2, cache.InternalStorage.Count);
            Assert.AreEqual("1", cache[1]);
            Assert.AreEqual(2, cache.InternalStorage.Count);  // should have purged first item
            Assert.AreEqual(new [] { "1", "2"}, cache.GetAllCachedValues().ToArray());
            Assert.AreEqual("100", cache[100]);
            Assert.AreEqual(1, cache.InternalStorage.Count);  // should have purged everything 
        }

		[Test]
		public void ContainsCachedItem_1() {
			var called = false;
			var cache = new ActionCache<int, string>(
				(x) => {
					if (x != 1)
						throw new Exception("test only allows key with value 1");
					if (called)
						throw new Exception("item 1 has been requested more than once");
					called = true;
					return "value";
				},
				reapStrategy: CacheReapPolicy.LeastUsed,
				expirationStrategy: ExpirationPolicy.SinceFetchedTime,
				expirationDuration: TimeSpan.FromMilliseconds(100)
			);
			Assert.IsFalse(cache.ContainsCachedItem(1));
			var val = cache[1];
			Assert.IsTrue(cache.ContainsCachedItem(1));
			Thread.Sleep(111);
			Assert.IsFalse(cache.ContainsCachedItem(1));
		}


		[Test]
		public void TestEmptySize() {
			var cache = new ActionCache<int, string>(
				_ => string.Empty,
				s => s.Length,
				0
			);
            for (var i = 0; i < 100; i++) {
                var item = cache[i];
                Assert.AreEqual(i + 1, cache.ItemCount);
                Assert.AreEqual(0, cache.CurrentSize);
            }
		}

        [Test]
        public void TestOutOfSpace_1() {
	        string[] items = { "", "1", "22", "333" };

	        var cache = new ActionCache<int, string>(
		        (x) => items[x],
                s => s.Length,
                CacheReapPolicy.Smallest,
                ExpirationPolicy.None,
                5
	        );

	        var x = cache[0]; // item ""
	        var y = cache[1]; // item "1"
	        var z = cache[2]; // item "22"
	        cache.Get(0).CanPurge = false;
	        cache.Get(1).CanPurge = false;
	        cache.Get(2).CanPurge = false;
            string d = default;
	        Assert.Throws<InvalidOperationException>(() => d = cache[3]);
        }

        

    }
}
