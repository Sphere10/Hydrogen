﻿using System;
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
	public class StreamMappedDictionaryTests : StreamMappedDictionaryTestsBase {
		private const int DefaultClusterDataSize = 32;
		protected override IDisposable CreateDictionary<TKey, TValue>(int estimatedMaxByteSize, StorageType storageType, int reservedRecords, ClusteredStoragePolicy policy, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, out IStreamMappedDictionary<TKey, TValue> clusteredDictionary) {
			var disposable = base.CreateStream(storageType, estimatedMaxByteSize, out var stream);
			clusteredDictionary = new StreamMappedDictionary<TKey, TValue>(stream, DefaultClusterDataSize, keySerializer, valueSerializer, null, keyComparer, valueComparer, policy | ClusteredStoragePolicy.TrackChecksums, reservedRecords);
			return disposable;
		}

	}

}
