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

namespace VelocityNET.Processing.Tests.Core;

public class HexEncodingTests {

	[Test]
	public void Simple() {
		Assert.AreEqual(new[] { 1, 2, 3 }, HexEncoding.Decode("0x010203"));
		Assert.AreEqual(new[] { 1, 2, 3 }, HexEncoding.Decode("010203"));
		Assert.Throws<FormatException>(() => HexEncoding.ByteLength("0x1"));
		Assert.Throws<FormatException>(() => HexEncoding.ByteLength("0x012"));
	}

	[Test]
	public void IsValid() {
		Assert.IsTrue(HexEncoding.IsValid("0x0"));
		Assert.IsFalse(HexEncoding.IsValid("0x"));
		Assert.IsTrue(HexEncoding.IsValid("00"));
		Assert.IsFalse(HexEncoding.IsValid("0")); // should be double-digits
	}

	[Test]
	public void ByteLength() {
		Assert.AreEqual(0, HexEncoding.ByteLength("0x0"));
		Assert.AreEqual(1, HexEncoding.ByteLength("0x00"));
		Assert.Throws<FormatException>(() => HexEncoding.ByteLength("0x000"));
	}

}
