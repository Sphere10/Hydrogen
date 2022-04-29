using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

[TestFixture]
public class StreamMappedHashSetTests : SetTestsBase {
	private const int DefaultClusterDataSize = 32;

	protected override IDisposable CreateSet<TValue>(IEqualityComparer<TValue> comparer, out ISet<TValue> set) {
		var serializer = ItemSerializer<TValue>.Default;
		var stream = new MemoryStream();
		set = new StreamMappedHashSet<TValue>(stream, DefaultClusterDataSize, serializer, CHF.SHA2_256, comparer);
		return stream;
	}
}
