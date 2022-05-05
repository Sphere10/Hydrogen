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

namespace Hydrogen.Tests;


[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamMappedMerkleDictionaryTests : StreamMappedMerkleDictionaryTestsBase {
	private const int DefaultClusterSize = 256;
	private const int DefaultReservedRecords = 11;

	protected override IDisposable CreateDictionary(CHF chf, out StreamMappedMerkleDictionary<string, TestObject> streamMappedMerkleDictionary)
		=> CreateDictionaryImpl(chf, out streamMappedMerkleDictionary);

	internal static IDisposable CreateDictionaryImpl(CHF chf, out StreamMappedMerkleDictionary<string, TestObject> streamMappedMerkleDictionary) {
		var memoryStream = new MemoryStream();
		streamMappedMerkleDictionary = new StreamMappedMerkleDictionary<string, TestObject>(
			memoryStream,
			DefaultClusterSize,
			chf,
			reservedRecords: DefaultReservedRecords,
			valueComparer: new TestObjectComparer(),
			valueSerializer: new TestObjectSerializer()
		);
		return memoryStream; ;
	}


	[Test]
	public void TestHeader() {
		using (CreateDictionary(CHF.SHA2_256, out var streamMappedMerkleDictionary)) {
			Assert.That(streamMappedMerkleDictionary.Storage.Header.ClusterSize, Is.EqualTo(DefaultClusterSize));
			Assert.That(streamMappedMerkleDictionary.Storage.Header.RecordKeySize, Is.EqualTo(0));
			Assert.That(streamMappedMerkleDictionary.Storage.Header.ReservedRecords, Is.EqualTo(DefaultReservedRecords));
		}
	}

}


