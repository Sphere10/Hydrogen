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
public class WOTSTests {

	[Test]
	public void Signature_Consistency(
		[Values(CHF.SHA2_256, CHF.Blake2b_160)]
		CHF algorithm,
		[Range(1, 8)] int w,
		[Values] bool optimized
	) {
		var wots = new WOTS(w, optimized);
		var key = wots.GenerateKeys();
		var sig = wots.Sign(key.PrivateKey, System.Text.Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
		var sig2 = wots.Sign(key.PrivateKey, System.Text.Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog"));
		ClassicAssert.IsTrue(sig.AsFlatSpan().Slice(..^wots.Config.DigestSize).SequenceEqual(sig2.AsFlatSpan().Slice(..^wots.Config.DigestSize)));
	}

	[Test]
	public void Digest_All_0(
		[Values(CHF.SHA2_256, CHF.Blake2b_160)]
		CHF algorithm,
		[Range(1, 8)] int w,
		[Values] bool optimized
	) {
		var wots = new WOTS(w, optimized);
		var digest = Tools.Array.Gen<byte>(wots.Config.DigestSize, 0);
		var key = wots.GenerateKeys();
		var sig = wots.SignDigest(key.PrivateKey, digest);
		// signature should be 2^w-1'th hash into the hash chain (private key is 1st) (exclusing digest)
		ClassicAssert.AreEqual(
			sig.ToFlatArray().Take(wots.Config.SignatureDigits * wots.Config.DigestSize),
			key
				.PrivateKey
				.GetRows()
				.Take(wots.Config.SignatureDigits)
				.Select(x => Hashers.Iterate(wots.Config.HashFunction, x, wots.Config.ChainLength))
				.SelectMany(x => x)
				.ToArray()
		);
	}

	[Test]
	public void Digest_All_1(
		[Values(CHF.SHA2_256, CHF.Blake2b_160)]
		CHF algorithm,
		[Range(1, 8)] int w,
		[Values] bool optimized) {
		var wots = new WOTS(w, optimized);
		var digest = Tools.Array.Gen<byte>(wots.Config.DigestSize, 255);
		var key = wots.GenerateKeys();
		var sig = wots.SignDigest(key.PrivateKey, digest);
		// signature should be the private key (minus checksum)
		var nonFracitionalSignatureDigits = wots.Config.SignatureDigits - (wots.Config.SignatureDigits % wots.Config.W);
		ClassicAssert.AreEqual(
			sig.ToFlatArray().Take(nonFracitionalSignatureDigits * wots.Config.DigestSize),
			key.PrivateKey.ToFlatArray().Take(nonFracitionalSignatureDigits * wots.Config.DigestSize)
		);
	}

	[Test]
	public void SigVerify_All_0(
		[Values(CHF.SHA2_256, CHF.Blake2b_160)]
		CHF algorithm,
		[Range(1, 8)] int w,
		[Values] bool optimized) {
		var wots = new WOTS(w, optimized);
		var digest = Tools.Array.Gen<byte>(wots.Config.DigestSize, 0);
		var key = wots.GenerateKeys();
		ClassicAssert.IsTrue(wots.VerifyDigest(wots.SignDigest(key.PrivateKey, digest), key.PublicKey, digest));
	}

	[Test]
	public void SigVerify_All_1(
		[Values(CHF.SHA2_256, CHF.Blake2b_160)]
		CHF algorithm,
		[Range(1, 8)] int w,
		[Values] bool optimized) {
		var wots = new WOTS(w, optimized);
		var digest = Tools.Array.Gen<byte>(wots.Config.DigestSize, 1);
		var key = wots.GenerateKeys();
		ClassicAssert.IsTrue(wots.VerifyDigest(wots.SignDigest(key.PrivateKey, digest), key.PublicKey, digest));
	}

	[Test]
	public void SigVerify_1(
		[Values(CHF.SHA2_256, CHF.Blake2b_160)]
		CHF algorithm,
		[Range(1, 8)] int w,
		[Values] bool optimized) {
		var wots = new WOTS(w, optimized);
		var key = wots.GenerateKeys();
		var message = System.Text.Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
		var sig = wots.Sign(key.PrivateKey, message);
		ClassicAssert.IsTrue(wots.Verify(sig, key.PublicKey, message));
	}

	[Test]
	public void SigVerify_w16() {
		var wots = new WOTS(16);
		var key = wots.GenerateKeys();
		var message = System.Text.Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
		var sig = wots.Sign(key.PrivateKey, message);
		ClassicAssert.IsTrue(wots.Verify(sig, key.PublicKey, message));
	}

	[Test]
	public void SigVerify_Fail(
		[Values(CHF.SHA2_256, CHF.Blake2b_160)]
		CHF algorithm,
		[Range(1, 8)] int w,
		[Values] bool optimized) {
		var wots = new WOTS(w, optimized);
		var key = wots.GenerateKeys();
		var message = System.Text.Encoding.ASCII.GetBytes("The quick brown fox jumps over the lazy dog");
		var sig = wots.Sign(key.PrivateKey, message);
		unchecked {
			sig[0, 0] = (byte)(sig[0, 0] + 1);
			ClassicAssert.IsFalse(wots.Verify(sig, key.PublicKey, message));
		}
	}

	[Test]
	public void SigVerify_Integration_1(
		[Values(CHF.SHA2_256, CHF.Blake2b_160)]
		CHF algorithm,
		[Range(1, 8)] int w,
		[Values] bool optimized) {
		var wots = new WOTS(w, optimized);
		const int TestRounds = 10;
		var RNG = new Random(3117);
		var key = wots.GenerateKeys(RNG.NextBytes(100));
		for (var i = 0; i < TestRounds; i++) {
			var message = RNG.NextBytes(RNG.Next(0, 100));
			var sig = wots.Sign(key.PrivateKey, message);
			ClassicAssert.IsTrue(wots.Verify(sig, key.PublicKey, message));

			// flip a random byte
			var row = RNG.Next(0, sig.GetLength(0));
			var col = RNG.Next(0, sig.GetLength(1));
			unchecked {
				sig[row, col] += 1;
			}
			ClassicAssert.IsFalse(wots.Verify(sig, key.PublicKey, message));
		}
	}

}
