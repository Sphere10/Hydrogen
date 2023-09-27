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

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class PackedEqualityComparerTests {

	[Test]
	public void TestPack() {
		IEqualityComparer<string> stringComparer = StringComparer.InvariantCultureIgnoreCase;
		Assert.That(()=> stringComparer.AsPacked(), Is.Not.Null);
	}

	[Test]
	public void TestUnpack() {
		IEqualityComparer<string> stringComparer = StringComparer.InvariantCultureIgnoreCase;
		var packedComparer = PackedEqualityComparer.Pack(stringComparer);

		var unpacked = packedComparer.Unpack<string>();

		Assert.That(unpacked, Is.SameAs(stringComparer));
	}

	[Test]
	public void TestUnpack_WrongTypeThrows() {
		IEqualityComparer<string> stringComparer = StringComparer.InvariantCultureIgnoreCase;
		var packedComparer = PackedEqualityComparer.Pack(stringComparer);
		Assert.That(() => packedComparer.Unpack<int>(), Throws.InvalidOperationException);
	}

	[Test]
	public void TestEquals_Consistency() {
		IEqualityComparer<string> stringComparer = StringComparer.InvariantCultureIgnoreCase;
		var packedComparer = stringComparer.AsPacked();
		Assert.AreEqual(stringComparer.Equals(null, null), packedComparer.Equals(null, null));
		Assert.AreEqual(stringComparer.Equals(null, "B"), packedComparer.Equals(null, "B"));
		Assert.AreEqual(stringComparer.Equals("A", null), packedComparer.Equals("A", null));
		Assert.AreEqual(stringComparer.Equals("a", "b"), packedComparer.Equals("a", "b"));
		Assert.AreEqual(stringComparer.Equals("a", "a"), packedComparer.Equals("a", "a"));
		Assert.AreEqual(stringComparer.Equals("a", "A"), packedComparer.Equals("a", "A"));
	}

	[Test]
	public void TestEquals_WrongTypeThrowsCastException_1() {
		IEqualityComparer<string> stringComparer = StringComparer.InvariantCultureIgnoreCase;
		var packedComparer = stringComparer.AsPacked();
		Assert.That(() => packedComparer.Equals(1, 2), Throws.InstanceOf<InvalidCastException>());
	}

	[Test]
	public void TestEquals_WrongTypeThrowsCastException_2() {
		IEqualityComparer<string> stringComparer = StringComparer.InvariantCultureIgnoreCase;
		var packedComparer = stringComparer.AsPacked();
		Assert.That(() => packedComparer.Equals("a", 2), Throws.InstanceOf<InvalidCastException>());
	}

	[Test]
	public void TestGetHashCode_Consistency() {
		IEqualityComparer<string> stringComparer = StringComparer.InvariantCultureIgnoreCase;
		var packedComparer = stringComparer.AsPacked();
		Assert.AreEqual(stringComparer.GetHashCode("a"), packedComparer.GetHashCode("a"));
		Assert.AreEqual(stringComparer.GetHashCode("B"), packedComparer.GetHashCode("B"));
		Assert.AreEqual(stringComparer.GetHashCode("c"), packedComparer.GetHashCode("c"));
	}

	[Test]
	public void TestGetHashCode_WrongTypeThrowsCastException() {
		IEqualityComparer<string> stringComparer = StringComparer.InvariantCultureIgnoreCase;
		var packedComparer = stringComparer.AsPacked();
		Assert.That(() => packedComparer.GetHashCode(1), Throws.InstanceOf<InvalidCastException>());
	}

}
