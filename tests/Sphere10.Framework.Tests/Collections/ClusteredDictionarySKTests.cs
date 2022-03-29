﻿using System;
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
	public class ClusteredDictionarySKTests : ClusteredDictionaryTestsBase {
		private const int DefaultClusterDataSize = 32;

		[Test]
		public void TestHeader() {
			var dict = new ClusteredDictionarySK<string, string>(new MemoryStream(), 21, new StringSerializer().ToStaticSizeSerializer(11), new StringSerializer(), reservedRecords: 33, policy: ClusteredStoragePolicy.BlobOptimized);
			Assert.That(dict.Storage.Header.ClusterSize, Is.EqualTo(21));
			Assert.That(dict.Storage.Header.RecordKeySize, Is.EqualTo(11));
			Assert.That(dict.Storage.Header.ReservedRecords, Is.EqualTo(33));
		}

		protected override IDisposable CreateDictionary<TKey, TValue>(int estimatedMaxByteSize, StorageType storageType, int reservedRecords, ClusteredStoragePolicy policy, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, out IClusteredDictionary<TKey, TValue> clusteredDictionary) {
			var disposable = base.CreateStream(storageType, estimatedMaxByteSize, out var stream);
			clusteredDictionary = new ClusteredDictionarySK<TKey, TValue>(stream, DefaultClusterDataSize, keySerializer.ToStaticSizeSerializer(256), valueSerializer, null, keyComparer, valueComparer, policy | ClusteredStoragePolicy.TrackChecksums | ClusteredStoragePolicy.TrackKey);
			return disposable;
		}

	}

}
