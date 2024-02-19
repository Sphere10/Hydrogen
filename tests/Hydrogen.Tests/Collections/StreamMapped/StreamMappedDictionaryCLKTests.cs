// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamMappedDictionaryCLKTests : StreamMappedDictionaryTestsBase {
	private const int DefaultClusterDataSize = 32;

	[Test]
	public void TestHeader() {
		var dict = StreamMappedFactory.CreateDictionaryClk<string, string>(
			new MemoryStream(),
			21,
			new StringSerializer().AsReferenceSerializer().AsConstantSize(11),
			new StringSerializer(),
			reservedStreamCount: 33,
			policy: ClusteredStreamsPolicy.Performance);
		if (dict.RequiresLoad)
			dict.Load();
		Assert.That(dict.ObjectStream.Streams.Header.ClusterSize, Is.EqualTo(21));
		Assert.That(dict.ObjectStream.Streams.Header.ReservedStreams, Is.EqualTo(33));
	}

	protected override IDisposable CreateDictionary<TKey, TValue>(int estimatedMaxByteSize, StorageType storageType, ClusteredStreamsPolicy policy, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer,
	                                                              IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, out IStreamMappedDictionary<TKey, TValue> clusteredDictionary) {
		var disposable = base.CreateStream(storageType, estimatedMaxByteSize, out var stream);
		clusteredDictionary = StreamMappedFactory.CreateDictionaryClk<TKey, TValue>(stream,
			DefaultClusterDataSize,
			keySerializer.AsConstantSize(256),
			valueSerializer,
			keyComparer,
			valueComparer,
			policy
		);
		Debug.Assert(clusteredDictionary is StreamMappedDictionaryCLK<TKey, TValue>, "Not correct implementation");
		if (clusteredDictionary.RequiresLoad)
			clusteredDictionary.Load();
		return new Disposables(clusteredDictionary, disposable);
	}

}
