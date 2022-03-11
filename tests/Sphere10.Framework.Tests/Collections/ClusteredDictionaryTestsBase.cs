using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Sphere10.Framework.NUnit;
using Tools;

namespace Sphere10.Framework.Tests {
	
	public abstract class ClusteredDictionaryTestsBase : StreamPersistedTestsBase {
		private const int EstimatedTestObjectSize = 400 + 256;

		[Test]
		public void AddNothing([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, policy, out var clusteredDictionary)) {
				Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void AddOne([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, policy, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				Assert.That(clusteredDictionary.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ReuseRecord([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, policy, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				clusteredDictionary.Add(key, new TestObject(rng));
				Assert.That(clusteredDictionary.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ContainsKey([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, policy, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				Assert.That(clusteredDictionary.ContainsKey(key), Is.True);
			}
		}

		[Test]
		public void DoesNotContainKeyAfterRemove([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, policy, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				Assert.That(clusteredDictionary.ContainsKey(key), Is.False);
			}
		}

		[Test]
		public void ContainsKeyValuePair([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, policy, out var clusteredDictionary)) {
				var value = new TestObject(rng);
				var kvp = KeyValuePair.Create(key, value);
				clusteredDictionary.Add(kvp);
				Assert.That(clusteredDictionary.Contains(kvp), Is.True);
			}
		}

		[Test]
		public void DoesNotContainKeyValuePair_SameKeyDifferentValue([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, policy, out var clusteredDictionary)) {
				var value = new TestObject(rng);
				var kvp = KeyValuePair.Create(key, value);
				clusteredDictionary.Add(kvp);
				value.A += "1";
				Assert.That(clusteredDictionary.Contains(kvp), Is.False);
			}
		}

		[Test]
		public void RemoveByKey([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, policy, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void RemoveByKeyValuePair([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, policy, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
			}
		}

#if DEBUG
		[Test]
		public void IntegrationTests_Heavy([Values(StorageType.MemoryStream)] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values(250)] int maxItems) {
			var keyGens = 0;
			using (CreateDictionary(EstimatedTestObjectSize * maxItems, storageType, policy, out var clusteredDictionary)) {
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
		public void IntegrationTests([Values] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values(23)] int maxItems) {
			var keyGens = 0;
			using (CreateDictionary(maxItems * EstimatedTestObjectSize, storageType, policy, out var clusteredDictionary)) {
				AssertEx.DictionaryIntegrationTest(
					clusteredDictionary,
					maxItems,
					(rng) => ($"{keyGens++}_{rng.NextString(0, 100)}", new TestObject(rng)),
					iterations: 10,
					valueComparer: new TestObjectComparer()
				);
			}
		}

		protected IDisposable CreateDictionary(int estimatedMaxByteSize, StorageType storageType,  ClusteredStoragePolicy policy, out IClusteredDictionary<string, TestObject> clusteredDictionary)
			=> CreateDictionary(estimatedMaxByteSize, storageType, policy, new StringSerializer(Encoding.UTF8), new TestObjectSerializer(), EqualityComparer<string>.Default, new TestObjectComparer(), out clusteredDictionary);

		protected abstract IDisposable CreateDictionary<TKey, TValue>(int estimatedMaxByteSize, StorageType storageType, ClusteredStoragePolicy policy, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, out IClusteredDictionary<TKey, TValue> clusteredDictionary);

	}

}
