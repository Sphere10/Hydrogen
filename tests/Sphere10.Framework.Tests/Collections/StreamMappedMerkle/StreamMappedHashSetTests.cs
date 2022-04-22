﻿using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests;

[TestFixture]
public class StreamMappedHashMerkleSetTests : SetTestsBase {
	private const int DefaultClusterDataSize = 32;

	protected override IDisposable CreateSet<TValue>(IEqualityComparer<TValue> comparer, out ISet<TValue> set) {
		var serializer = ItemSerializer<TValue>.Default;
		var stream = new MemoryStream();
		set = new StreamMappedMerkleHashSet<TValue>(stream, DefaultClusterDataSize, serializer, CHF.SHA2_256, comparer);
		return stream;
	}
}
