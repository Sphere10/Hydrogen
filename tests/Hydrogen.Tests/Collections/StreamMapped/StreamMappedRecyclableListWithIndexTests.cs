using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamMappedRecyclableListWithIndexTests : RecyclableListTestsBase {

	protected override IDisposable CreateList<T>(IItemSerializer<T> serializer, IEqualityComparer<T> comparer, out IRecyclableList<T> list) {
		var stream = new MemoryStream();
		var smrlist = StreamMappedFactory.CreateRecyclableList(
			stream,
			32, 
			serializer,
			itemChecksummer: new ObjectHashCodeChecksummer<T>(), 
			reservedStreams: 2, 
			autoLoad: true
		);
		list = smrlist;
		return new Disposables(smrlist, stream);
	}



}
