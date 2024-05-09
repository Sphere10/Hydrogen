// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Hydrogen.CryptoEx.Bitcoin;
using NUnit.Framework.Legacy;

namespace Hydrogen.CryptoEx.Tests;

//https://github.com/bitcoin/bitcoin/blob/master/src/test/base58_tests.cpp
[TestFixture]
public class BitcoinBase58EncodingTests {

	private struct TestItem<TInput, TExpected> {
		public TInput HexString;
		public TExpected Base58String;
	}


	//https://github.com/bitcoin/bitcoin/blob/master/src/test/data/base58_encode_decode.json
	private static readonly TestItem<string, string>[] DATA = {
		new() { HexString = "", Base58String = "" },
		new() { HexString = "61", Base58String = "2g" },
		new() { HexString = "626262", Base58String = "a3gV" },
		new() { HexString = "636363", Base58String = "aPEr" },
		new() { HexString = "73696d706c792061206c6f6e6720737472696e67", Base58String = "2cFupjhnEsSn59qHXstmK2ffpLv2" },
		new() { HexString = "00eb15231dfceb60925886b67d065299925915aeb172c06647", Base58String = "1NS17iag9jJgTHD1VXjvLCEnZuQ3rJDE9L" },
		new() { HexString = "516b6fcd0f", Base58String = "ABnLTmg" },
		new() { HexString = "bf4f89001e670274dd", Base58String = "3SEo3LWLoPntC" },
		new() { HexString = "572e4794", Base58String = "3EFU7m" },
		new() { HexString = "ecac89cad93923c02321", Base58String = "EJDM8drfXA6uyA" },
		new() { HexString = "10c8511e", Base58String = "Rt5zm" },
		new() { HexString = "00000000000000000000", Base58String = "1111111111" },
		new() { HexString = "000111d38e5fc9071ffcd20b4a763cc9ae4f252bb4e48fd66a835e252ada93ff480d6dd43dc62a641155a5", Base58String = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz" },
		new() {
			HexString =
				"000102030405060708090a0b0c0d0e0f101112131415161718191a1b1c1d1e1f202122232425262728292a2b2c2d2e2f303132333435363738393a3b3c3d3e3f404142434445464748494a4b4c4d4e4f505152535455565758595a5b5c5d5e5f606162636465666768696a6b6c6d6e6f707172737475767778797a7b7c7d7e7f808182838485868788898a8b8c8d8e8f909192939495969798999a9b9c9d9e9fa0a1a2a3a4a5a6a7a8a9aaabacadaeafb0b1b2b3b4b5b6b7b8b9babbbcbdbebfc0c1c2c3c4c5c6c7c8c9cacbcccdcecfd0d1d2d3d4d5d6d7d8d9dadbdcdddedfe0e1e2e3e4e5e6e7e8e9eaebecedeeeff0f1f2f3f4f5f6f7f8f9fafbfcfdfeff",
			Base58String =
				"1cWB5HCBdLjAuqGGReWE3R3CguuwSjw6RHn39s2yuDRTS5NsBgNiFpWgAnEx6VQi8csexkgYw3mdYrMHr8x9i7aEwP8kZ7vccXWqKDvGv3u1GxFKPuAkn8JCPPGDMf3vMMnbzm6Nh9zh1gcNsMvH3ZNLmP5fSG6DGbbi2tuwMWPthr4boWwCxf7ewSgNQeacyozhKDDQQ1qL5fQFUW52QKUZDZ5fw3KXNQJMcNTcaB723LchjeKun7MuGW5qyCBZYzA1KjofN1gYBV3NqyhQJ3Ns746GNuf9N2pQPmHz4xpnSrrfCvy6TVVz5d4PdrjeshsWQwpZsZGzvbdAdN8MKV5QsBDY"
		}
	};

	private static void TestEncode<TResult>(Func<byte[], TResult> encoder, IEnumerable<TestItem<string, TResult>> testCases) {
		foreach (var testCase in testCases) {
			var input = string.Equals(testCase.HexString, string.Empty, StringComparison.OrdinalIgnoreCase) ? Array.Empty<byte>() : HexEncoding.Decode(testCase.HexString).ToArray();
			var result = encoder(input);
			ClassicAssert.AreEqual(testCase.Base58String, result);
		}
	}

	private static void TestDecode<TResult>(Func<TResult, byte[]> decoder, IEnumerable<TestItem<string, TResult>> testCases) {
		foreach (var testCase in testCases) {
			var result = decoder(testCase.Base58String);
			ClassicAssert.AreEqual(testCase.HexString, HexEncoding.Encode(result));
		}
	}

	[Test]
	public void TestShouldEncodeProperly() {
		TestEncode(x => BitcoinBase58Encoding.Encode(x), DATA);
	}

	[Test]
	public void TestShouldDecodeProperly() {
		TestDecode(x => BitcoinBase58Encoding.Decode(x), DATA);
	}

	[Test]
	public void TestShouldFailOnInvalidBase58() {
		ClassicAssert.IsFalse(BitcoinBase58Encoding.TryDecode("invalid", out _));
		ClassicAssert.IsFalse(BitcoinBase58Encoding.TryDecode("invalid\0", out _));
		ClassicAssert.IsFalse(BitcoinBase58Encoding.TryDecode("\0invalid", out _));

		ClassicAssert.IsFalse(BitcoinBase58Encoding.TryDecode("bad0IOl", out _));
		ClassicAssert.IsFalse(BitcoinBase58Encoding.TryDecode("goodbad0IOl", out _));
		ClassicAssert.IsFalse(BitcoinBase58Encoding.TryDecode("good\0bad0IOl", out _));

		// check that DecodeBase58 skips whitespace, but still fails with unexpected non-whitespace at the end.
		ClassicAssert.IsFalse(BitcoinBase58Encoding.TryDecode(" \t\n\v\f\r skip \r\f\v\n\t a", out _));
	}

	[Test]
	public void TestShouldPassOnValidBase58() {
		ClassicAssert.IsTrue(BitcoinBase58Encoding.TryDecode("good", out _));
		ClassicAssert.IsTrue(BitcoinBase58Encoding.TryDecode(" ", out _));

		var result = BitcoinBase58Encoding.Decode(" \t\n\v\f\r skip \r\f\v\n\t ");
		var expected = HexEncoding.Decode("971a55");
		ClassicAssert.AreEqual(expected, result);
	}
}
