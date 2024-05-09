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
public class StringTests {


	[Test]
	public void TrimStart_1() {
		ClassicAssert.AreEqual("", "alpha".TrimStart("alpha"));
	}

	[Test]
	public void TrimStart_2() {
		ClassicAssert.AreEqual("1", "alpha1".TrimStart("alpha"));
	}

	[Test]
	public void TrimStart_3() {
		ClassicAssert.AreEqual("1alpha2", "1alpha2".TrimStart("alpha"));
	}


	[Test]
	public void TrimStart_4() {
		ClassicAssert.AreEqual("", "aLphA".TrimStart("alpha", false));
	}

	[Test]
	public void TrimStart_5() {
		ClassicAssert.AreEqual("1", "AlpHa1".TrimStart("alpha", false));
	}

	[Test]
	public void TrimStart_6() {
		ClassicAssert.AreEqual("1aLPha2", "1aLPha2".TrimStart("alpha", false));
	}


	[Test]
	public void TrimEnd_1() {
		ClassicAssert.AreEqual("", "alpha".TrimEnd("alpha"));
	}

	[Test]
	public void TrimEnd_2() {
		ClassicAssert.AreEqual("1", "1alpha".TrimEnd("alpha"));
	}

	[Test]
	public void TrimEnd_3() {
		ClassicAssert.AreEqual("1aLpha2", "1aLpha2".TrimEnd("alpha"));
	}

	[Test]
	public void TrimEnd_4() {
		ClassicAssert.AreEqual("", "alpHa".TrimEnd("alpha", false));
	}

	[Test]
	public void TrimEnd_5() {
		ClassicAssert.AreEqual("1", "1AlphA".TrimEnd("alpha", false));
	}

	[Test]
	public void TrimEnd_6() {
		ClassicAssert.AreEqual("1alpHa2", "1alpHa2".TrimEnd("alpha", false));
	}
}
