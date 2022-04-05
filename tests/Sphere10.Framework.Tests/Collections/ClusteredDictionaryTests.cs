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
	public class ClusteredDictionaryTests : ClusteredDictionaryTestsBase {
		private const int DefaultClusterDataSize = 32;
		protected override IDisposable CreateDictionary<TKey, TValue>(int estimatedMaxByteSize, StorageType storageType, int reservedRecords, ClusteredStoragePolicy policy, IItemSerializer<TKey> keySerializer, IItemSerializer<TValue> valueSerializer, IEqualityComparer<TKey> keyComparer, IEqualityComparer<TValue> valueComparer, out IClusteredDictionary<TKey, TValue> clusteredDictionary) {
			var disposable = base.CreateStream(storageType, estimatedMaxByteSize, out var stream);
			clusteredDictionary = new ClusteredDictionary<TKey, TValue>(stream, DefaultClusterDataSize, keySerializer, valueSerializer, null, keyComparer, valueComparer, policy | ClusteredStoragePolicy.TrackChecksums, reservedRecords);
			return disposable;
		}

	}

}
