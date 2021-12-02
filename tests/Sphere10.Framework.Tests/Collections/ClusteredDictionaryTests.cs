using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {


	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class ClusteredDictionaryTests : ClusteredCollectionTestsBase {
		private const int DefaultClusterDataSize = 32;

		[Test]
		public void AddNothing([Values] ClusteringType clusteringType, [Values] StorageType storageType) {
			var rng = new Random(31337);
			using (CreateDictionary(100, 10, Tools.Memory.ToBytes(1, MemoryMetric.Megabyte), clusteringType, storageType, out var clusteredDictionary)) {
				Assert.That(clusteredDictionary.Count, Is.EqualTo(0));
			}
		}


		[Test]
		public void AddOne([Values] ClusteringType clusteringType, [Values] StorageType storageType) {
			var rng = new Random(31337);
			using (CreateDictionary(100, 10, Tools.Memory.ToBytes(1, MemoryMetric.Megabyte), clusteringType, storageType, out var clusteredDictionary)) {
				clusteredDictionary.Add("alpha", new TestObject(rng));
				Assert.That(clusteredDictionary.Count, Is.EqualTo(1));
			}
		}


// Test 3: Check ContainsKey(x)

// Test 4: Check ContainsKeyValuePair

// Test 5: Remove(x)
	//- Count 0
	//- TryGetKey(x) == false


// Test 6: Add 

		

		protected IDisposable CreateDictionary(int estimatedMaxByteSize, int staticMaxItems, int staticMaxStorageBytes, ClusteringType clusteringType, StorageType storageType, out ClusteredDictionary<string, TestObject> clusteredDictionary)
			=> CreateDictionary(estimatedMaxByteSize, staticMaxItems, staticMaxStorageBytes, clusteringType, storageType, new StringSerializer(Encoding.UTF8), new TestObjectSerializer(), EqualityComparer<string>.Default, out clusteredDictionary);

		protected IDisposable CreateDictionary<TKey, TValue>(int estimatedMaxByteSize, int staticMaxItems, int staticMaxStorageBytes, ClusteringType clusteringType, StorageType storageType, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer, out ClusteredDictionary<TKey, TValue> clusteredDictionary) {
			var disposable = base.CreateStream(storageType, estimatedMaxByteSize, out var stream);
			switch (clusteringType) {
				case ClusteringType.Static:
					clusteredDictionary = new ClusteredDictionary<TKey, TValue>(DefaultClusterDataSize, staticMaxItems, staticMaxStorageBytes, stream, keySerializer, valueSerializer, keyComparer);
					break;
				case ClusteringType.Dynamic:
					clusteredDictionary = new ClusteredDictionary<TKey, TValue>(DefaultClusterDataSize, stream, keySerializer, valueSerializer, keyComparer);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(clusteringType), clusteringType, null);
			}
			return disposable;
		}

	}

}
