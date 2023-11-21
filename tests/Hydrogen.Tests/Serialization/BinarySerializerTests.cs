// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable]
public class BinarySerializerTests {


	[Test]
	public void Simple_1() {
		var serializer = new BinarySerializer();
		Assert.That(serializer.DeserializeBytesLE(serializer.SerializeBytesLE(123)), Is.EqualTo(123));
	}

	[Test]
	public void Simple_2() {
		var serializer = new BinarySerializer();
		var obj = new List<int> { 1, 2, 3, 4, 5 };
		Assert.That(serializer.DeserializeBytesLE(serializer.SerializeBytesLE(obj)), Is.EqualTo(obj));
	}

	[Test]
	public void Complex() {
		var serializer = new BinarySerializer();
		var obj = new List<string> { "hello", "world", "hello" };
		var deserialized = serializer.DeserializeBytesLE(serializer.SerializeBytesLE(obj)) as List<string>;
		Assert.That(deserialized, Is.EqualTo(obj));
		Assert.That(deserialized[0], Is.SameAs(deserialized[2]));
	}

}
