// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;
using Hydrogen.NUnit;

namespace Hydrogen.Tests.Merkle;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class MerkleListAdapterTests : MerkleListTestsBase {

	protected override IDisposable CreateMerkleList(CHF chf, out IMerkleList<string> merkleList) {
		merkleList = new MerkleListAdapter<string>(chf);
		return Disposables.None;
	}

	[Test]
	public void IntegrationTests_Heavy([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values(100)] int iterations) {
		var rng = new Random(31337);
		using (CreateMerkleList(chf, out var merkleList)) {
			var expectedList = new ExtendedList<string>();
			AssertEx.ListIntegrationTest(
				merkleList,
				1000,
				(rng, i) => Tools.Collection.GenerateArray(i, _ => rng.NextString(100)),
				false,
				iterations,
				expectedList,
				() => Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(expectedList, chf)))
			);
		}
	}

}
