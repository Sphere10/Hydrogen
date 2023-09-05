// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

public abstract class StreamMappedMerkleDictionaryTestsBase : StreamPersistedCollectionTestsBase {
	protected abstract IDisposable CreateTestObjectDictionary(CHF chf, out StreamMappedMerkleDictionary<string, TestObject> merkleDictionary);

	protected abstract IDisposable CreateStringDictionary(CHF chf, out StreamMappedMerkleDictionary<string, string> merkleDictionary);


	[Test]
	public void TestTestObjectSerializer_1() {
		var rng = new Random(31337);
		var testObject = new TestObject(rng);
		testObject.A = string.Empty;

		var serializer = new TestObjectSerializer();
		var bytes = serializer.SerializeLE(testObject);
		var testObject2 = serializer.DeserializeLE(bytes);
		var comparer = new TestObjectComparer();
		Assert.That(comparer.Equals(testObject, testObject2), Is.True);
	}

	
	[Test]
	public void TestTestObjectSerializer_2() {
		var rng = new Random(31337);
		var testObject = new TestObject(rng);
		testObject.A = null;

		var serializer = new TestObjectSerializer();
		var bytes = serializer.SerializeLE(testObject);
		var testObject2 = serializer.DeserializeLE(bytes);
		var comparer = new TestObjectComparer();
		Assert.That(comparer.Equals(testObject, testObject2), Is.True);
	}



	[Test]
	public void ToArrayTest_Empty([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
			var items = clusteredDictionary.ToArray();
			Assert.That(items, Is.Empty);
		}
	}

	[Test]
	public void ToArrayTest_1([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		var rng = new Random(31337);

		var items = new List<KeyValuePair<string, TestObject>>();
		var key = rng.NextString(rng.Next(0, 100));
			var value = new TestObject(rng);
			items.Add(new KeyValuePair<string, TestObject>(key, value));
		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
		
			foreach(var kvp in items)
				clusteredDictionary.Add(kvp);

			var items2 = clusteredDictionary.ToArray();

			var comparer = new TestObjectComparer();
			Assert.That(items, Is.EquivalentTo(items2).Using(comparer));

		}
	}

