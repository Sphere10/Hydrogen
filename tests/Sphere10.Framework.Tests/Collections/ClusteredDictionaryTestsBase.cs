using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
using NUnit.Framework;
using Sphere10.Framework.NUnit;
using Tools;

namespace Sphere10.Framework.Tests {
	
	public abstract class ClusteredDictionaryTestsBase : StreamPersistedTestsBase {
		private const int EstimatedTestObjectSize = 400 + 256;
		private const int ReservedRecordsInStorage = 11;

		[Test]
		public void AddNothing([ClusteredStorageStorageTypeValues] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values(0, ReservedRecordsInStorage)] int reserved) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
				Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void AddOne([ClusteredStorageStorageTypeValues] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key, [Values(0, ReservedRecordsInStorage)] int reserved) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				Assert.That(clusteredDictionary.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ReuseRecord([ClusteredStorageStorageTypeValues] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key, [Values(0, ReservedRecordsInStorage)] int reserved) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				clusteredDictionary.Add(key, new TestObject(rng));
				Assert.That(clusteredDictionary.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ContainsKey([ClusteredStorageStorageTypeValues] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key, [Values(0, ReservedRecordsInStorage)] int reserved) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				Assert.That(clusteredDictionary.ContainsKey(key), Is.True);
			}
		}

		[Test]
		public void DoesNotContainKeyAfterRemove([ClusteredStorageStorageTypeValues] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key, [Values(0, ReservedRecordsInStorage)] int reserved) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				Assert.That(clusteredDictionary.ContainsKey(key), Is.False);
			}
		}

		[Test]
		public void ContainsKeyValuePair([ClusteredStorageStorageTypeValues] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key, [Values(0, ReservedRecordsInStorage)] int reserved) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
				var value = new TestObject(rng);
				var kvp = KeyValuePair.Create(key, value);
				clusteredDictionary.Add(kvp);
				Assert.That(clusteredDictionary.Contains(kvp), Is.True);
			}
		}

		[Test]
		public void DoesNotContainKeyValuePair_SameKeyDifferentValue([ClusteredStorageStorageTypeValues] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key, [Values(0, ReservedRecordsInStorage)] int reserved) {
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
		public void RemoveByKey([ClusteredStorageStorageTypeValues] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key, [Values(0, ReservedRecordsInStorage)] int reserved) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void RemoveByKeyValuePair([ClusteredStorageStorageTypeValues] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values("alpha", "Unicode😊😊😊", "")] string key, [Values(0, ReservedRecordsInStorage)] int reserved) {
			var rng = new Random(31337);
			using (CreateDictionary(EstimatedTestObjectSize * 1, storageType, reserved, policy, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
			}
		}

#if DEBUG
		[Test]
		public void IntegrationTests_Heavy([Values(StorageType.MemoryStream)] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values(250)] int maxItems, [Values(0, ReservedRecordsInStorage)] int reserved) {
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
		public void IntegrationTests([ClusteredStorageStorageTypeValues] StorageType storageType, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy, [Values(23)] int maxItems, [Values(0, ReservedRecordsInStorage)] int reserved) {
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

		protected IDisposable CreateDictionary(int estimatedMaxByteSize, StorageType storageType, int reservedRecords, ClusteredStoragePolicy policy, out IClusteredDictionary<string, TestObject> clusteredDictionary) {
			var disposables = CreateDictionary(estimatedMaxByteSize, storageType, reservedRecords, policy, new StringSerializer(Encoding.UTF8), new TestObjectSerializer(), EqualityComparer<string>.Default, new TestObjectComparer(), out clusteredDictionary);
			if (clusteredDictionary.RequiresLoad)
				clusteredDictionary.Load();
			return disposables;
		}

		protected abstract IDisposable CreateDictionary<TKey, TValue>(int estimatedMaxByteSize, StorageType storageType, int reservedRecords, ClusteredStoragePolicy policy, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, out IClusteredDictionary<TKey, TValue> clusteredDictionary);

	}

}
