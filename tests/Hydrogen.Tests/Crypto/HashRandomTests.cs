// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using Hydrogen.Maths;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
public class HashRandomTests {


	[Test]
	public void BadCHF() {
		Assert.That(() => new HashRandom(CHF.ConcatBytes, Guid.Empty.ToByteArray()), Throws.InstanceOf<ArgumentOutOfRangeException>());
	}

	[Test]
	public void AcceptsMinSeed() {
		var seed = Tools.Array.Gen<byte>(HashRandom.MinimumSeedLength, 0);
		Assert.That(() => new HashRandom(seed), Throws.Nothing);
	}


	[Test]
	public void BadSeed_1() {
		var seed = Tools.Array.Gen<byte>(HashRandom.MinimumSeedLength - 1, 0);
		Assert.That(() => new HashRandom(seed), Throws.InstanceOf<ArgumentOutOfRangeException>());
	}


	[Test]
	public void BadSeed_2() {
		Assert.That(() => new HashRandom(null), Throws.ArgumentNullException);
	}


	[Test]
	public void GeneratesCorrectly([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		const int TotalHashIterations = 100;
		var digestSize = Hashers.GetDigestSizeBytes(chf);
		Debug.Assert(digestSize > 0);
		var seed = new Random(31337).NextBytes(digestSize);

		// Calculate all the bytes of iterating the seed TotalHashIterations times
		var expected = new ByteArrayBuilder();
		var lastValue = seed;
		for (var i = 0; i < TotalHashIterations; i++) {
			var nextHash = Hashers.Hash(chf, Tools.Array.Concat<byte>(lastValue, EndianBitConverter.Little.GetBytes((long)i)));
			expected.Append(nextHash);
			lastValue = nextHash;
		}

		var hashRandom = new HashRandom(chf, seed);
		var result = new ByteArrayBuilder();

		// Gen nothing
		result.Append(hashRandom.NextBytes(0));

		// Take first byte
		result.Append(hashRandom.NextBytes(1));

		// Gen nothing
		result.Append(hashRandom.NextBytes(0));

		// Take remaining bytes of first iteration
		result.Append(hashRandom.NextBytes(digestSize - 1));

		// Gen nothing
		result.Append(hashRandom.NextBytes(0));

		// Take full second iteration
		result.Append(hashRandom.NextBytes(digestSize));

		// Take 3rd iteration except last byte
		result.Append(hashRandom.NextBytes(digestSize - 1));

		// Gen nothing
		result.Append(hashRandom.NextBytes(0));

		// Take 2 bytes (last byte of 3rd iteration and first bytes of 4th iteration)
		result.Append(hashRandom.NextBytes(2));

		// Gen nothing
		result.Append(hashRandom.NextBytes(0));

		// Take remaining bytes of 4th iteration and all bytes from remaining iterations
		result.Append(hashRandom.NextBytes((digestSize - 1) + digestSize * (TotalHashIterations - 4)));

		// Gen nothing
		result.Append(hashRandom.NextBytes(0));


		Assert.That(result.ToArray(), Is.EqualTo(expected.ToArray()));

	}


}
