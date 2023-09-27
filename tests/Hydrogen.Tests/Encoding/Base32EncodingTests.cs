// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using BenchmarkDotNet.Attributes;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class Base32EncodingTests {

	public void TestEmpty() {
		Assert.That(Base32Encoding.Decode(string.Empty), Is.EqualTo(Array.Empty<byte>()));
	}

	[Test]
	public void IntegrationTest_Random() {
		const int Iterations = 10000;
		const int MaxByteLength = 100;
		var rng = new Random(31337);
		for(var i = 0; i < Iterations; i++) {
			var bytes =  rng.NextBytes(rng.Next(0, MaxByteLength+1));
			var encoded = Base32Encoding.Encode(bytes);
			var decoded = Base32Encoding.Decode(encoded);
			Assert.That(decoded, Is.EqualTo(bytes));
		}
	}

}