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
public class StreamMappedDictionaryTests : StreamMappedDictionaryTestsBase {
	private const int DefaultClusterDataSize = 32;


	[Test]
	public void SupportsKeysWithCollidingChecksum() {
		using var stream = new MemoryStream();
		var dict = StreamMappedFactory.CreateDictionaryKvp<int, string>(
			stream,
			4096,
			keyChecksummer: new ActionChecksum<int>( _ => 1),
			autoLoad: true
		);

		dict[1] = "hello";
		dict[11] = "world";

		Assert.That(dict.ContainsKey(1), Is.True);
		Assert.That(dict.ContainsKey(11), Is.True);

		Assert.That(dict[1], Is.EqualTo("hello"));
		Assert.That(dict[11], Is.EqualTo("world"));
	}

	protected override IDisposable CreateDictionary<TKey, TValue>(
		int estimatedMaxByteSize,
		StorageType storageType,
		ClusteredStreamsPolicy policy, 
		IItemSerializer<TKey> keySerializer, 
		IItemSerializer<TValue> valueSerializer,
		IEqualityComparer<TKey> keyComparer, 
		IEqualityComparer<TValue> valueComparer, 
		out IStreamMappedDictionary<TKey, TValue> clusteredDictionary
	) {
		var disposable = base.CreateStream(storageType, estimatedMaxByteSize, out var stream);
		clusteredDictionary = StreamMappedFactory.CreateDictionaryKvp<TKey, TValue>(stream, DefaultClusterDataSize, keySerializer, valueSerializer, keyComparer, valueComparer, null, policy, Endianness.LittleEndian, false);
		return new Disposables(clusteredDictionary, disposable);
	}

}
