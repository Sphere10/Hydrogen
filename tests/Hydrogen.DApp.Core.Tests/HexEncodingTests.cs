// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;
using Hydrogen;
using NUnit.Framework.Legacy;

namespace VelocityNET.Processing.Tests.Core;

public class HexEncodingTests {

	[Test]
	public void Simple() {
		ClassicAssert.AreEqual(new[] { 1, 2, 3 }, HexEncoding.Decode("0x010203"));
		ClassicAssert.AreEqual(new[] { 1, 2, 3 }, HexEncoding.Decode("010203"));
		Assert.Throws<FormatException>(() => HexEncoding.ByteLength("0x1"));
		Assert.Throws<FormatException>(() => HexEncoding.ByteLength("0x012"));
	}

	[Test]
	public void IsValid() {
		ClassicAssert.IsTrue(HexEncoding.IsValid("0x0"));
		ClassicAssert.IsFalse(HexEncoding.IsValid("0x"));
		ClassicAssert.IsTrue(HexEncoding.IsValid("00"));
		ClassicAssert.IsFalse(HexEncoding.IsValid("0")); // should be double-digits
	}

	[Test]
	public void ByteLength() {
		ClassicAssert.AreEqual(0, HexEncoding.ByteLength("0x0"));
		ClassicAssert.AreEqual(1, HexEncoding.ByteLength("0x00"));
		Assert.Throws<FormatException>(() => HexEncoding.ByteLength("0x000"));
	}

}
