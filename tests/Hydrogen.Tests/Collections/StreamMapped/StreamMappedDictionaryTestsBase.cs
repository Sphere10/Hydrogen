// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

public abstract class StreamMappedDictionaryTestsBase : StreamPersistedCollectionTestsBase {
	private const int EstimatedTestObjectSize = 400 + 256;
	private const int ReservedRecordsInStorage = 11;

	[Test]
	public void AddNothing([StreamContainerStorageTypeValues] StorageType storageType, [StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values(0, ReservedRecordsInStorage)] int reserved) {
		var rng = new Random(31337);
		using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
			Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
		}
	}

	[Test]
	public void AddOne([StreamContainerStorageTypeValues] StorageType storageType, [StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key,
	                   [Values(0, ReservedRecordsInStorage)] int reserved) {
		var rng = new Random(31337);
		using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
			clusteredDictionary.Add(key, new TestObject(rng));
			Assert.That(clusteredDictionary.Count, Is.EqualTo(1));
		}
	}

	[Test]
	public void ReuseRecord([StreamContainerStorageTypeValues] StorageType storageType, [StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key,
	                        [Values(0, ReservedRecordsInStorage)] int reserved) {
		var rng = new Random(31337);
		using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
			clusteredDictionary.Add(key, new TestObject(rng));
			clusteredDictionary.Remove(key);
			clusteredDictionary.Add(key, new TestObject(rng));
			Assert.That(clusteredDictionary.Count, Is.EqualTo(1));
		}
	}

	[Test]
	public void ContainsKey([StreamContainerStorageTypeValues] StorageType storageType, [StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key,
	                        [Values(0, ReservedRecordsInStorage)] int reserved) {
		var rng = new Random(31337);
		using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
			clusteredDictionary.Add(key, new TestObject(rng));
			Assert.That(clusteredDictionary.ContainsKey(key), Is.True);
		}
	}

	[Test]
	public void DoesNotContainKeyAfterRemove([StreamContainerStorageTypeValues] StorageType storageType, [StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key,
	                                         [Values(0, ReservedRecordsInStorage)] int reserved) {
		var rng = new Random(31337);
		using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
			clusteredDictionary.Add(key, new TestObject(rng));
			clusteredDictionary.Remove(key);
			Assert.That(clusteredDictionary.ContainsKey(key), Is.False);
		}
	}

	[Test]
	public void ContainsKeyValuePair([StreamContainerStorageTypeValues] StorageType storageType, [StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key,
	                                 [Values(0, ReservedRecordsInStorage)] int reserved) {
		var rng = new Random(31337);
		using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
			var value = new TestObject(rng);
			var kvp = KeyValuePair.Create(key, value);
			clusteredDictionary.Add(kvp);
			Assert.That(clusteredDictionary.Contains(kvp), Is.True);
		}
	}

	[Test]
	public void DoesNotContainKeyValuePair_SameKeyDifferentValue([StreamContainerStorageTypeValues] StorageType storageType, [StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key,
	                                                             [Values(0, ReservedRecordsInStorage)] int reserved) {
		var rng = new Random(31337);
		using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
			var value = new TestObject(rng);
			var kvp = KeyValuePair.Create(key, value);
			clusteredDictionary.Add(kvp);
			value.A += "1";
			Assert.That(clusteredDictionary.Contains(kvp), Is.False);
		}
	}

	[Test]
	public void RemoveByKey([StreamContainerStorageTypeValues] StorageType storageType, [StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key,
	                        [Values(0, ReservedRecordsInStorage)] int reserved) {
		var rng = new Random(31337);
		using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
			clusteredDictionary.Add(key, new TestObject(rng));
			clusteredDictionary.Remove(key);
			Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
		}
	}

	[Test]
	public void RemoveByKeyValuePair([StreamContainerStorageTypeValues] StorageType storageType, [StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key,
	                                 [Values(0, ReservedRecordsInStorage)] int reserved) {
		var rng = new Random(31337);
		using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
			clusteredDictionary.Add(key, new TestObject(rng));
			clusteredDictionary.Remove(key);
			Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
		}
	}

#if DEBUG
	[Test]
	public void IntegrationTests_Heavy([Values(StorageType.MemoryStream)] StorageType storageType, [StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values(250)] int maxItems, [Values(0, ReservedRecordsInStorage)] int reserved) {
		var keyGens = 0;
		using (CreateDictionary(EstimatedTestObjectSize * maxItems, storageType, reserved, policy, out var clusteredDictionary)) {
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
	public void IntegrationTests([StreamContainerStorageTypeValues] StorageType storageType, [StreamContainerPolicyTestValues] StreamContainerPolicy policy, [Values(23)] int maxItems, [Values(0, ReservedRecordsInStorage)] int reserved) {
		var keyGens = 0;
		using (CreateDictionary(maxItems * EstimatedTestObjectSize, storageType, reserved, policy, out var clusteredDictionary)) {
			AssertEx.DictionaryIntegrationTest(
				clusteredDictionary,
				maxItems,
				(rng) => ($"{keyGens++}_{rng.NextString(0, 100)}", new TestObject(rng)),
				iterations: 10,
				valueComparer: new TestObjectComparer()
			);
		}
	}

	protected IDisposable CreateDictionary(int estimatedMaxByteSize, StorageType storageType, int reservedRecords, StreamContainerPolicy policy, out IStreamMappedDictionary<string, TestObject> clusteredDictionary) {
		var disposables = CreateDictionary(estimatedMaxByteSize,
			storageType,
			reservedRecords,
			policy,
			new StringSerializer(Encoding.UTF8),
			new TestObjectSerializer(),
			EqualityComparer<string>.Default,
			new TestObjectComparer(),
			out clusteredDictionary);
		if (clusteredDictionary.RequiresLoad)
			clusteredDictionary.Load();
		return disposables;
	}

	protected abstract IDisposable CreateDictionary<TKey, TValue>(int estimatedMaxByteSize, StorageType storageType, int reservedRecords, StreamContainerPolicy policy, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer,
	                                                              IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, out IStreamMappedDictionary<TKey, TValue> clusteredDictionary);

}
