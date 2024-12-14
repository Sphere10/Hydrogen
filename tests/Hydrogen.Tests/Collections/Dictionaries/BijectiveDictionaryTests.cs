// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[Parallelizable]
public class BijectiveDictionaryTests {
	
	[Test]
	public void Test() {
		var dictionary = new Dictionary<string, int> {
			 ["one"] = 1,
			 ["two"] = 2,
			 ["three"] = 3,
			 ["four"] = 4,
			 ["five"] = 5,
		}.AsBijection();

		Assert.That(dictionary.Bijection[1], Is.EqualTo("one"));
		Assert.That(dictionary.Bijection[2], Is.EqualTo("two"));
		Assert.That(dictionary.Bijection[3], Is.EqualTo("three"));
		Assert.That(dictionary.Bijection[4], Is.EqualTo("four"));
		Assert.That(dictionary.Bijection[5], Is.EqualTo("five"));
	}

	[Test]
	public void TestAdd() {
		var dictionary = new Dictionary<string, int> {
			["one"] = 1,
			["two"] = 2,
			["three"] = 3,
			["four"] = 4,
			["five"] = 5,
		}.AsBijection();

		dictionary.Bijection.Add(6, "six");

		Assert.That(dictionary["six"], Is.EqualTo(6));

	}

	[Test]
	public void TestRemove() {
		var dictionary = new Dictionary<string, int> {
			["one"] = 1,
			["two"] = 2,
			["three"] = 3,
			["four"] = 4,
			["five"] = 5,
		}.AsBijection();

		dictionary.Bijection.Remove(5);

		Assert.That(dictionary["one"], Is.EqualTo(1));
		Assert.That(dictionary["two"], Is.EqualTo(2));
		Assert.That(dictionary["three"], Is.EqualTo(3));
		Assert.That(dictionary["four"], Is.EqualTo(4));

	}

	[Test]
	public void TestBijectionReferences() {
		var dictionary = new Dictionary<string, int> {
			["one"] = 1,
			["two"] = 2,
			["three"] = 3,
			["four"] = 4,
			["five"] = 5,
		}.AsBijection();
		Assert.That(dictionary, Is.SameAs(dictionary.Bijection.Bijection));
	}

	[Test]
	public void TestUpdateViaBijection() {
		var dictionary = new BijectiveDictionary<string, int>();
		dictionary["one"] = 1;
		dictionary.Bijection[1] = "one updated";

		Assert.That(dictionary.Count, Is.EqualTo(1));
		ClassicAssert.AreEqual(dictionary.Keys, new [] { "one updated" });
		ClassicAssert.AreEqual(dictionary.Values, new [] { 1 });
	}

	[Test]
	public void TestUpdate() {
		var dictionary = new BijectiveDictionary<string, int>();
		dictionary["one"] = 1;
		dictionary["two"] = 2;
		dictionary["one"] = 11;

		Assert.That(dictionary.Count, Is.EqualTo(2));
		Assert.That(dictionary["one"], Is.EqualTo(11));
		Assert.That(dictionary.Bijection[11], Is.EqualTo("one"));
		Assert.That(dictionary.Bijection.ContainsKey(1), Is.False);
	}
}
