// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;
using System.Linq;
using System.Text;
using Hydrogen;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests.Merkle;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class SimpleMerkleTreeTests {

	[Test]
	public void Add() {
		var elems = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
		var expectedTrees =
			Enumerable.Range(0, elems.Count())
				.Select(x => new SimpleMerkleTree(CHF.ConcatBytes, elems.Take(x).Select(Encoding.ASCII.GetBytes)));

		var testTree = new SimpleMerkleTree(CHF.ConcatBytes);
		foreach (var (newLeaf, expectedTree) in new string[] { null }.Concat(elems).Zip(expectedTrees, Tuple.Create)) {
			if (newLeaf != null)
				testTree.Leafs.Add(Encoding.ASCII.GetBytes(newLeaf));
			ClassicAssert.AreEqual(expectedTree.Root, testTree.Root);
		}
	}

	[Test]
	public void Insert() {
		var expected = new SimpleMerkleTree(CHF.ConcatBytes, new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" }.Select(Encoding.ASCII.GetBytes));
		var test = new SimpleMerkleTree(CHF.ConcatBytes, new[] { "A", "B", "C", "G", "H", "I" }.Select(Encoding.ASCII.GetBytes));
		ClassicAssert.AreNotEqual(expected.Root, test.Root);
		test.Leafs.InsertRange(3, new[] { "D", "E", "F" }.Select(Encoding.ASCII.GetBytes));
		ClassicAssert.AreEqual(expected.Root, test.Root);
	}

	[Test]
	public void Update() {
		var expected = new SimpleMerkleTree(CHF.ConcatBytes, new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" }.Select(Encoding.ASCII.GetBytes));
		var test = new SimpleMerkleTree(CHF.ConcatBytes, new[] { "A", "B", "C", "Z", "Z", "Z", "G", "H", "I" }.Select(Encoding.ASCII.GetBytes));
		ClassicAssert.AreNotEqual(expected.Root, test.Root);
		test.Leafs.UpdateRange(3, new[] { "D", "E", "F" }.Select(Encoding.ASCII.GetBytes));
		ClassicAssert.AreEqual(expected.Root, test.Root);
	}

	[Test]
	public void Remove() {
		var expected = new SimpleMerkleTree(CHF.ConcatBytes, new[] { "A", "B", "C", "G", "H", "I" }.Select(Encoding.ASCII.GetBytes));
		var test = new SimpleMerkleTree(CHF.ConcatBytes, new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I" }.Select(Encoding.ASCII.GetBytes));
		ClassicAssert.AreNotEqual(expected.Root, test.Root);
		test.Leafs.RemoveRange(3, 3);
		ClassicAssert.AreEqual(expected.Root, test.Root);
	}

}
