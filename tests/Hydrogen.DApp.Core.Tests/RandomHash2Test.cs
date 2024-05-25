// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using Hydrogen.CryptoEx;
using Hydrogen.DApp.Core.Maths;
using System;
using System.Text;
using NUnit.Framework.Legacy;

namespace Hydrogen.DApp.Core.Tests;

internal abstract class RandomHash2TestBase {
	protected struct TestItem<TInput, TExpected> {
		public TInput Input;
		public TExpected Expected;
	}


	#region TestData

	// General purpose byte array for testing 
	protected const string DATA_BYTES =
		"0x4f550200ca022000bb718b4b00d6f74478c332f5fb310507e55a9ef9b38551f63858e3f7c86dbd00200006f69afae8a6b0735b6acfcc58b7865fc8418897c530211f19140c9" +
		"f95f24532102700000000000003000300a297fd17506f6c796d696e65722e506f6c796d696e65722e506f6c796d6939303030303030302184d63666eb166619e925cef2a306549bbc4" +
		"d6f4da3bdf28b4393d5c1856f0ee3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855000000006d68295b00000000";

	// RandomHash Official Values
	protected static readonly TestItem<string, string>[] DATA_RANDOMHASH_STANDARD = {
		new TestItem<string, string>
			{ Input = "0x0", Expected = "0x76821dd68e384bdc2a69fea66a70191c1d7df5799bddc70f2dfaaeda2393899b" },
		new TestItem<string, string> {
			Input = "The quick brown fox jumps over the lazy dog",
			Expected = "0x65ec5370f913497abc57621e9b6703b51d541a320ac2422a51c16e48d0d1fc05"
		},
		new TestItem<string, string> {
			Input = "0x000102030405060708090a0b0c0d0e0f",
			Expected = "0xfabbfcd96c9ef4734bd82d59bfa4bc9c0ff389a53c7c247abb1c1c1794e6dea8"
		},
	};

	// Hash Test Data
	protected static TestItem<int, string>[] DATA_RANDOMHASH = {
		// NOTE: Input denotes the number of bytes to take from DATA_BYTES when executing test 
		new TestItem<int, string>
			{ Input = 17, Expected = "0x5f66d30d5872652d6f7c88ade147e5a2dfb1082850c48ce3c4391a354c27f6ef" },
		new TestItem<int, string>
			{ Input = 31, Expected = "0x36a8da1e7af559dd77bd9588c78b0f4a6a5424d049d9dec3379f5246bad26733" },
		new TestItem<int, string>
			{ Input = 32, Expected = "0xf9ec7c6a442296c92352b4b8e74186bc12318f0f6c33cdbea45cf235ab4ba36c" },
		new TestItem<int, string>
			{ Input = 33, Expected = "0xbaca33a265173ec828378e1842d50183d72fc74713e5814db8fd11258b139fc8" },
		new TestItem<int, string>
			{ Input = 34, Expected = "0xeed8e703ab3bb2e525e5f753a401bf506316963c3721bbb006f1f14d0c7e77ed" },
		new TestItem<int, string>
			{ Input = 63, Expected = "0x938256c0a20e8466829b0147215f37bbc21612b097f488680231830ccb6e8073" },
		new TestItem<int, string>
			{ Input = 64, Expected = "0x22efd0c3a556b3b604ddff123c229ba28c37990a4bb2440419689ecc721aea51" },
		new TestItem<int, string>
			{ Input = 65, Expected = "0x79c61dfa3f1c2851feddf768708eb9905f7b074912f1b67bc92c059996c0bb10" },
		new TestItem<int, string>
			{ Input = 100, Expected = "0x6a1eb54ce86e5737d03cf3818f4fac142ffb6c345fb7bf536a14700d650b6ce5" },
		new TestItem<int, string>
			{ Input = 117, Expected = "0xae9993eadbd78e61f5ef4058e8bf034d66bfd44a7ee4b8e95447f774c13b51f3" },
		new TestItem<int, string>
			{ Input = 127, Expected = "0xc2ac91fdd2cffa19c840a53ded2236aed5dccdff8566be4e4a6c95d2b9788f05" },
		new TestItem<int, string>
			{ Input = 128, Expected = "0x83c06103d03564515e17929e5083453c9ad7a35f92baa821a9758dbb7e6819a0" },
		new TestItem<int, string>
			{ Input = 129, Expected = "0xb8556216f6c2256faee05176bb1b429ff6aa3d514e9f49c2526ded4374ff8881" },
		new TestItem<int, string>
			{ Input = 178, Expected = "0xf4abd91b9392c636cf8b22ae4e54f72c5734bc05a80f8a430c6f41e1f7bd5fd1" },
		new TestItem<int, string>
			{ Input = 199, Expected = "0x3c392fc666bf0d1127e10989234c8f2f5d4d9cc1c4eba41c1d4736924988a8ac" },
		new TestItem<int, string>
			{ Input = 200, Expected = "0xbe8880a61f1039adca78c5d4d073044f142a033d1a31fb4f2cdb73d9501b424a" }
	};

