// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[Parallelizable]
public class ReferenceHashSetTests {
	
	[Test]
	public void Test_Add() {
		var referenceHashSet = new ReferenceHashSet<string>();
		var key1 = "one";
		var key2 = new string(new [] {'o', 'n', 'e' });
		referenceHashSet.Add(key1);
		referenceHashSet.Add(key2);
		Assert.That(referenceHashSet.Count, Is.EqualTo(2));
		Assert.That(referenceHashSet.Contains("one"), Is.True);
		ClassicAssert.AreEqual(referenceHashSet.ToArray(), new [] { key1, key2 });
	}

	[Test]
	public void Test_Remove() {
		var referenceHashSet = new ReferenceHashSet<string>();
		var key1 = "one";
		var key2 = new string(new [] {'o', 'n', 'e' });
		referenceHashSet.Add(key1);
		referenceHashSet.Add(key2);
		referenceHashSet.Remove("one");  // remove key1 value since compiler re-uses string literals
		ClassicAssert.AreEqual(referenceHashSet.ToArray(), new [] { key2 });

	}
}
