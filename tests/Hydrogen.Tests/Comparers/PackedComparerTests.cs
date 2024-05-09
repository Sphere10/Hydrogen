// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class PackedComparerTests {

	[Test]
	public void TestPack() {
		IComparer<string> stringComparer = StringComparer.InvariantCultureIgnoreCase;
		Assert.That(()=> stringComparer.AsPacked(), Is.Not.Null);
	}

	[Test]
	public void TestUnpack() {
		IComparer<string> stringComparer = StringComparer.InvariantCultureIgnoreCase;
		var packedComparer = PackedComparer.Pack(stringComparer);

		var unpacked = packedComparer.Unpack<string>();

		Assert.That(unpacked, Is.SameAs(stringComparer));
	}

	[Test]
	public void TestUnpack_WrongTypeThrows() {
		IComparer<string> stringComparer = StringComparer.InvariantCultureIgnoreCase;
		var packedComparer = PackedComparer.Pack(stringComparer);
		Assert.That(() => packedComparer.Unpack<int>(), Throws.InvalidOperationException);
	}

	[Test]
	public void TestCompare_Consistency() {
		IComparer<string> stringComparer = StringComparer.InvariantCultureIgnoreCase;
		var packedComparer = stringComparer.AsPacked();
		ClassicAssert.AreEqual(stringComparer.Compare(null, null), packedComparer.Compare(null, null));
		ClassicAssert.AreEqual(stringComparer.Compare(null, "B"), packedComparer.Compare(null, "B"));
		ClassicAssert.AreEqual(stringComparer.Compare("A", null), packedComparer.Compare("A", null));
		ClassicAssert.AreEqual(stringComparer.Compare("a", "b"), packedComparer.Compare("a", "b"));
		ClassicAssert.AreEqual(stringComparer.Compare("a", "a"), packedComparer.Compare("a", "a"));
		ClassicAssert.AreEqual(stringComparer.Compare("a", "A"), packedComparer.Compare("a", "A"));
	}

	[Test]
	public void TestCompareWrongTypeThrowsCastException_1() {
		IComparer<string> stringComparer = StringComparer.InvariantCultureIgnoreCase;
		var packedComparer = stringComparer.AsPacked();
		Assert.That(() => packedComparer.Compare(1, 2), Throws.InstanceOf<InvalidCastException>());
	}

	[Test]
	public void TestCompareWrongTypeThrowsCastException_2() {
		IComparer<string> stringComparer = StringComparer.InvariantCultureIgnoreCase;
		var packedComparer = stringComparer.AsPacked();
		Assert.That(() => packedComparer.Compare("a", 2), Throws.InstanceOf<InvalidCastException>());
	}

}
