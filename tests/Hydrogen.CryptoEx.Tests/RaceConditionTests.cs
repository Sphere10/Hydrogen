// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Hydrogen.CryptoEx.Tests;

[TestFixture]
[Parallelizable(ParallelScope.None)]
// Similar to RaceConditionTests in Hydrogen.Tests except for all CHF values
public class RaceConditionTests {

	[Test]
	public void EmptyPreImage([Values(100)] int parallelRuns, [Values] CHF chf) {
		Parallel.For(0, parallelRuns, _ => RaceConditionScenario());

		void RaceConditionScenario() {
			var expected = Hashers.Hash(chf, Array.Empty<byte>());
			using (Hashers.BorrowHasher(chf, out var hasher)) {
				Assert.That(hasher.GetResult(), Is.EqualTo(expected));
			}
			;
		}
	}

	[Test]
	public void EmptyPreImage_MultipleGetResult([Values(100)] int parallelRuns, [Values] CHF chf) {
		Parallel.For(0, parallelRuns, _ => RaceConditionScenario());

		void RaceConditionScenario() {
			var expected = Hashers.Hash(chf, Array.Empty<byte>());
			using (Hashers.BorrowHasher(chf, out var hasher)) {
				Assert.That(hasher.GetResult(), Is.EqualTo(expected));
				Assert.That(hasher.GetResult(), Is.EqualTo(expected));
				Assert.That(hasher.GetResult(), Is.EqualTo(expected));
				Assert.That(hasher.GetResult(), Is.EqualTo(expected));
				Assert.That(hasher.GetResult(), Is.EqualTo(expected));
			}
			;
		}
	}


	[Test]
	public void RandomPreImage([Values(100)] int parallelRuns, [Values] CHF chf) {
		Parallel.For(0, parallelRuns, RaceConditionScenario);

		void RaceConditionScenario(int x) {
			var rng = new Random(x * parallelRuns);
			var bytes = new ByteArrayBuilder();
			using (Hashers.BorrowHasher(chf, out var hasher)) {
				for (var i = 0; i < rng.Next(0, 100); i++) {
					var block = rng.NextBytes(rng.Next(0, 100));
					bytes.Append(block);
					hasher.Transform(block);
				}
				Assert.That(hasher.GetResult(), Is.EqualTo(Hashers.Hash(chf, bytes.ToArray())));
			}
			;
		}
	}


	[Test]
	public void RandomPreImage_MultipleGetResult([Values(100)] int parallelRuns, [Values] CHF chf) {
		Parallel.For(0, parallelRuns, RaceConditionScenario);

		void RaceConditionScenario(int x) {
			var rng = new Random(x * parallelRuns);
			var bytes = new ByteArrayBuilder();
			using (Hashers.BorrowHasher(chf, out var hasher)) {
				for (var i = 0; i < rng.Next(0, 100); i++) {
					var block = rng.NextBytes(rng.Next(0, 100));
					bytes.Append(block);
					hasher.Transform(block);
				}
				Assert.That(hasher.GetResult(), Is.EqualTo(Hashers.Hash(chf, bytes.ToArray())));
				Assert.That(hasher.GetResult(), Is.EqualTo(Hashers.Hash(chf, bytes.ToArray())));
				Assert.That(hasher.GetResult(), Is.EqualTo(Hashers.Hash(chf, bytes.ToArray())));
				Assert.That(hasher.GetResult(), Is.EqualTo(Hashers.Hash(chf, bytes.ToArray())));
				Assert.That(hasher.GetResult(), Is.EqualTo(Hashers.Hash(chf, bytes.ToArray())));
			}
			;
		}
	}


}
