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
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
public class StreamMappedHashMerkleSetTests : SetTestsBase {
	private const int DefaultClusterDataSize = 32;

	protected override IDisposable CreateSet<TValue>(IEqualityComparer<TValue> comparer, out ISet<TValue> set) {
		var serializer = ItemSerializer<TValue>.Default;
		var stream = new MemoryStream();
		var merkleSet = new StreamMappedMerkleHashSet<TValue>(stream, DefaultClusterDataSize, serializer, CHF.SHA2_256, comparer);
		if (merkleSet.RequiresLoad)
			merkleSet.Load();
		set = merkleSet;
		return stream;
	}
}
