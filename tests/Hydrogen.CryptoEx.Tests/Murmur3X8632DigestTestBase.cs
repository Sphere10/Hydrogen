// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities.Encoders;
using Hydrogen.CryptoEx.HF;
using NUnit.Framework.Legacy;

namespace Hydrogen.CryptoEx.Tests;

/**
      * Test vectors for Murmur3_x86_32Digest
      *  
      */
[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class Murmur3_x86_32DigestTest : DigestTest {
	private static readonly string[] Messages = {
		"",
		"a",
		"abc",
		"message digest",
		"abcdefghijklmnopqrstuvwxyz",
		"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789",
		"12345678901234567890123456789012345678901234567890123456789012345678901234567890",
		"abcdbcdecdefdefgefghfghighijhijk"
	};

	private static readonly string[] Digests = {
		"00000000",
		"3C2569B2",
		"B3DD93FA",
		"638F4169",
		"A34E036D",
		"A27AF39B",
		"F8E05287",
		"465C48E8"
	};

	// General purpose byte array for testing		
	private const string DataBytes =
		"0x4f550200ca022000bb718b4b00d6f74478c332f5fb310507e55a9ef9b38551f63858e3f7c86dbd00200006f69afae8a6b0735b6acfcc58b7865fc8418897c530211f19140c9f95f24532102700000000000003000300a297fd17506f6c796d696e65722e506f6c796d696e65722e506f6c796d6939303030303030302184d63666eb166619e925cef2a306549bbc4d6f4da3bdf28b4393d5c1856f0ee3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855000000006d68295b00000000";

	private void PerformUInt32VectorTest(string testTitle, byte[] inputBytes,
	                                     uint resultsAsUInt32) {
		DoPerformTest(testTitle, inputBytes, resultsAsUInt32);
	}

	private void DoPerformTest(string testTitle, byte[] inputBytes, uint resultsAsUInt32) {
		uint resUInt32 = CreateUInt32OutputFromDigest(inputBytes);

		ClassicAssert.AreEqual(resultsAsUInt32, resUInt32, testTitle);
	}

	private uint CreateUInt32OutputFromDigest(byte[] digestBytes) {
		Murmur3_x86_32Digest digest = new Murmur3_x86_32Digest();
		digest.BlockUpdate(digestBytes, 0, digestBytes.Length);
		return digest.DoFinal();
	}

	protected override IDigest CloneDigest(IDigest digest) {
		return new Murmur3_x86_32Digest((Murmur3_x86_32Digest)digest);
	}

	public Murmur3_x86_32DigestTest()
		: base(new Murmur3_x86_32Digest(), Messages, Digests) {
	}

	[Test]
	public void TestMurmur3_x86_32() {
		Murmur3_x86_32DigestTest digestTest = new Murmur3_x86_32DigestTest();
		digestTest.PerformTest();
	}

	[Test]
	public void TestMurmur3_x86_32_UInt22_Output() {
		PerformUInt32VectorTest("17 Bytes test",
			Hex.Decode(DataBytes.Substring(2, 17 * 2)),
			3935905087);

		PerformUInt32VectorTest("31 Bytes test",
			Hex.Decode(DataBytes.Substring(2, 31 * 2)),
			357806018);

		PerformUInt32VectorTest("64 Bytes test",
			Hex.Decode(DataBytes.Substring(2, 64 * 2)),
			1305844766);

		PerformUInt32VectorTest("127 Bytes test",
			Hex.Decode(DataBytes.Substring(2, 127 * 2)),
			586511266);

		PerformUInt32VectorTest("200 Bytes test",
			Hex.Decode(DataBytes.Substring(2, 200 * 2)),
			1143690750);
	}
}
