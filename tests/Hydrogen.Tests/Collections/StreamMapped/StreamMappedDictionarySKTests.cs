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
public class StreamMappedDictionarySKTests : StreamMappedDictionaryTestsBase {
	private const int DefaultClusterDataSize = 32;

	[Test]
	public void TestHeader() {
		var dict = new StreamMappedDictionarySK<string, string>(new MemoryStream(),
			21,
			new StringSerializer().AsStaticSizeSerializer(11, SizeDescriptorStrategy.UseUInt32),
			new StringSerializer(),
			reservedRecords: 33,
			policy: StreamContainerPolicy.BlobOptimized);
		if (dict.RequiresLoad)
			dict.Load();
		Assert.That(dict.ObjectContainer.StreamContainer.Header.ClusterSize, Is.EqualTo(21));
		Assert.That(dict.ObjectContainer.StreamContainer.Header.StreamDescriptorKeySize, Is.EqualTo(11));
		Assert.That(dict.ObjectContainer.StreamContainer.Header.ReservedStreams, Is.EqualTo(33));
	}

	protected override IDisposable CreateDictionary<TKey, TValue>(int estimatedMaxByteSize, StorageType storageType, int reservedRecords, StreamContainerPolicy policy, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer,
	                                                              IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, out IStreamMappedDictionary<TKey, TValue> clusteredDictionary) {
		var disposable = base.CreateStream(storageType, estimatedMaxByteSize, out var stream);
		clusteredDictionary = new StreamMappedDictionarySK<TKey, TValue>(stream,
			DefaultClusterDataSize,
			keySerializer.AsStaticSizeSerializer(256, SizeDescriptorStrategy.UseUInt32),
			valueSerializer,
			null,
			keyComparer,
			valueComparer,
			policy | StreamContainerPolicy.TrackChecksums | StreamContainerPolicy.TrackKey,
			reservedRecords);
		if (clusteredDictionary.RequiresLoad)
			clusteredDictionary.Load();
		return disposable;
	}

}
