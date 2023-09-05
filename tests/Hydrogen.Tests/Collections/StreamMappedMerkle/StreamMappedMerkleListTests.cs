// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;
using System.IO;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamMappedMerkleListTests : MerkleListTestsBase {
	private const int DefaultClusterSize = 256;

	[Test]
	public void TestAdaptedScopes([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		var memStream = new MemoryStream();
		using var clusteredList = new StreamMappedMerkleList<string>(memStream, 256, chf, autoLoad: true);
		clusteredList.Add("beta");
		clusteredList.Insert(0, "alpha");
		clusteredList.Add("alphaa");
		clusteredList.RemoveAt(2);
		clusteredList.Insert(2, "gammaa");
		clusteredList.Add("delta");
		clusteredList.Update(2, "gamma");
		clusteredList.Add("epsilon");
		Assert.That(clusteredList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "alpha", "beta", "gamma", "delta", "epsilon" }, chf)));
	}

	protected override IDisposable CreateMerkleList([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, out IMerkleList<string> merkleList) {
		var memStream = new MemoryStream();
		var clusteredList = new StreamMappedMerkleList<string>(memStream, DefaultClusterSize, chf, autoLoad: true);
		merkleList = clusteredList;
		return new Disposables(memStream, clusteredList);
	}

}