	[Test]
	public void SupportsNullValue([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		using var _ = CreateStringDictionary(chf, out var merkleDict);
		merkleDict.Add("alpha", null);
		Assert.That(merkleDict["alpha"], Is.Null);
	}

	[Test]
	public void SupportsEmptyValue([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		using var _ = CreateStringDictionary(chf, out var merkleDict);
		merkleDict.Add("alpha", "");
		Assert.That(merkleDict["alpha"], Is.EqualTo(string.Empty));
	}

		
	[Test]
	public void NullAndEmptyNotSame([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		using var _ = CreateStringDictionary(chf, out var merkleDict1);
		using var __ = CreateStringDictionary(chf, out var merkleDict2);
		merkleDict1.Add("alpha", null);
		merkleDict2.Add("alpha", "");
		Assert.That(merkleDict1.MerkleTree.Root, Is.Not.EqualTo(merkleDict2.MerkleTree.Root));
	}
	
		
	[Test]
	public void ToArrayTest_100([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		var rng = new Random(31337);

		var items = new List<KeyValuePair<string, TestObject>>();
		for (var i = 0; i < 10; i++) {
			var key = rng.NextString(rng.Next(0, 100));
			var value = new TestObject(rng);
			items.Add(new KeyValuePair<string, TestObject>(key, value));
		}

		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
		
			foreach(var kvp in items)
				clusteredDictionary.Add(kvp);

			var items2 = clusteredDictionary.ToArray();

			var comparer = new TestObjectComparer();
			Assert.That(items, Is.EquivalentTo(items2).Using(comparer));

		}

		
	}



	[Test]
	public void AddNothing([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		var rng = new Random(31337);
		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
			Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
		}
	}

	[Test]
	public void AddOne([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
		var rng = new Random(31337);
		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
			clusteredDictionary.Add(key, new TestObject(rng));
			Assert.That(clusteredDictionary.Count, Is.EqualTo(1));
		}
	}

	[Test]
	public void ReuseRecord([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
		var rng = new Random(31337);
		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
			clusteredDictionary.Add(key, new TestObject(rng));
			clusteredDictionary.Remove(key);
			clusteredDictionary.Add(key, new TestObject(rng));
			Assert.That(clusteredDictionary.Count, Is.EqualTo(1));
		}
	}

	[Test]
	public void ContainsKey([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
		var rng = new Random(31337);
		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
			clusteredDictionary.Add(key, new TestObject(rng));
			Assert.That(clusteredDictionary.ContainsKey(key), Is.True);
		}
	}

	[Test]
	public void NotContainsKey([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
		var rng = new Random(31337);
		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
			clusteredDictionary.Add(key, new TestObject(rng));
			key += "1";
			Assert.That(clusteredDictionary.ContainsKey(key), Is.False);
		}
	}

	[Test]
	public void DoesNotContainKeyAfterRemove([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
		var rng = new Random(31337);
		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
			clusteredDictionary.Add(key, new TestObject(rng));
			clusteredDictionary.Remove(key);
			Assert.That(clusteredDictionary.ContainsKey(key), Is.False);
		}
	}

	[Test]
	public void ContainsKeyValuePair([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
		var rng = new Random(31337);
		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
			var value = new TestObject(rng);
			var kvp = KeyValuePair.Create(key, value);
			clusteredDictionary.Add(kvp);
			Assert.That(clusteredDictionary.Contains(kvp), Is.True);
		}
	}

	[Test]
	public void DoesNotContainKeyValuePair_SameKeyDifferentValue([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
		var rng = new Random(31337);
		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
			var value = new TestObject(rng);
			var kvp = KeyValuePair.Create(key, value);
			clusteredDictionary.Add(kvp);
			value.A += "1";
			Assert.That(clusteredDictionary.Contains(kvp), Is.False);
		}
	}

	[Test]
	public void RemoveByKey([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key) {
		var rng = new Random(31337);
		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
			clusteredDictionary.Add(key, new TestObject(rng));
			clusteredDictionary.Remove(key);
			Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
		}
	}

	[Test]
	public void RemoveByKeyValuePair([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values("alpha", "Unicode😊😊😊", "")] string key, [Values(23)] int maxItems) {
		var rng = new Random(31337);
		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
			clusteredDictionary.Add(key, new TestObject(rng));
			clusteredDictionary.Remove(key);
			Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
		}
	}

#if DEBUG
	[Test]
	public void IntegrationTests_Heavy([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values(250)] int maxItems) {
		var keyGens = 0;
		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
			AssertEx.DictionaryIntegrationTest(
				clusteredDictionary,
				maxItems,
				(rng) => ($"{keyGens++}_{rng.NextString(0, 100)}", new TestObject(rng)),
				iterations: 250,
				valueComparer: new TestObjectComparer()
			);
		}
	}
#endif

	[Test]
	public void IntegrationTests([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		const int maxItems = 100;
		var keyGens = 0;
		using (CreateTestObjectDictionary(chf, out var clusteredDictionary)) {
			AssertEx.DictionaryIntegrationTest(
				clusteredDictionary,
				maxItems,
				(rng) => ($"{keyGens++}_{rng.NextString(0, 100)}", new TestObject(rng)),
				iterations: 10,
				valueComparer: new TestObjectComparer(),
				endOfIterTest: () => {
					// Manually test the merkle root
					var itemSerializer = new TestObjectSerializer();
					
					var itemHashes = Enumerable.Range(0, (int)clusteredDictionary.ObjectContainer.Count).Select(i => {

						if (clusteredDictionary.ObjectContainer.GetItemDescriptor(i).Traits.HasFlag(ClusteredStreamTraits.Reaped))
							return Hashers.ZeroHash(chf);

						var keyHash = Hashers.HashWithNullSupport(chf, clusteredDictionary.ReadKeyBytes(i));
						var valueHash = Hashers.HashWithNullSupport(chf, clusteredDictionary.ReadValueBytes(i));
						return Hashers.JoinHash(chf, keyHash, valueHash);
					}).ToArray();
					var treeLeaves = clusteredDictionary.MerkleTree.GetLeafs().ToArray();
					CollectionAssert.AreEqual(itemHashes, treeLeaves);
					Assert.That(clusteredDictionary.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(itemHashes, chf)));
				}
			);
		}
	}

}
