// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ApproxEqual {

	[Test]
	public void Exact() {
		var date = DateTime.Now;
		var test = date;
		ClassicAssert.IsTrue(date.ApproxEqual(test));
	}

	[Test]
	public void LessThanButWithinTolerance() {
		var date = DateTime.Now;
		var test = date.Subtract(TimeSpan.FromMilliseconds(100));
		ClassicAssert.IsTrue(date.ApproxEqual(test, TimeSpan.FromMilliseconds(250)));
	}

	[Test]
	public void LessThanButAtMaxTolerance() {
		var date = DateTime.Now;
		var test = date.Subtract(TimeSpan.FromMilliseconds(250));
		ClassicAssert.IsTrue(date.ApproxEqual(test, TimeSpan.FromMilliseconds(250)));
	}

	[Test]
	public void LessThanAndBeyondTolerance() {
		var date = DateTime.Now;
		var test = date.Subtract(TimeSpan.FromMilliseconds(251));
		ClassicAssert.IsFalse(date.ApproxEqual(test, TimeSpan.FromMilliseconds(250)));
	}
	[Test]
	public void GreaterThanButWithinTolerance() {
		var date = DateTime.Now;
		var test = date.Add(TimeSpan.FromMilliseconds(100));
		ClassicAssert.IsTrue(date.ApproxEqual(test, TimeSpan.FromMilliseconds(250)));
	}

	[Test]
	public void GreaterThanButAtMaxTolerance() {
		var date = DateTime.Now;
		var test = date.Add(TimeSpan.FromMilliseconds(250));
		ClassicAssert.IsTrue(date.ApproxEqual(test, TimeSpan.FromMilliseconds(250)));
	}

	[Test]
	public void GreaterThanAndBeyondTolerance() {
		var date = DateTime.Now;
		var test = date.Add(TimeSpan.FromMilliseconds(251));
		ClassicAssert.IsFalse(date.ApproxEqual(test, TimeSpan.FromMilliseconds(250)));
	}
}
