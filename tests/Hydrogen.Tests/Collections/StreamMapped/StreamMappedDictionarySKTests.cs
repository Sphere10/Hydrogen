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
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Hydrogen.NUnit;
using Tools;

namespace Hydrogen.Tests {


	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class StreamMappedDictionarySKTests : StreamMappedDictionaryTestsBase {
		private const int DefaultClusterDataSize = 32;

		[Test]
		public void TestHeader() {
			var dict = new StreamMappedDictionarySK<string, string>(new MemoryStream(), 21, new StringSerializer().ToStaticSizeSerializer(11), new StringSerializer(), reservedRecords: 33, policy: ClusteredStoragePolicy.BlobOptimized);
			Assert.That(dict.Storage.Header.ClusterSize, Is.EqualTo(21));
			Assert.That(dict.Storage.Header.RecordKeySize, Is.EqualTo(11));
			Assert.That(dict.Storage.Header.ReservedRecords, Is.EqualTo(33));
		}

		protected override IDisposable CreateDictionary<TKey, TValue>(int estimatedMaxByteSize, StorageType storageType, int reservedRecords, ClusteredStoragePolicy policy, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, out IStreamMappedDictionary<TKey, TValue> clusteredDictionary) {
			var disposable = base.CreateStream(storageType, estimatedMaxByteSize, out var stream);
			clusteredDictionary = new StreamMappedDictionarySK<TKey, TValue>(stream, DefaultClusterDataSize, keySerializer.ToStaticSizeSerializer(256), valueSerializer, null, keyComparer, valueComparer, policy | ClusteredStoragePolicy.TrackChecksums | ClusteredStoragePolicy.TrackKey, reservedRecords);
			return disposable;
		}

	}

}
