// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamMappedDictionaryTests : StreamMappedDictionaryTestsBase {
	private const int DefaultClusterDataSize = 32;
	protected override IDisposable CreateDictionary<TKey, TValue>(
		int estimatedMaxByteSize,
		StorageType storageType,
		StreamContainerPolicy policy, 
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
