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
using System.Threading.Tasks;
using NUnit.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

    [TestFixture]
	public class PooledCacheTests {

	    [Test]
	    public void DisallowInconsistentMaxSize() {
		    var pool = new PooledCacheReaper(10);
		    using var cache1 = new ActionCache<int, int>(x => x, sizeEstimator: x => x, maxCapacity: 10, reapStrategy: CacheReapPolicy.LeastUsed, reaper: pool);
		    ICache cache2;
		    Assert.Throws<ArgumentException>( () => cache2 = new ActionCache<string, string>(x => x, sizeEstimator: x => x.Length, maxCapacity: 11, reapStrategy: CacheReapPolicy.Smallest, reaper: pool));
	    }


        [Test]
        public void Exhausted_RequestdSpaceTooLarge() {
	        var pool = new PooledCacheReaper(10);
	        using var cache1 = new ActionCache<int, int>(x => x, sizeEstimator: x => x, maxCapacity: 10, reapStrategy: CacheReapPolicy.LeastUsed, reaper: pool);
	        using var cache2 = new ActionCache<string, string>(x => x, sizeEstimator: x => x.Length, maxCapacity: 10, reapStrategy: CacheReapPolicy.Smallest, reaper: pool);

	        var _1 = cache1.Get(9); // allocated 9 bytes, total 9
	        var _2 = cache2.Get("0"); // allocated 1 bytes, total 10 (pool exhausted now)
	        Assert.Throws<InvalidOperationException>(()=>cache1.Get(11)); // allocated 11 bytes, too much
	        Assert.IsFalse(_1.Traits.HasFlag(CachedItemTraits.Purged)); 
	        Assert.IsFalse(_2.Traits.HasFlag(CachedItemTraits.Purged));
        }

        [Test]
        public void Exhausted_NotEnoughSpaceDueToUnpurgable() {
	        var pool = new PooledCacheReaper(10);
	        using var cache1 = new ActionCache<int, int>(x => x, sizeEstimator: x => x, maxCapacity: 10, reapStrategy: CacheReapPolicy.LeastUsed, reaper: pool);
	        using var cache2 = new ActionCache<string, string>(x => x, sizeEstimator: x => x.Length, maxCapacity: 10, reapStrategy: CacheReapPolicy.Smallest, reaper: pool);

	        var _1 = cache1.Get(9); // allocated 9 bytes, total 9
	        var _2 = cache2.Get("0"); // allocated 1 bytes, total 10 (pool exhausted now)
	        _2.CanPurge = false;
	        Assert.Throws<InvalidOperationException>(() => cache1.Get(10)); // allocated 10 bytes, but can't purge _2 
	        Assert.IsTrue(_1.Traits.HasFlag(CachedItemTraits.Purged)); // purged this one whilst trying to free space
	        Assert.IsFalse(_2.Traits.HasFlag(CachedItemTraits.Purged)); // didn't purge this one, then failed and threw
        }

        [Test]
        public void Simple_1() {
	        var pool = new PooledCacheReaper(10);
	        using var cache1 = new ActionCache<int, int>(x => x, sizeEstimator: x => x, maxCapacity: 10, reapStrategy: CacheReapPolicy.LeastUsed, reaper: pool);
	        using var cache2 = new ActionCache<string, string>(x => x, sizeEstimator: x => x.Length, maxCapacity: 10, reapStrategy: CacheReapPolicy.Smallest, reaper: pool);

	        var _1 = cache1.Get(9); // allocated 9 bytes, total 9
	        Assert.AreEqual(9, cache1.CurrentSize);
	        Assert.AreEqual(9, pool.CurrentSize);

			var _2 = cache2.Get("0"); // allocated 1 bytes, total 10 (pool full now)
			Assert.AreEqual(9, cache1.CurrentSize);
			Assert.AreEqual(1, cache2.CurrentSize);
			Assert.AreEqual(10, pool.CurrentSize);

			var _3 = cache1.Get(1); // allocated 1 bytes, will reap _1 but not _2
			Assert.AreEqual(1, cache1.CurrentSize);
			Assert.AreEqual(1, cache2.CurrentSize);
			Assert.AreEqual(2, pool.CurrentSize);

			Assert.IsTrue(_1.Traits.HasFlag(CachedItemTraits.Purged));
	        Assert.IsFalse(_2.Traits.HasFlag(CachedItemTraits.Purged));
	        Assert.IsFalse(_3.Traits.HasFlag(CachedItemTraits.Purged));
		}

		[Test]
        public void Simple_2() {
	        var pool = new PooledCacheReaper(10);
	        using var cache1 = new ActionCache<int, int>(x => x, sizeEstimator: x => x, maxCapacity: 10, reapStrategy: CacheReapPolicy.LeastUsed, reaper: pool);
	        using var cache2 = new ActionCache<string, string>(x => x, sizeEstimator: x => x.Length, maxCapacity: 10, reapStrategy: CacheReapPolicy.Smallest, reaper: pool);

	        var _1 = cache1.Get(9); // allocated 9 bytes, total 9
	        Assert.AreEqual(9, cache1.CurrentSize);
	        Assert.AreEqual(0, cache2.CurrentSize);
	        Assert.AreEqual(9, pool.CurrentSize);

			var _2 = cache2.Get("0"); // allocated 1 bytes, total 10 (pool full now)
			Assert.AreEqual(9, cache1.CurrentSize);
			Assert.AreEqual(1, cache2.CurrentSize);
			Assert.AreEqual(10, pool.CurrentSize);

			var _3 = cache1.Get(10); // allocated 10 bytes, needs to reap _1 and _2
			Assert.AreEqual(10, cache1.CurrentSize);
			Assert.AreEqual(0, cache2.CurrentSize);
			Assert.AreEqual(10, pool.CurrentSize);

			Assert.IsTrue(_1.Traits.HasFlag(CachedItemTraits.Purged));
			Assert.IsTrue(_2.Traits.HasFlag(CachedItemTraits.Purged));
	        Assert.IsFalse(_3.Traits.HasFlag(CachedItemTraits.Purged));
		}

        [Test]
        public void Integration([Values(10,100)] int numCaches, [Values(100,1000,1000000)] int maxCapacity, [Values(1000)] int numIterations) {
	        var pool = new PooledCacheReaper(maxCapacity);

	        var caches = new ICache<int,int>[numCaches];
	        var maxSizeEver = 0;
	        var maxItemCountEver = 0;
			var removes = 0;
			for (var i = 0; i < numCaches; i++) {
				caches[i] = new ActionCache<int, int>(x => x, sizeEstimator: x => x, maxCapacity: maxCapacity, reapStrategy: CacheReapPolicy.LeastUsed, reaper: pool);
				caches[i].ItemRemoved += (key, cachedValue) => {
					Interlocked.Increment(ref removes);
					maxSizeEver = (int)Math.Max(maxSizeEver, pool.CurrentSize);
					maxItemCountEver = (int)Math.Max(maxItemCountEver, pool.TotalItemCount);
				};
			}
			var tasks = caches.Select((c,i) => Task.Run(() => CacheLoop(c,i))).ToArray();

			Task.WaitAll(tasks);

			Assert.LessOrEqual(maxSizeEver, maxCapacity);
			System.Console.WriteLine($"Max Size Reached: {maxSizeEver}, Total Removes: {removes}, Max Item Count Reached: {maxItemCountEver}");

			void CacheLoop(ICache<int,int> c, int index) {
				var rng = new Random(31337 * index);
				for(var i = 0; i < numIterations; i++) {
					var x = c[i % maxCapacity];
				}
			}
        }

	}
}
