// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using Hydrogen.NUnit;
using NUnit.Framework;

namespace Hydrogen.Tests;

[Parallelizable]
public class DictionaryListTests {


	[Test]
	public void WalkthroughTests() {

		var ivd = new DictionaryList<string, string>(StringComparer.InvariantCultureIgnoreCase, StringComparer.InvariantCulture);

		Assert.That(ivd.Count, Is.EqualTo(0));

		ivd["zero"] = "alpha";
		ivd["one"] = "beta";
		ivd["two"] = "gamma";
		ivd["three"] = "delta";

		Assert.That(ivd.Count, Is.EqualTo(4));
		Assert.That(ivd[0], Is.EqualTo("alpha"));
		Assert.That(ivd[1], Is.EqualTo("beta"));
		Assert.That(ivd[2], Is.EqualTo("gamma"));
		Assert.That(ivd[3], Is.EqualTo("delta"));

		Assert.That(ivd.ContainsKey("one"), Is.True);
		Assert.That(ivd.ContainsKey("OnE"), Is.True);

		Assert.That(ivd.Contains(new KeyValuePair<string, string>("one", "beta")), Is.True);
		Assert.That(ivd.Contains(new KeyValuePair<string, string>("OnE", "beta")), Is.True);
		Assert.That(ivd.Contains(new KeyValuePair<string, string>("one", "BeTa")), Is.False);
		Assert.That(ivd.Contains(new KeyValuePair<string, string>("OnE", "BeTa")), Is.False);

		ivd.Remove("one");
		Assert.That(ivd[0], Is.EqualTo("alpha"));
		Assert.That(ivd[1], Is.EqualTo("gamma"));
		Assert.That(ivd[2], Is.EqualTo("delta"));
	}

	[Test]
	public void Integration() {
		var keyGens = 0;
		var dict = new DictionaryList<string, string>();
			AssertEx.DictionaryIntegrationTest(
				dict,
				1000,
				(rng) => ($"{keyGens++}_{rng.NextString(0, 100)}", rng.NextString(rng.Next(100))),
				iterations: 250
			);
		}
}
