// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests.Cache;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class GeneralCacheTests {

	[Test]
	public void PackedEventHandlerRemovedCorrectly() {
		var cache = new ActionCache<int, int>(x => x);
		var handlerInvocations = 0;
		cache.ItemFetched += Handler;
		var val = cache[0];
		Assert.That(handlerInvocations, Is.EqualTo(1));
		cache.ItemFetched -= Handler;
		val = cache[1];
		Assert.That(handlerInvocations, Is.EqualTo(1));  // shouldnt of have been called again

		void Handler(int arg1, int arg2) {
			handlerInvocations++;
		}
	}

	[Test]
	public void MaxCapacity_1() {
		var cache = new ActionCache<int, int>(x => x, sizeEstimator: x => 1, maxCapacity: 1, reapStrategy: CacheReapPolicy.LeastUsed);
		var val = cache[0];
		ClassicAssert.AreEqual(1, cache.ItemCount);
		val = cache[1];
		ClassicAssert.AreEqual(1, cache.ItemCount);
	}

	[Test]
	public void MaxCapacity_2() {
		var cache = new ActionCache<int, int>(x => x, sizeEstimator: x => 1, maxCapacity: 3, reapStrategy: CacheReapPolicy.LeastUsed);
		var val = cache[0];
		val = cache[1];
		val = cache[2];
		ClassicAssert.AreEqual(3, cache.ItemCount);
		for (var i = 3; i < 1000; i++) {
			val = cache[i];
			ClassicAssert.AreEqual(3, cache.ItemCount);
		}
	}


	[Test]
	public void ItemRemoved_1() {
		var removed = new List<int>();
		var cache = new ActionCache<int, int>(x => x, sizeEstimator: x => 1, maxCapacity: 1, reapStrategy: CacheReapPolicy.LeastUsed);
		cache.ItemRemoved += (i, item) => removed.Add(item.Value);

		var val = cache[0];
		ClassicAssert.AreEqual(0, removed.Count);

		val = cache[1];
		ClassicAssert.AreEqual(1, removed.Count);
		ClassicAssert.AreEqual(0, removed[0]);
	}

	[Test]
	public void ItemRemoved_2() {
		var removed = new List<int>();
		var cache = new ActionCache<int, int>(x => x, sizeEstimator: x => 1, maxCapacity: 1, reapStrategy: CacheReapPolicy.LeastUsed);
		cache.ItemRemoved += (i, item) => removed.Add(item.Value);

		for (var i = 0; i < 1000; i++) {
			var val = cache[i];
		}

		ClassicAssert.AreEqual(999, removed.Count);
		removed.Sort();
		ClassicAssert.AreEqual(Enumerable.Range(0, 999).ToArray(), removed.ToArray());
	}

	[Test]
	public void BulkTest_Simple_1() {
		var cache = new BulkFetchActionCache<int, string>(
			() => new Dictionary<int, string>() {
				{ 1, "one" }, { 2, "two" }, { 3, "three" }
			});
		ClassicAssert.AreEqual("one", cache[1]);
		ClassicAssert.AreEqual(new[] { "one", "two", "three" }, cache.CachedItems.Select(x => x.Value).ToArray());
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
		ClassicAssert.AreEqual("first", cache[1]);
		val = "second";
		ClassicAssert.AreEqual("first", cache[1]);
		ClassicAssert.AreEqual("first", cache[1]);
		Thread.Sleep(111);
		ClassicAssert.AreEqual("second", cache[1]);
		ClassicAssert.AreEqual("second", cache[1]);
		ClassicAssert.AreEqual("second", cache[1]);
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

		Assert.Throws<InvalidOperationException>(() => {
			var x = cache[101];
		});
		ClassicAssert.AreEqual("98", cache[98]);
		ClassicAssert.AreEqual(1, cache.InternalStorage.Count);
		ClassicAssert.AreEqual("2", cache[2]);
		ClassicAssert.AreEqual(2, cache.InternalStorage.Count);
		ClassicAssert.AreEqual("1", cache[1]);
		ClassicAssert.AreEqual(2, cache.InternalStorage.Count); // should have purged first item
		ClassicAssert.AreEqual(new[] { "1", "2" }, cache.GetAllCachedValues().ToArray());
		ClassicAssert.AreEqual("100", cache[100]);
		ClassicAssert.AreEqual(1, cache.InternalStorage.Count); // should have purged everything 
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
		ClassicAssert.IsFalse(cache.ContainsCachedItem(1));
		var val = cache[1];
		ClassicAssert.IsTrue(cache.ContainsCachedItem(1));
		Thread.Sleep(111);
		ClassicAssert.IsFalse(cache.ContainsCachedItem(1));
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
			ClassicAssert.AreEqual(i + 1, cache.ItemCount);
			ClassicAssert.AreEqual(0, cache.CurrentSize);
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
