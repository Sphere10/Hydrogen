// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using NUnit.Framework.Legacy;


namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ByteArrayEqualityComparerTests {

	[Test]
	public void TestNull() {
		ClassicAssert.AreEqual(true, ByteArrayEqualityComparer.Instance.Equals(null, null));
	}

	[Test]
	public void TestEmpty() {
		ClassicAssert.AreEqual(true, ByteArrayEqualityComparer.Instance.Equals(new byte[0], new byte[0]));
	}

	[Test]
	public void TestSame() {
		ClassicAssert.AreEqual(true, ByteArrayEqualityComparer.Instance.Equals(new byte[] { 1, 2 }, new byte[] { 1, 2 }));
	}

	[Test]
	public void TestDiff() {
		ClassicAssert.AreEqual(false, ByteArrayEqualityComparer.Instance.Equals(new byte[] { 1, 2 }, new byte[] { 2, 1 }));
	}

	[Test]
	public void TestDiffLonger_1() {
		ClassicAssert.AreEqual(false, ByteArrayEqualityComparer.Instance.Equals(new byte[] { 1, 2, 3 }, new byte[] { 2, 1 }));
	}

	[Test]
	public void TestDiffLonger_2() {
		ClassicAssert.AreEqual(false, ByteArrayEqualityComparer.Instance.Equals(new byte[] { 1, 2 }, new byte[] { 2, 1, 3 }));
	}

}
