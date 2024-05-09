// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
public class RecursiveDataTypeTests {

	[Test]
	public void TestSimple() {
		var rdt = new RecursiveDataType<string>();
		rdt.State = "A";
		rdt.AddSubStates("B", "C", "D");
		rdt.SubStates[1].AddSubStates("C1");

		Assert.That(rdt.State, Is.EqualTo("A"));
		Assert.That(rdt.SubStates.Count, Is.EqualTo(3));

		Assert.That(rdt.SubStates[0].State, Is.EqualTo("B"));
		Assert.That(rdt.SubStates[0].SubStates.Count, Is.EqualTo(0));
		
		Assert.That(rdt.SubStates[1].State, Is.EqualTo("C"));
		Assert.That(rdt.SubStates[1].SubStates.Count, Is.EqualTo(1));

		Assert.That(rdt.SubStates[1].SubStates[0].State, Is.EqualTo("C1"));
		Assert.That(rdt.SubStates[1].SubStates[0].SubStates.Count, Is.EqualTo(0));
		
		Assert.That(rdt.SubStates[2].State, Is.EqualTo("D"));
		Assert.That(rdt.SubStates[2].SubStates.Count, Is.EqualTo(0));
	}

	[Test]
	public void TestFlattenIterator() {
		var rdt = new RecursiveDataType<string>();
		rdt.State = "A";
		rdt.AddSubStates("B", "C", "D");
		rdt.SubStates[1].AddSubStates("C1");

		ClassicAssert.AreEqual(new[] { "A", "B", "C", "C1", "D" }, rdt.Flatten());
	}

	[Test]
	public void TestFlattenArray() {
		var rdt = new RecursiveDataType<string>();
		rdt.State = "A";
		rdt.AddSubStates("B", "C", "D");
		rdt.SubStates[1].AddSubStates("C1");
		rdt.Flatten(out var states, out var counts);
		ClassicAssert.AreEqual(new[] { "A", "B", "C", "C1", "D" }, states);
		ClassicAssert.AreEqual(new[] { 3, 0, 1, 0, 0 }, counts);
	}

	[Test]
	public void TestFromFlattenedArray() {

		var rdt =  RecursiveDataType<string>.FromFlattened(new[] { "A", "B", "C", "C1", "D" }, new[] { 3, 0, 1, 0, 0 });
		
		Assert.That(rdt.State, Is.EqualTo("A"));
		Assert.That(rdt.SubStates.Count, Is.EqualTo(3));

		Assert.That(rdt.SubStates[0].State, Is.EqualTo("B"));
		Assert.That(rdt.SubStates[0].SubStates.Count, Is.EqualTo(0));
		
		Assert.That(rdt.SubStates[1].State, Is.EqualTo("C"));
		Assert.That(rdt.SubStates[1].SubStates.Count, Is.EqualTo(1));

		Assert.That(rdt.SubStates[1].SubStates[0].State, Is.EqualTo("C1"));
		Assert.That(rdt.SubStates[1].SubStates[0].SubStates.Count, Is.EqualTo(0));
		
		Assert.That(rdt.SubStates[2].State, Is.EqualTo("D"));
		Assert.That(rdt.SubStates[2].SubStates.Count, Is.EqualTo(0));

	}
}
