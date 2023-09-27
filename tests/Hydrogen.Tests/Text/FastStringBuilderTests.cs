// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class FastStringBuilderTests {

	[Test]
	public void Empty() {
		var sb = new FastStringBuilder();
		Assert.That(sb.ToString(), Is.EqualTo(string.Empty));
	}

	[Test]
	public void Add() {
		var sb = new FastStringBuilder();
		sb.Append("alpha");
		Assert.That(sb.ToString(), Is.EqualTo("alpha"));
	}

	[Test]
	public void Add_2() {
		var sb = new FastStringBuilder();
		sb.Append("alpha");
		sb.Append("beta");
		Assert.That(sb.ToString(), Is.EqualTo("alphabeta"));
	}

	[Test]
	public void Insert() {
		var sb = new FastStringBuilder();
		sb.Insert(0, "alpha");
		Assert.That(sb.ToString(), Is.EqualTo("alpha"));
	}

	[Test]
	public void Insert_2() {
		var sb = new FastStringBuilder();
		sb.Insert(0, "alpha");
		sb.Insert(1, "beta");
		Assert.That(sb.ToString(), Is.EqualTo("alphabeta"));
	}

	[Test]
	public void Insert_3() {
		var sb = new FastStringBuilder();
		sb.Insert(0, "alpha");
		sb.Insert(1, "beta");
		Assert.That(sb.ToString(), Is.EqualTo("alphabeta"));
	}

	[Test]
	public void Insert_4() {
		var sb = new FastStringBuilder();
		sb.Insert(0, "alpha");
		sb.Insert(1, "gamma");
		sb.Insert(1, "beta");
		Assert.That(sb.ToString(), Is.EqualTo("alphabetagamma"));
	}


	[Test]
	public void Mix() {
		var sb = new FastStringBuilder();
		sb.Append("gamma");
		sb.Prepend("beta");
		sb.Prepend("alpha");
		sb.Append("delta");
		Assert.That(sb.ToString(), Is.EqualTo("alphabetagammadelta"));
	}

	[Test]
	public void ChopFromEnd() {
		var sb = new FastStringBuilder();
		sb.Append("gamma");
		sb.Prepend("beta");
		sb.Prepend("alpha");
		sb.Append("delta");
		sb.ChopFromEnd("agammadelta".Length);
		Assert.That(sb.ToString(), Is.EqualTo("alphabet"));
		Assert.That(sb.SubStringCount, Is.EqualTo(2));
	}

	[Test]
	public void ChopFromEnd_Empty() {
		var sb = new FastStringBuilder();
		Assert.That(sb.ChopFromEnd(0), Is.EqualTo(string.Empty));
	}

	[Test]
	public void ChopFromEnd_Empty_1() {
		var sb = new FastStringBuilder();
		Assert.That(sb.ChopFromEnd(1), Is.EqualTo(string.Empty));
	}

	[Test]
	public void ChopFromEnd_Overflow() {
		var sb = new FastStringBuilder();
		sb.Append("a");
		sb.Append("b");
		sb.Append("c");
		Assert.That(sb.ChopFromEnd(4), Is.EqualTo("abc"));
	}
}