	#endregion

	protected static void AssertAreEqual(byte[] expected, byte[] actual) => ClassicAssert.AreEqual(expected, actual);

	// if stars with 0x parses as input hexstring else ascii
	protected static byte[] ParseBytes(string input) =>
		input.StartsWith("0x") ? HexEncoding.Decode(input) : Encoding.ASCII.GetBytes(input);

}


[TestFixture]
internal class RandomHash2Test : RandomHash2TestBase {

	[OneTimeSetUp]
	public void Setup() {
		Hydrogen.CryptoEx.ModuleConfiguration.InitializeInternal();
	}

	[Test]
	public void TestRandomHash2_Standard() {
		foreach (var testItem in DATA_RANDOMHASH_STANDARD) {
			AssertAreEqual(ParseBytes(testItem.Expected), RandomHash2.Compute(ParseBytes(testItem.Input)));
		}
	}

	[Test]
	public void TestRandomHash2() {
		foreach (var testItem in DATA_RANDOMHASH) {
			var input = new byte[testItem.Input];
			Array.Copy(ParseBytes(DATA_BYTES), 0, input, 0, input.Length);
			AssertAreEqual(ParseBytes(testItem.Expected), RandomHash2.Compute(input));
		}
	}

	[Test]
	public void TestGetSetLastDWordConsistency() {
		var buffer = ParseBytes(DATA_BYTES);
		var hasher = RandomHash2.RandomHash2Instance();
		for (var idx = 1; idx <= 100; idx++) {
			var buffer2 = hasher.Hash(buffer);
			ClassicAssert.AreEqual(32768 + idx,
				RandomHashUtils.GetLastDWordLE(RandomHashUtils.SetLastDWordLE(buffer2, (uint)(32768 + idx))));
		}
	}

	[Test]
	public void TestRandomHash2StressTest() {
		var input = ParseBytes(DATA_BYTES);
		var randomHash2 = RandomHash2.RandomHash2Instance();
		const int NUM_ITER = 1000;
		for (var idx = 0; idx <= NUM_ITER; idx++) {
			Assert.DoesNotThrow(() => input = randomHash2.Hash(input));
		}
	}
}


[TestFixture]
internal class RandomHash2FastTest : RandomHash2TestBase {
	
	[OneTimeSetUp]
	public void Setup() {
		Hydrogen.CryptoEx.ModuleConfiguration.InitializeInternal();
	}

	[Test]
	public void TestRandomHash2_Standard() {
		foreach (var testItem in DATA_RANDOMHASH_STANDARD) {
			AssertAreEqual(ParseBytes(testItem.Expected), RandomHash2Fast.Compute(ParseBytes(testItem.Input)));
		}
	}

	[Test]
	public void TestRandomHash2() {
		foreach (var testItem in DATA_RANDOMHASH) {
			var input = new byte[testItem.Input];
			Array.Copy(ParseBytes(DATA_BYTES), 0, input, 0, input.Length);
			AssertAreEqual(ParseBytes(testItem.Expected), RandomHash2Fast.Compute(input));
		}
	}

	[Test]
	public void TestReferenceConsistency_RandomHash() {
		var input = ParseBytes(DATA_BYTES);
		var randomHash2 = RandomHash2.RandomHash2Instance();
		var randomHash2Fast = new RandomHash2Fast();

		for (var idx = 0; idx <= 100; idx++) {
			input = RandomHash2.Compute(input);
			AssertAreEqual(randomHash2.Hash(input), randomHash2Fast.Hash(input));
		}
	}

	[Test]
	public void TestCacheConsistency() {
		var input = ParseBytes(DATA_BYTES);
		var randomHash2Fast = new RandomHash2Fast();

		for (var idx = 0; idx <= 100; idx++) {
			input = RandomHash2.Compute(input);
			while (randomHash2Fast.CacheInstance.HasComputedHash()) {
				var cachedHash = randomHash2Fast.CacheInstance.PopComputedHash();
				AssertAreEqual(RandomHash2Fast.Compute(cachedHash.Header), cachedHash.RoundOutputs[0]);
			}
		}
	}

	[Test]
	public void TestRandomHash2FastStressTest() {
		var input = ParseBytes(DATA_BYTES);
		var randomHash2Fast = new RandomHash2Fast();
		const int NUM_ITER = 1000;
		for (var idx = 0; idx <= NUM_ITER; idx++) {
			Assert.DoesNotThrow(() => input = randomHash2Fast.Hash(input));
		}
	}
}
