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
using System.Linq;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

public abstract class MerkleListTestsBase {

	protected abstract IDisposable CreateMerkleList(CHF chf, out IMerkleList<string> merkleList);

	[Test]
	public void EmptyHasZeroHash([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		using (CreateMerkleList(chf, out var merkleList)) {
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Hashers.ZeroHash(chf)));
		}
	}

	[Test]
	public void TestSimple_1([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		using (CreateMerkleList(chf, out var merkleList)) {
			merkleList.AddRange(new[] { "Alpha", "Beta", "Gamma" });
			merkleList.Remove("Beta");
			merkleList.RemoveAt(0);
			merkleList.Insert(0, "Beta");
			merkleList.Insert(0, "AlphaX");
			merkleList[0] = "Alpha";
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "Alpha", "Beta", "Gamma" }, chf)));
		}
	}

	[Test]
	public void TestSimple_2([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		var memStream = new MemoryStream();
		var clusteredList = new StreamMappedMerkleList<string>(memStream, 256, chf);
		clusteredList.Load();
		clusteredList.Add("beta");
		clusteredList.Insert(0, "alpha");
		clusteredList.Insert(2, "gammaa");
		clusteredList.Add("delta");
		clusteredList.Update(2, "gamma");
		clusteredList.Add("epsilon");
		Assert.That(clusteredList.ToArray(), Is.EqualTo(new[] { "alpha", "beta", "gamma", "delta", "epsilon" }));
		Assert.That(clusteredList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "alpha", "beta", "gamma", "delta", "epsilon" }, chf)));
	}

	[Test]
	public void TestSimple_3([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		using (CreateMerkleList(chf, out var merkleList)) {
			merkleList.AddRange(new[] { "Alpha", "Beta", "Gamma" });
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "Alpha", "Beta", "Gamma" }, chf)));

			merkleList.Remove("Beta");
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "Alpha", "Gamma" }, chf)));

			merkleList.RemoveAt(0);
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "Gamma" }, chf)));

			merkleList.Insert(0, "Beta");
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "Beta", "Gamma" }, chf)));

			merkleList.Insert(0, "AlphaX");
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "AlphaX", "Beta", "Gamma" }, chf)));

			merkleList[0] = "Alpha";
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "Alpha", "Beta", "Gamma" }, chf)));

		}
	}

	[Test]
	public void SupportsNullItems([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		using (CreateMerkleList(chf, out var merkleList)) {
			var nullHashValue = Tools.Array.Gen<byte>(Hashers.GetDigestSizeBytes(chf), 0);
			Assert.That(() => merkleList.Add(null), Throws.Nothing);
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(nullHashValue));

			Assert.That(() => merkleList.Insert(0, null), Throws.Nothing);
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new string[] { null, null }, chf)));

			merkleList.Add("alpha");
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new string[] { null, null, "alpha" }, chf)));

			Assert.That(() => merkleList.Update(2, null), Throws.Nothing);
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new string[] { null, null, null }, chf)));

			merkleList.Clear();
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Hashers.ZeroHash(chf)));
		}
	}

	[Test]
	public void TestSimple_4([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		using (CreateMerkleList(chf, out var merkleList)) {
			merkleList.AddRange(new[] { "Alpha", "Beta", "Gamma" });
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "Alpha", "Beta", "Gamma" }, chf)));

			merkleList.Remove("Beta");
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "Alpha", "Gamma" }, chf)));

			merkleList.RemoveAt(0);
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "Gamma" }, chf)));

			merkleList.Insert(0, "Beta");
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "Beta", "Gamma" }, chf)));

			merkleList.Insert(0, "AlphaX");
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "AlphaX", "Beta", "Gamma" }, chf)));

			merkleList[0] = "Alpha";
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "Alpha", "Beta", "Gamma" }, chf)));

		}
	}

	[Test]
	public void Insert_Empty([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		using (CreateMerkleList(chf, out var merkleList)) {
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Hashers.ZeroHash(chf)));
			merkleList.InsertRange(0, Enumerable.Empty<string>());
			Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Hashers.ZeroHash(chf)));
		}
	}

	[Test]
	public void AddRangeEmptyNullStrings([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		using (CreateMerkleList(chf, out var merkleList)) {
			string[] input = { string.Empty, null, string.Empty, null };
			merkleList.AddRange(input);
			Assert.That(merkleList.Count, Is.EqualTo(4));
			Assert.That(merkleList, Is.EqualTo(input));
		}
	}

	[Test]
	public void IndexOf_NullEmptyStrings([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		using (CreateMerkleList(chf, out var merkleList)) {
			string[] input = { string.Empty, null, string.Empty, null };
			merkleList.AddRange(input);
			Assert.That(merkleList.IndexOf(null), Is.EqualTo(1));
			merkleList[1] = "z";
			Assert.That(merkleList.IndexOf(null), Is.EqualTo(3));
			Assert.That(merkleList.IndexOf(string.Empty), Is.EqualTo(0));
			merkleList[0] = "z";
			Assert.That(merkleList.IndexOf(string.Empty), Is.EqualTo(2));
		}
	}

	[Test]
	public void IntegrationTests([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values(100)] int iterations) {
		var rng = new Random(31337);
		using (CreateMerkleList(chf, out var merkleList)) {
			var expectedList = new ExtendedList<string>();
			AssertEx.ListIntegrationTest(
				merkleList,
				100,
				(rng, i) => Tools.Collection.GenerateArray<string>(i, _ => rng.NextString(100)),
				false,
				iterations,
				expectedList,
				() => Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(expectedList, chf)))
			);
		}
	}


}
