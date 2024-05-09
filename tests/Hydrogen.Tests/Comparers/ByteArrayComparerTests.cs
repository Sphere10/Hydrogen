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
public class ByteArrayComparerTests {

	[Test]
	public void TestNull() {
		ClassicAssert.AreEqual(0, ByteArrayComparer.Instance.Compare(null, null));
	}

	[Test]
	public void TestEmpty() {
		ClassicAssert.AreEqual(0, ByteArrayComparer.Instance.Compare(new byte[0], new byte[0]));
	}

	[Test]
	public void TestSame() {
		ClassicAssert.AreEqual(0, ByteArrayComparer.Instance.Compare(new byte[] { 1, 2, 3 }, new byte[] { 1, 2, 3 }));
	}

	[Test]
	public void TestSmaller() {
		ClassicAssert.AreEqual(-1, ByteArrayComparer.Instance.Compare(new byte[] { 1, 2, 3 }, new byte[] { 3, 2, 1 }));
	}

	[Test]
	public void TestGreater() {
		ClassicAssert.AreEqual(1, ByteArrayComparer.Instance.Compare(new byte[] { 3, 2, 1 }, new byte[] { 1, 2, 3 }));
	}
}
