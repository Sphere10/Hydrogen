// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamMappedMerkleDictionaryCLKTests : StreamMappedMerkleDictionaryTestsBase {
	private const int DefaultClusterSize = 256;
	private const int ConstantKeySize = 200;

	protected override IDisposable CreateTestObjectDictionary(CHF chf, out StreamMappedMerkleDictionary<string, TestObject> streamMappedMerkleDictionary)
		=> CreateDictionaryImpl(chf, new TestObjectSerializer().AsNullableSerializer(), new TestObjectEqualityComparer(), out streamMappedMerkleDictionary);

	protected override IDisposable CreateStringDictionary(CHF chf, out StreamMappedMerkleDictionary<string, string> merkleDictionary) 
		=> CreateDictionaryImpl(chf, new StringSerializer().AsNullableSerializer(), StringComparer.InvariantCulture, out merkleDictionary);

	internal static IDisposable CreateDictionaryImpl<TValue>(CHF chf, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TValue> valueComparer, out StreamMappedMerkleDictionary<string, TValue> streamMappedMerkleDictionary) {
		var memoryStream = new MemoryStream();
		streamMappedMerkleDictionary = new StreamMappedMerkleDictionary<string, TValue>(
			memoryStream,
			DefaultClusterSize,
			new StringSerializer().AsConstantSize(ConstantKeySize),
			valueSerializer,
			valueComparer: valueComparer,
			hashAlgorithm: chf,
			implementation: StreamMappedDictionaryImplementation.ConstantLengthKeyBased,
			autoLoad: true
		);
		return new Disposables(streamMappedMerkleDictionary, memoryStream);
	}

	[Test]
	public void TestHeader() {
		using (CreateTestObjectDictionary(CHF.SHA2_256, out var streamMappedMerkleDictionary)) {
			Assert.That(streamMappedMerkleDictionary.ObjectStream.Streams.Header.ClusterSize, Is.EqualTo(DefaultClusterSize));
			Assert.That(streamMappedMerkleDictionary.ObjectStream.Streams.Header.ReservedStreams, Is.EqualTo(3));
		}
	}

	[Test]
	public void DynamicKey_StaticKey_ConsistencyTest([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		const int maxItems = 100;
		var keyGens = 0;

		var keySerializer = new StringSerializer().AsConstantSize(ConstantKeySize);
		using var memStream1 = new MemoryStream();
		using var clk = new StreamMappedMerkleDictionary<string, TestObject>(
			memStream1,
			DefaultClusterSize,
			keySerializer,
			new TestObjectSerializer().AsNullableSerializer(),
			valueComparer: new TestObjectEqualityComparer(),
			hashAlgorithm: chf,
			implementation: StreamMappedDictionaryImplementation.ConstantLengthKeyBased,
			autoLoad: true 
		);

		using var memStream2 = new MemoryStream();
		using var kvp = new StreamMappedMerkleDictionary<string, TestObject>(
			memStream2,
			DefaultClusterSize,
			keySerializer,  // need to use consistent key serializer for generating same merkle tree
			new TestObjectSerializer().AsNullableSerializer(),
			valueComparer: new TestObjectEqualityComparer(),
			keyChecksummer: new ItemDigestor<string>(chf, keySerializer),
			hashAlgorithm: chf,
			implementation: StreamMappedDictionaryImplementation.KeyValueListBased,
			autoLoad: true
		);

		var rng = new Random(31337);
		var key = "test";
		var item = new TestObject(rng);
		clk.Add(key, item);
		kvp.Add(key, item);
		Assert.That(clk.MerkleTree.Root, Is.EqualTo(kvp.MerkleTree.Root));

		AssertEx.DictionaryIntegrationTest(
			clk,
			maxItems,
			(rng) => new ($"{keyGens++}_{rng.NextString(0, 100)}", new TestObject(rng)),
			iterations: 10,
			valueComparer: new TestObjectEqualityComparer(),
			endOfIterTest: () => {
				//var clkLeafs = clk.MerkleTree.GetLeafs().ToArray();
				//var kvpLeafs = kvp.MerkleTree.GetLeafs().ToArray();
				//ClassicAssert.AreEqual(clkLeafs, kvpLeafs);
				Assert.That(clk.MerkleTree.Root, Is.EqualTo(kvp.MerkleTree.Root));
			},
			expected: kvp
		);
	}


}
