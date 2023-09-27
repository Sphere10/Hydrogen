// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;

namespace Hydrogen.CryptoEx.Tests;

[TestFixture]
[Parallelizable(ParallelScope.None)]
public class Blake2bFastConsistency {

	[Test, Sequential]
	public void Empty(
		[Values(CHF.Blake2b_512, CHF.Blake2b_384, CHF.Blake2b_256, CHF.Blake2b_224, CHF.Blake2b_160, CHF.Blake2b_128, CHF.Blake2s_256, CHF.Blake2s_224, CHF.Blake2s_128, CHF.Blake2s_160)]
		CHF normal,
		[Values(CHF.Blake2b_512_Fast, CHF.Blake2b_384_Fast, CHF.Blake2b_256_Fast, CHF.Blake2b_224_Fast, CHF.Blake2b_160_Fast, CHF.Blake2b_128_Fast, CHF.Blake2s_256_Fast, CHF.Blake2s_224_Fast, CHF.Blake2s_128_Fast, CHF.Blake2s_160_Fast)]
		CHF fast) {
		Assert.That(Hashers.Hash(normal, Array.Empty<byte>()), Is.EqualTo(Hashers.Hash(fast, Array.Empty<byte>())));
	}

	[Test, Sequential]
	public void Random(
		[Values(CHF.Blake2b_512, CHF.Blake2b_384, CHF.Blake2b_256, CHF.Blake2b_224, CHF.Blake2b_160, CHF.Blake2b_128, CHF.Blake2s_256, CHF.Blake2s_224, CHF.Blake2s_128, CHF.Blake2s_160)]
		CHF normal,
		[Values(CHF.Blake2b_512_Fast, CHF.Blake2b_384_Fast, CHF.Blake2b_256_Fast, CHF.Blake2b_224_Fast, CHF.Blake2b_160_Fast, CHF.Blake2b_128_Fast, CHF.Blake2s_256_Fast, CHF.Blake2s_224_Fast, CHF.Blake2s_128_Fast, CHF.Blake2s_160_Fast)]
		CHF fast) {
		var bytes = new Random(31337).NextBytes(100);
		Assert.That(Hashers.Hash(normal, bytes), Is.EqualTo(Hashers.Hash(fast, bytes)));
	}


	[Test, Sequential]
	public void Complex(
		[Values(CHF.Blake2b_512, CHF.Blake2b_384, CHF.Blake2b_256, CHF.Blake2b_224, CHF.Blake2b_160, CHF.Blake2b_128, CHF.Blake2s_256, CHF.Blake2s_224, CHF.Blake2s_128, CHF.Blake2s_160)]
		CHF normal,
		[Values(CHF.Blake2b_512_Fast, CHF.Blake2b_384_Fast, CHF.Blake2b_256_Fast, CHF.Blake2b_224_Fast, CHF.Blake2b_160_Fast, CHF.Blake2b_128_Fast, CHF.Blake2s_256_Fast, CHF.Blake2s_224_Fast, CHF.Blake2s_128_Fast, CHF.Blake2s_160_Fast)]
		CHF fast) {
		var rng = new Random(31337);

		using (Hashers.BorrowHasher(normal, out var normalHasher))
		using (Hashers.BorrowHasher(fast, out var fasterHasher)) {
			for (var i = 0; i < rng.Next(0, 100); i++) {
				var block = rng.NextBytes(rng.Next(0, 100));
				normalHasher.Transform(block);
				fasterHasher.Transform(block);
			}
			Assert.That(normalHasher.GetResult(), Is.EqualTo(fasterHasher.GetResult()));
			Assert.That(normalHasher.GetResult(), Is.EqualTo(fasterHasher.GetResult()));
			Assert.That(normalHasher.GetResult(), Is.EqualTo(fasterHasher.GetResult()));
			Assert.That(normalHasher.GetResult(), Is.EqualTo(fasterHasher.GetResult()));
			Assert.That(normalHasher.GetResult(), Is.EqualTo(fasterHasher.GetResult()));
		}
	}


}
