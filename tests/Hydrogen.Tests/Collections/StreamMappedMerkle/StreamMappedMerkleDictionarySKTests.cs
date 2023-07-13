// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using NUnit.Framework;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamMappedMerkleDictionarySKTests : StreamMappedMerkleDictionaryTestsBase {
	private const int DefaultClusterSize = 256;
	private const int StaticKeySize = 200;
	private const int DefaultReservedRecords = 33;

	protected override IDisposable CreateDictionary(CHF chf, out StreamMappedMerkleDictionary<string, TestObject> streamMappedMerkleDictionary)
		=> CreateDictionaryImpl(chf, out streamMappedMerkleDictionary);

	internal static IDisposable CreateDictionaryImpl(CHF chf, out StreamMappedMerkleDictionary<string, TestObject> streamMappedMerkleDictionary) {
		var memoryStream = new MemoryStream();
		streamMappedMerkleDictionary = new StreamMappedMerkleDictionary<string, TestObject>(
			memoryStream,
			DefaultClusterSize,
			new StringSerializer().ToStaticSizeSerializer(StaticKeySize),
			chf,
			reservedRecords: DefaultReservedRecords,
			valueComparer: new TestObjectComparer(),
			valueSerializer: new TestObjectSerializer()
		);
		if (streamMappedMerkleDictionary.RequiresLoad)
			streamMappedMerkleDictionary.Load();
		return memoryStream;
		;
	}

	[Test]
	public void TestHeader() {
		using (CreateDictionary(CHF.SHA2_256, out var streamMappedMerkleDictionary)) {
			Assert.That(streamMappedMerkleDictionary.Storage.Header.ClusterSize, Is.EqualTo(DefaultClusterSize));
			Assert.That(streamMappedMerkleDictionary.Storage.Header.RecordKeySize, Is.EqualTo(StaticKeySize));
			Assert.That(streamMappedMerkleDictionary.Storage.Header.ReservedRecords, Is.EqualTo(DefaultReservedRecords));
		}
	}

	[Test]
	public void DynamicKey_StaticKey_ConsistencyTest([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		const int maxItems = 100;
		var keyGens = 0;
		using (StreamMappedMerkleDictionaryTests.CreateDictionaryImpl(chf, out var referenceDictionary))
		using (CreateDictionary(chf, out var clusteredDictionary)) {

			AssertEx.DictionaryIntegrationTest(
				clusteredDictionary,
				maxItems,
				(rng) => ($"{keyGens++}_{rng.NextString(0, 100)}", new TestObject(rng)),
				iterations: 10,
				valueComparer: new TestObjectComparer(),
				endOfIterTest: () => { Assert.That(clusteredDictionary.MerkleTree.Root, Is.EqualTo(referenceDictionary.MerkleTree.Root)); },
				expected: referenceDictionary
			);
		}
	}


}
