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


	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class ClusteredDictionaryWithListingReuseTests : ClusteredCollectionTestsBase {
		private const int DefaultStaticMaxBytesSize = 400+256;
		private const int DefaultStaticMaxItems = 10;
		private const int DefaultClusterDataSize = 32;

		[Test]
		public void AddNothing([Values] ClusteringType clusteringType, [Values] StorageType storageType, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(DefaultStaticMaxBytesSize, DefaultStaticMaxItems, clusteringType, storageType, out var clusteredDictionary)) {
				Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void AddOne([Values] ClusteringType clusteringType, [Values] StorageType storageType, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(DefaultStaticMaxBytesSize, DefaultStaticMaxItems, clusteringType, storageType, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				Assert.That(clusteredDictionary.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ReuseListing([Values] ClusteringType clusteringType, [Values] StorageType storageType, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(DefaultStaticMaxBytesSize, DefaultStaticMaxItems, clusteringType, storageType, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				clusteredDictionary.Add(key, new TestObject(rng));
				Assert.That(clusteredDictionary.Count, Is.EqualTo(1));
			}
		}

		[Test]
		public void ContainsKey([Values] ClusteringType clusteringType, [Values] StorageType storageType, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(DefaultStaticMaxBytesSize, DefaultStaticMaxItems, clusteringType, storageType, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				Assert.That(clusteredDictionary.ContainsKey(key), Is.True);
			}
		}


		[Test]
		public void DoesNotContainKeyAfterRemove([Values] ClusteringType clusteringType, [Values] StorageType storageType, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(DefaultStaticMaxBytesSize, DefaultStaticMaxItems, clusteringType, storageType, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				Assert.That(clusteredDictionary.ContainsKey(key), Is.False);
			}
		}


		[Test]
		public void ContainsKeyValuePair([Values] ClusteringType clusteringType, [Values] StorageType storageType, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(DefaultStaticMaxBytesSize, DefaultStaticMaxItems, clusteringType, storageType, out var clusteredDictionary)) {
				var value = new TestObject(rng);
				var kvp = KeyValuePair.Create(key, value);
				clusteredDictionary.Add(kvp);
				Assert.That(clusteredDictionary.Contains(kvp), Is.True);
			}
		}

		[Test]
		public void DoesNotContainKeyValuePair_SameKeyDifferentValue([Values] ClusteringType clusteringType, [Values] StorageType storageType, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(DefaultStaticMaxBytesSize, DefaultStaticMaxItems, clusteringType, storageType, out var clusteredDictionary)) {
				var value = new TestObject(rng);
				var kvp = KeyValuePair.Create(key, value);
				clusteredDictionary.Add(kvp);
				value.A += "1";
				Assert.That(clusteredDictionary.Contains(kvp), Is.False);
			}
		}

		[Test]
		public void RemoveByKey([Values] ClusteringType clusteringType, [Values] StorageType storageType, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(DefaultStaticMaxBytesSize, DefaultStaticMaxItems, clusteringType, storageType, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void RemoveByKeyValuePair([Values] ClusteringType clusteringType, [Values] StorageType storageType, [Values("alpha", "Unicode😊😊😊", "")] string key) {
			var rng = new Random(31337);
			using (CreateDictionary(DefaultStaticMaxBytesSize, DefaultStaticMaxItems, clusteringType, storageType, out var clusteredDictionary)) {
				clusteredDictionary.Add(key, new TestObject(rng));
				clusteredDictionary.Remove(key);
				Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public void IntegrationTests_Heavy([Values] ClusteringType clusteringType, [Values(StorageType.MemoryStream)] StorageType storageType, [Values(250)] int maxItems) {
			var keyGens = 0;
			using (CreateDictionary(Tools.Memory.ToBytes(1, MemoryMetric.Megabyte), maxItems, clusteringType, storageType, out var clusteredDictionary)) {
				AssertEx.DictionaryIntegrationTest(
					clusteredDictionary,
					maxItems,
					(rng) => ($"{keyGens++}_{rng.NextString(0, 100)}", new TestObject(rng)),
					iterations: 250,
					valueComparer: new TestObjectComparer()
				);
			}
		}


		[Test]
		public void IntegrationTests([Values] ClusteringType clusteringType, [Values] StorageType storageType, [Values(23)] int maxItems) {
			var keyGens = 0;
			using (CreateDictionary(Tools.Memory.ToBytes(1, MemoryMetric.Megabyte), maxItems, clusteringType, storageType, out var clusteredDictionary)) {
				AssertEx.DictionaryIntegrationTest(
					clusteredDictionary,
					maxItems,
					(rng) => ($"{keyGens++}_{rng.NextString(0, 100)}", new TestObject(rng)),
					iterations: 10,
					valueComparer: new TestObjectComparer()
				);
			}
		}

		protected IDisposable CreateDictionary(int staticMaxByteSize, int staticMaxItems,  ClusteringType clusteringType, StorageType storageType, out ClusteredDictionaryWithListingReuse<string, TestObject> clusteredDictionary)
			=> CreateDictionary(staticMaxByteSize, staticMaxItems, clusteringType, storageType, new StringSerializer(Encoding.UTF8), new TestObjectSerializer(), EqualityComparer<string>.Default, out clusteredDictionary);

		protected IDisposable CreateDictionary<TKey, TValue>(int staticMaxByteSize, int staticMaxItems, ClusteringType clusteringType, StorageType storageType, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer, out ClusteredDictionaryWithListingReuse<TKey, TValue> clusteredDictionary) {
			var disposable = base.CreateStream(storageType, staticMaxByteSize, out var stream);
			switch (clusteringType) {
				case ClusteringType.Static:
					clusteredDictionary = new ClusteredDictionaryWithListingReuse<TKey, TValue>(DefaultClusterDataSize, staticMaxItems, staticMaxByteSize, stream, keySerializer, valueSerializer, keyComparer);
					break;
				case ClusteringType.Dynamic:
					clusteredDictionary = new ClusteredDictionaryWithListingReuse<TKey, TValue>(DefaultClusterDataSize, stream, keySerializer, valueSerializer, keyComparer);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(clusteringType), clusteringType, null);
			}
			return disposable;
		}

	}

}
