// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using NUnit.Framework.Internal;
using Hydrogen;
using Hydrogen.CryptoEx.PascalCoin;
using NUnit.Framework.Legacy;

namespace Hydrogen.CryptoEx.Tests;

public class PascalBase58EncodingTests {
	[SetUp]
	public void Setup() {
	}

	[Test]
	public void Compatibility_Test() {
		// Test values derived from Pascal implementation
		const string keyBase58 = "3GhhbonKEE3SzPvRqPEeXbLc1v1LCcYvqY7wuxW8esJLCb2FCj4jofWmkmMTtqP1atrmioAMqEwigHA2CEwidxgB1i1gGhL39unBkk";
		const string keyHex = "ca0220003ce142a2bfa1e2d4c6246bf7dfa8106d975f498133fb7084e83062f8941d09ea2000ab8fa84d3287d8efe04b865e50f9c7ff2a9a85187191c2528045e697ec9d8b5143db2899";
		ClassicAssert.AreEqual(keyBase58, PascalBase58Encoding.Encode(keyHex.ToHexByteArray()));
		ClassicAssert.AreEqual(keyHex, PascalBase58Encoding.Decode(keyBase58).ToHexString(true));
	}

}
