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
using NUnit.Framework.Legacy;

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
		var bytes = serializer.SerializeBytesLE(testObject);
		var testObject2 = serializer.DeserializeBytesLE(bytes);
		var comparer = new TestObjectEqualityComparer();
		Assert.That(comparer.Equals(testObject, testObject2), Is.True);
	}

	
	[Test]
	public void TestTestObjectSerializer_2() {
		var rng = new Random(31337);
		var testObject = new TestObject(rng);
		testObject.A = null;

		var serializer = new TestObjectSerializer();
		var bytes = serializer.SerializeBytesLE(testObject);
		var testObject2 = serializer.DeserializeBytesLE(bytes);
		var comparer = new TestObjectEqualityComparer();
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

			var comparer = new TestObjectEqualityComparer();
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
	public void SampleWalkthroughTest([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		using (CreateStringDictionary(chf, out var merkleDict)) {
			// merkle root should be empty
			Assert.That(merkleDict.MerkleTree.Root, Is.Null);
			Assert.That(merkleDict.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new byte[][] { })));

			//  add alpha
			merkleDict.Add("alpha", "");
			var rootAlpha = merkleDict.MerkleTree.Root;
			var alphaLeafHash = Hashers.JoinHash(chf, Hashers.Hash(chf, merkleDict.ReadKeyBytes(0)), Hashers.Hash(chf, merkleDict.ReadValueBytes(0))); 
			Assert.That(alphaLeafHash, Is.EqualTo( merkleDict.MerkleTree.GetNodeAt(MerkleCoordinate.LeafAt(0)).Hash));
			Assert.That(merkleDict.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new [] { alphaLeafHash }, chf)));

			// add beta
			merkleDict.Add("beta", null);
			var rootAlphaBeta = merkleDict.MerkleTree.Root;
			var betaLeafHash = Hashers.JoinHash(chf, Hashers.Hash(chf, merkleDict.ReadKeyBytes(1)), Hashers.ZeroHash(chf)); 
			Assert.That(betaLeafHash, Is.EqualTo( merkleDict.MerkleTree.GetNodeAt(MerkleCoordinate.LeafAt(1)).Hash));
			Assert.That(merkleDict.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new [] { alphaLeafHash, betaLeafHash }, chf)));

			// add gamma
			merkleDict.Add("gamma", "value");
			var rootAlphaBetaGamma = merkleDict.MerkleTree.Root;
			var gammaLeafHash = Hashers.JoinHash(chf, Hashers.Hash(chf, merkleDict.ReadKeyBytes(2)), Hashers.Hash(chf, merkleDict.ReadValueBytes(2))); 
			Assert.That(gammaLeafHash, Is.EqualTo(merkleDict.MerkleTree.GetNodeAt(MerkleCoordinate.LeafAt(2)).Hash));
			Assert.That(merkleDict.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new [] { alphaLeafHash, betaLeafHash, gammaLeafHash }, chf)));
		
			merkleDict.Clear();
			Assert.That(merkleDict.MerkleTree.Root, Is.Null);
			

			// rebuild and test consistency
			merkleDict.Add("alpha", "");
			Assert.That(merkleDict.MerkleTree.Root, Is.EqualTo(rootAlpha));

			merkleDict.Add("beta", null);
			Assert.That(merkleDict.MerkleTree.Root, Is.EqualTo(rootAlphaBeta));

			merkleDict.Add("gamma", "value");
			Assert.That(merkleDict.MerkleTree.Root, Is.EqualTo(rootAlphaBetaGamma));

			// remove beta
			merkleDict.Remove("beta");
			betaLeafHash = Hashers.ZeroHash(chf);
			Assert.That(betaLeafHash, Is.EqualTo( merkleDict.MerkleTree.GetNodeAt(MerkleCoordinate.LeafAt(1)).Hash));
			Assert.That(merkleDict.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new [] { alphaLeafHash, betaLeafHash, gammaLeafHash }, chf)));
		}
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

			var comparer = new TestObjectEqualityComparer();
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
				valueComparer: new TestObjectEqualityComparer()
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
				valueComparer: new TestObjectEqualityComparer(),
				endOfIterTest: () => {
					// Manually test the merkle root
					var itemSerializer = new TestObjectSerializer();
					
					var itemHashes = Enumerable.Range(0, (int)clusteredDictionary.ObjectStream.Count).Select(i => {

						if (clusteredDictionary.ObjectStream.GetItemDescriptor(i).Traits.HasFlag(ClusteredStreamTraits.Reaped))
							return Hashers.ZeroHash(chf);

						var keyHash = Hashers.HashWithNullSupport(chf, clusteredDictionary.ReadKeyBytes(i));
						var valueHash = Hashers.HashWithNullSupport(chf, clusteredDictionary.ReadValueBytes(i));
						return Hashers.JoinHash(chf, keyHash, valueHash);
					}).ToArray();
					var treeLeaves = clusteredDictionary.MerkleTree.GetLeafs().ToArray();
					ClassicAssert.AreEqual(itemHashes, treeLeaves);
					Assert.That(clusteredDictionary.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(itemHashes, chf)));
				}
			);
		}
	}

}
