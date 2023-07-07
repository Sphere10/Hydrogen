// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Text;
using Hydrogen.Maths;
using Hydrogen.NUnit;
using Hydrogen;

namespace Hydrogen.Tests
{

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class StreamMappedMerkleListTests : MerkleListTestsBase {
		private const int DefaultClusterSize = 256;

		[Test]
		public void TestAdaptedScopes([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
			var memStream = new MemoryStream();
			var clusteredList = new StreamMappedMerkleList<string>(memStream, 256, chf);
			if (clusteredList.RequiresLoad) 
				clusteredList.Load();

			using (clusteredList.EnterAddScope("beta"));
			using (clusteredList.EnterInsertScope(0, "alpha"));
			using (clusteredList.EnterAddScope("alphaa"));
			clusteredList.RemoveAt(2);
			using (clusteredList.EnterInsertScope(2, "gammaa"));
			using (clusteredList.EnterAddScope("delta"));
			using (clusteredList.EnterUpdateScope(2, "gamma"));
			using (clusteredList.EnterAddScope("epsilon"));
			Assert.That(clusteredList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new [] { "alpha", "beta", "gamma", "delta", "epsilon" }, chf)));
		}

		protected override IDisposable CreateMerkleList([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, out IMerkleList<string> merkleList) {
			var memStream = new MemoryStream();
			var clusteredList = new StreamMappedMerkleList<string>(memStream, DefaultClusterSize, chf);
			if (clusteredList.RequiresLoad)
				clusteredList.Load();

			merkleList = clusteredList;
			return memStream;
		}

	}
}
