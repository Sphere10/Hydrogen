// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
public class WAMSTests {
	private static readonly byte[] Secret = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };

	private AMS BuildWAMS(AMSOTS ots, CHF chf, int w, int h) {
		switch (ots) {
			case AMSOTS.WOTS:
				return new WAMS(h, w, chf);
			case AMSOTS.WOTS_Sharp:
				return new WAMSSharp(h, w, chf);
			default:
				throw new NotSupportedException(ots.ToString());
		}
	}

	[Test]
	public void Signature_Consistency(
		[Values(AMSOTS.WOTS, AMSOTS.WOTS_Sharp)]
		AMSOTS ots,
		[Values(CHF.SHA2_256, CHF.Blake2b_128)]
		CHF algorithm,
		[Values(1, 3, 8)] int w,
		[Values(0, 4, 8)] byte h,
		[Values(0, 1, 100, 23482, 21, 77, 28322)]
		int batch) {
		var wams = BuildWAMS(ots, algorithm, w, h);
		var key = wams.GeneratePrivateKey(Secret);
		var sig = wams.Sign(key, Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"), (ulong)batch, 0);
		var sig2 = wams.Sign(key, Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"), (ulong)batch, 0);
		if (ots == AMSOTS.WOTS)
			ClassicAssert.AreEqual(sig, sig2);
		else
			ClassicAssert.AreNotEqual(sig, sig2);
	}

	[Test]
	public void SignVerify_Basic(
		[Values(AMSOTS.WOTS, AMSOTS.WOTS_Sharp)]
		AMSOTS ots,
		[Values(CHF.SHA2_256, CHF.Blake2b_128)]
		CHF algorithm,
		[Values(1, 3, 8)] int w,
		[Values(0, 4, 8)] byte h,
		[Values(0, 1, 100, 23482, 21, 77, 28322)]
		int batch) {
		var wams = BuildWAMS(ots, algorithm, w, h);
		var privateKey = wams.GeneratePrivateKey(Secret);
		var publicKey = wams.DerivePublicKeyForBatch(privateKey, (ulong)batch, true);

		var message = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
		var sig = wams.Sign(privateKey, message, publicKey.BatchNo, 0);
		ClassicAssert.IsTrue(wams.Verify(sig, message, publicKey));
	}

	[Test]
	public void SignVerify_AllOTSKeys(
		[Values(AMSOTS.WOTS, AMSOTS.WOTS_Sharp)]
		AMSOTS ots,
		[Values(CHF.SHA2_256, CHF.Blake2b_128)]
		CHF algorithm,
		[Values(1, 3, 8)] int w,
		[Values(0, 4, 8)] byte h,
		[Values(0, 1, 100, 23482, 21, 77, 28322)]
		int batch) {

		var wams = BuildWAMS(ots, algorithm, w, h);
		var privateKey = wams.GeneratePrivateKey(Secret);
		var publicKey = wams.DerivePublicKeyForBatch(privateKey, (ulong)batch, true);
		var messageDigest = Hashers.Hash(algorithm, Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));

		for (var i = 0; i < 1 << privateKey.Height; i++) {
			var sig = wams.SignDigest(privateKey, messageDigest, (ulong)batch, i);
			ClassicAssert.IsTrue(wams.VerifyDigest(sig, messageDigest, publicKey.RawBytes));
		}
	}

	[Test]
	public void SigVerify_AllHeights(
		[Values(AMSOTS.WOTS, AMSOTS.WOTS_Sharp)]
		AMSOTS ots,
		[Values(CHF.SHA2_256, CHF.Blake2b_128)]
		CHF algorithm,
		[Values(1, 3, 8)] int w,
		[Values(0, 4, 8)] byte h) {
		var wams = BuildWAMS(ots, algorithm, w, h);
		var privateKey = wams.GeneratePrivateKey(Secret);
		var publicKey = wams.DerivePublicKeyForBatch(privateKey, 0, true);

		var message = Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
		var sig = wams.Sign(privateKey, message, publicKey.BatchNo, 0);
		ClassicAssert.IsTrue(wams.Verify(sig, message, publicKey));
	}

	[Test]
	public void KeyTest_Basic_True(
		[Values(AMSOTS.WOTS, AMSOTS.WOTS_Sharp)]
		AMSOTS ots,
		[Values(CHF.SHA2_256, CHF.Blake2b_128)]
		CHF algorithm,
		[Values(1, 3, 8)] int w,
		[Values(0, 4, 8)] byte h,
		[Values(0, 1, 100, 23482, 21, 77, 28322)]
		int batch) {
		var wams = BuildWAMS(ots, algorithm, w, h);
		var privateKey = wams.GeneratePrivateKey(Secret);
		var publicKey = wams.DerivePublicKeyForBatch(privateKey, (ulong)batch);
		var bytes = publicKey.RawBytes.ToArray();
		ClassicAssert.IsTrue(wams.IsPublicKey(privateKey, bytes));
	}

	[Test]
	public void KeyTest_Basic_False(
		[Values(AMSOTS.WOTS, AMSOTS.WOTS_Sharp)]
		AMSOTS ots,
		[Values(CHF.SHA2_256, CHF.Blake2b_128)]
		CHF algorithm,
		[Values(1, 3, 8)] int w,
		[Values(0, 4, 8)] byte h,
		[Values(0, 1, 100, 23482, 21, 77, 28322)]
		int batch) {
		var wams = BuildWAMS(ots, algorithm, w, h);
		var privateKey = wams.GeneratePrivateKey(Secret);
		var publicKey = wams.DerivePublicKeyForBatch(privateKey, (ulong)batch);
		var bytes = publicKey.RawBytes.ToArray();
		unchecked {
			bytes[0] += 1;
		}
		ClassicAssert.IsFalse(wams.IsPublicKey(privateKey, bytes));
	}

}
