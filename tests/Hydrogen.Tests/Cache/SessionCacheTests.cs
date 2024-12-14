// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests.Cache;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class SessionCacheTests {

	[Test]
	public void Simple_1() {
		var cache = new SessionCache<int, string>(TimeSpan.FromMilliseconds(100));
		cache.Set(1, "one");
		Thread.Sleep(300);
		cache.Cleanup(); // Need to call this manually since https://dzone.com/articles/unit-testing-multi-threaded
		ClassicAssert.AreEqual(0, cache.ItemCount);
	}

	[Test]
	public void Simple_2() {
		var cache = new SessionCache<int, string>(TimeSpan.FromMilliseconds(100));
		cache.Set(1, "one");
		cache.Set(2, "two");
		cache.Set(3, "three");
		cache.Set(4, "four");
		cache.Set(5, "five");
		cache.Set(6, "six");
		ClassicAssert.AreEqual(6, cache.ItemCount);
		Thread.Sleep(300);
		cache.Cleanup(); // Need to call this manually since https://dzone.com/articles/unit-testing-multi-threaded
		ClassicAssert.AreEqual(0, cache.ItemCount);
	}


	[Test]
	public void Complex_1() {
		var cache = new SessionCache<int, string>(TimeSpan.FromMilliseconds(200));
		cache.Set(1, "one");
		cache.Set(2, "two");
		cache.Set(3, "three");
		cache.Set(4, "four");
		cache.Set(5, "five");
		cache.Set(6, "six");
		ClassicAssert.AreEqual(6, cache.ItemCount);
		for (var i = 0; i < 10; i++) {
			cache.KeepAlive(1);
			cache.KeepAlive(2);
			cache.KeepAlive(3);
			Thread.Sleep(50);
		}
		cache.Cleanup(); // Need to call this manually since https://dzone.com/articles/unit-testing-multi-threaded
		var remaining = cache.InternalStorage;
		ClassicAssert.AreEqual(3, remaining.Count);
		ClassicAssert.IsTrue(remaining.ContainsKey(1));
		ClassicAssert.IsTrue(remaining.ContainsKey(2));
		ClassicAssert.IsTrue(remaining.ContainsKey(3));
	}

	[Test]
	public void Complex_2() {
		var cache = new SessionCache<int, string>(TimeSpan.FromMilliseconds(150));
		cache.Set(1, "one");
		cache.Set(2, "two");
		cache.Set(3, "three");
		cache.Set(4, "four");
		cache.Set(5, "five");
		cache.Set(6, "six");
		ClassicAssert.AreEqual(6, cache.ItemCount);
		for (int i = 0; i < 6; i++) {
			Thread.Sleep(100);
			cache.KeepAlive(1);
			cache.KeepAlive(2);
			cache.KeepAlive(3);
		}
		cache.Remove(2);
		cache.Remove(3);
		cache.Cleanup(); // Need to call this manually since https://dzone.com/articles/unit-testing-multi-threaded
		var remaining = cache.InternalStorage;
		ClassicAssert.AreEqual(1, remaining.Count);
		ClassicAssert.IsTrue(remaining.ContainsKey(1));
	}


	[Test]
	public void SessionDisposed() {
		var disposed = false;
		var cache = new SessionCache<int, IDisposable>(TimeSpan.FromMilliseconds(100));
		cache.ItemRemoved += (i, item) => { item.Dispose(); };
		cache.Set(1, Tools.Scope.ExecuteOnDispose(() => disposed = true));
		Thread.Sleep(200);
		cache.Cleanup(); // Need to call this manually since https://dzone.com/articles/unit-testing-multi-threaded
		ClassicAssert.IsTrue(disposed);
	}

	[Test]
	public void SessionDisposedOnFlush() {
		var disposed = false;
		var cache = new SessionCache<int, IDisposable>(TimeSpan.FromMilliseconds(100));
		cache.ItemRemoved += (i, item) => item.Dispose();
		cache.Set(1, Tools.Scope.ExecuteOnDispose(() => disposed = true));
		cache.Purge();
		ClassicAssert.IsTrue(disposed);
	}


	[Test]
	public void Throws_1() {
		var cache = new SessionCache<int, string>(TimeSpan.FromMilliseconds(100));
		cache.Set(1, "one");
		ClassicAssert.AreEqual("one", cache[1]);
		Thread.Sleep(300);
		Assert.Throws<Exception>(() => {
			var x = cache[1];
		});
	}


	[Test]
	public void DoesNotExpire() {
		var cache = new SessionCache<int, string>(TimeSpan.FromMilliseconds(100));
		cache.Set(1, "one");
		DateTime start = DateTime.Now;
		while (DateTime.Now.Subtract(start).TotalSeconds <= 1.0D) {
			ClassicAssert.AreEqual("one", cache[1]);
			Thread.Sleep(50);
		}
		ClassicAssert.AreEqual("one", cache[1]);

	}

}
