// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;

namespace Hydrogen.Tests;

public abstract class StreamMappedListTestsBase : StreamPersistedCollectionTestsBase {

	[Test]
	public void AddOneTest() {
		var rng = new Random(31337);
		using (CreateList(out var clusteredList)) {
			var obj = new TestObject(rng);
			clusteredList.Add(obj);
			Assert.That(clusteredList.Count, Is.EqualTo(1));
			Assert.That(clusteredList[0], Is.EqualTo(obj).Using(new TestObjectEqualityComparer()));
		}
	}

	[Test]
	public void AddOneRepeat([Values(100)] int iterations) {
		var rng = new Random(31337);
		using (CreateList(out var clusteredList)) {
			for (var i = 0; i < iterations; i++) {
				var obj = new TestObject(rng);
				clusteredList.Add(obj);
				Assert.That(clusteredList.Count, Is.EqualTo(i + 1));
				Assert.That(clusteredList[i], Is.EqualTo(obj).Using(new TestObjectEqualityComparer()));
			}
		}
	}


	protected abstract IDisposable CreateList(out StreamMappedList<TestObject> clusteredList);
}
