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
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamMappedMerkleDictionaryTests : StreamMappedMerkleDictionaryTestsBase {
	private const int DefaultClusterSize = 256;
	private const int DefaultReservedRecords = 11;

	protected override IDisposable CreateTestObjectDictionary(CHF chf, out StreamMappedMerkleDictionary<string, TestObject> streamMappedMerkleDictionary)
		=> CreateDictionaryImpl(chf, new TestObjectSerializer().AsNullableSerializer(), new TestObjectEqualityComparer(), out streamMappedMerkleDictionary);

	protected override IDisposable CreateStringDictionary(CHF chf, out StreamMappedMerkleDictionary<string, string> merkleDictionary) 
		=> CreateDictionaryImpl(chf, new StringSerializer().AsNullableSerializer(), StringComparer.InvariantCulture, out merkleDictionary);

	internal static IDisposable CreateDictionaryImpl<TValue>(CHF chf, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TValue> valueComparer, out StreamMappedMerkleDictionary<string, TValue> streamMappedMerkleDictionary) {
		var memoryStream = new MemoryStream();
		streamMappedMerkleDictionary = new StreamMappedMerkleDictionary<string, TValue>(
			memoryStream,
			DefaultClusterSize,
			new StringSerializer(),
			valueSerializer,
			keyChecksummer: new ObjectHashCodeChecksummer<string>(),
			valueComparer: valueComparer,
			hashAlgorithm: chf,
			implementation: StreamMappedDictionaryImplementation.KeyValueListBased
		);
		if (streamMappedMerkleDictionary.RequiresLoad)
			streamMappedMerkleDictionary.Load();
		return new Disposables(streamMappedMerkleDictionary, memoryStream);
	}


	[Test]
	public void TestHeader() {
		using (CreateTestObjectDictionary(CHF.SHA2_256, out var streamMappedMerkleDictionary)) {
			Assert.That(streamMappedMerkleDictionary.ObjectStream.Streams.Header.ClusterSize, Is.EqualTo(DefaultClusterSize));
			Assert.That(streamMappedMerkleDictionary.ObjectStream.Streams.Header.ReservedStreams, Is.EqualTo(3));
		}
	}

}
