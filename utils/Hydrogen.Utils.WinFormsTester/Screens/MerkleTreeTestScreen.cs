// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Linq;
using System.Text;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class MerkleTreeTestScreen : ApplicationScreen {
	private TextWriter _outputWriter;

	public MerkleTreeTestScreen() {
		InitializeComponent();
		_outputWriter = new TextBoxWriter(_outputTextBox);
	}

	private void _printAlphabetButton_Click(object sender, EventArgs e) {
		var elems = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
		var tree = new SimpleMerkleTree(CHF.ConcatBytes);

		_outputWriter.WriteLine($"Tree: #: {tree.Size.LeafCount}, Levels: {tree.Size.Height}");
		foreach (var elem in elems) {
			tree.Leafs.Add(Encoding.ASCII.GetBytes(elem));
			_outputWriter.WriteLine($"Tree: #: {tree.Size.LeafCount}, Levels: {tree.Size.Height}");
			if (tree.Size.LeafCount > 0) {
				_outputWriter.PrintTree(
					tree.GetRootNode(),
					c => Encoding.ASCII.GetString(c.Hash),
					c => {
						if (!MerkleMath.IsLeaf(c.Coordinate)) {
							var childs = tree.GetChildren(c.Coordinate);
							return
								new[] { childs.Left }
									.Concat(childs.Right != null ? new[] { childs.Right } : Enumerable.Empty<MerkleNode>());
						}
						return Enumerable.Empty<MerkleNode>();
					}
				);
			}
			_outputWriter.WriteLine();

			//// Write existence proof path for last item
			//var lastItemIX = tree.LeafCount - 1;
			//var existenceProof = tree.GetExistenceProof(lastItemIX).ToArray();
			//// Print existence proof
			//foreach (var hash in existenceProof.WithDescriptions()) {
			//	_outputWriter.Write(hash.Item);
			//	if (!hash.Description.HasFlag(EnumeratedItemDescription.Last)) 
			//		_outputWriter.Write(" -> ");
			//}
			//_outputWriter.WriteLine();

			//// Verify existence proof
			//var passed = MerkleTreeMath.VerifyExistenceProof((h1, h2) => h1 + h2, tree.Root, tree.LeafCount, MerkleCoordinate.LeafAt(lastItemIX), tree.Leafs.Last(), existenceProof);
			//_outputWriter.Write($"ExistenceProofPassed: {passed}");
		}
	}

	private void _printIntegersButton_Click(object sender, EventArgs e) {
		var tree = new SimpleMerkleTree(CHF.ConcatBytes);
		_outputWriter.WriteLine($"Tree: #: {tree.Size.LeafCount}, Levels: {tree.Size.Height}");
		foreach (var elem in Enumerable.Range(1, 100)) {
			tree.Leafs.Add(Encoding.ASCII.GetBytes(elem.ToString()));
			_outputWriter.WriteLine($"Tree: #: {tree.Size.LeafCount}, Levels: {tree.Size.Height}");
			if (tree.Size.LeafCount > 0) {

				_outputWriter.PrintTree(
					tree.GetRootNode(),
					c => Encoding.ASCII.GetString(c.Hash),
					c => {
						if (MerkleMath.IsLeaf(c.Coordinate))
							return Enumerable.Empty<MerkleNode>();

						var childs = tree.GetChildren(c.Coordinate);
						return new[] { childs.Left }.Concat(childs.Right != null ? new[] { childs.Right } : Enumerable.Empty<MerkleNode>());
					}
				);
			}
			_outputWriter.WriteLine();

			//// Write existence proof path for last item
			//var lastItemIX = tree.LeafCount - 1;
			//var existenceProof = tree.GetExistenceProof(lastItemIX).ToArray();
			//// Print existence proof
			//foreach (var hash in existenceProof.WithDescriptions()) {
			//	_outputWriter.Write(hash.Item);
			//	if (!hash.Description.HasFlag(EnumeratedItemDescription.Last))
			//		_outputWriter.Write(" -> ");
			//}
			//_outputWriter.WriteLine();

			//// Verify existence proof
			//var passed = MerkleTreeMath.VerifyExistenceProof((h1, h2) => h1 + h2, tree.Root, tree.LeafCount, MerkleCoordinate.LeafAt(lastItemIX), tree.Leafs.Last(), existenceProof);
			//_outputWriter.Write($"ExistenceProofPassed: {passed}");
		}
	}

	private void _sumPow2Button_Click(object sender, EventArgs e) {

		var start = Enumerable.Empty<int>();
		for (var i = 0; i < 1000; i++) {
			var left = i;
			var right = i + 1;
			var a = Pow2.CalculatePow2Partition(left);
			var b = Pow2.CalculatePow2Partition(right);
			var c = Pow2.Mul(a, b);
			var expected = Pow2.CalculatePow2Partition(left * right);
			var eq = expected.SequenceEqual(c);
			_outputWriter.WriteLine($"{left} * {right} = {left * right}: {c.ToDelimittedString(", ")}");
			if (!eq)
				_outputWriter.Write($"ERROR: Expected - {expected.ToDelimittedString(", ")}");
			//start = Pow2.AddOne(start);
		}

	}

	private void _perfectMerkleButton_Click(object sender, EventArgs e) {
		//var n = 100;
		//var standard = new MerkleTree(CHF.SHA2_256);
		//var perfect = new SubRootTree(CHF.SHA2_256);
		//var rng = new Random(31337 * (n + 1));
		//for (var i = 0; i < n; i++) {
		//	var data = /*new[] { (byte)i }; //*/rng.NextBytes(32);
		//	standard.Leafs.Add(data);
		//	perfect.Leafs.Add(data);
		//	_outputWriter.WriteLine($"EQ: {standard.Size.LeafCount == perfect.Size.LeafCount}, Standard Count: {standard.Size.LeafCount}, Perfect Count: {perfect.Size.LeafCount}");
		//	_outputWriter.WriteLine($"EQ: {ByteArrayEqualityComparer.Instance.Equals(standard.Root, perfect.Root)}, Standard Root: {standard.Root.ToHexString()}, Perfect Root: {perfect.Root.ToHexString()}");
		//}

		//var standard = new MerkleTree(CHF.ConcatBytes);
		//var perfect = new PerfectTree(CHF.ConcatBytes);
		//for (var i = 0; i < 15; i++) {
		//	var data = new[] { (byte)i };
		//	standard.Add(data);
		//	perfect.Add(data);
		//	_outputWriter.WriteLine($"EQ: {standard.Count == perfect.Count}, Standard Count: {standard.Count}, Perfect Count: {perfect.Count}");
		//	_outputWriter.WriteLine($"EQ: {ByteArrayEqualityComparer.Instance.Equals(standard.Root, perfect.Root)}, Standard Root: {standard.Root.ToHexString()}, Perfect Root: {perfect.Root.ToHexString()}");
		//}


		//var _hashStack = new int[10];
		//var _ix = -1; 

		//void AddOne() {
		//	int aggHash = 0;
		//	while (true) {
		//		if (_ix < 0 || _hashStack[_ix] > aggHash) {
		//			_hashStack[++_ix] = aggHash;
		//			return;
		//		}
		//		var tip = _hashStack[_ix--];
		//		aggHash = tip + 1;
		//	}
		//}

		//var i = 0;
		//do {
		//	_outputWriter.WriteLine($"{i}: {_hashStack.Take(_ix + 1).ToDelimittedString(" ")}");
		//	AddOne();
		//} while (i++ <= 1000);

		////	for (int i = 0; i <= 10000; i++) {
		////		_outputWriter.WriteLine($"{i}: {AsPower2Sum(i).ToDelimittedString(" ")}");
		////	}
		////}

	}

	private void _printInvPow2Button_Click(object sender, EventArgs e) {
		_outputWriter.WriteLine("Inv Pow2");
		var sum = 0M;
		for (var i = 0; i < 64; i++) {
			var pow2 = 1M / (1UL << i);
			sum += pow2;
			_outputWriter.WriteLine($"{i}: 1/2^{i} = {pow2}, sum = {sum}");
		}
	}

	private void _treePerfTestsButton_Click(object sender, EventArgs e) {
		var items = Tools.Maths.RNG.NextByteArrays(32, 1000000);
		var reference = new SimpleMerkleTree(CHF.SHA2_256, items);
		var flat = new FlatMerkleTree(CHF.SHA2_256, items);
		var root = reference.Root;
		root = flat.Root;


		var itemList = Tools.Collection.Generate(() => Tools.Maths.RNG.NextByteArrays(32, Tools.Maths.RNG.Next(0, 100))).Take(5).ToArray();
		foreach (var list in itemList) {
			var start = DateTime.Now;
			reference.Leafs.AddRange(list);
			root = reference.Root;
			var end = DateTime.Now;
			_outputWriter.WriteLine($"Reference: Root: {MURMUR3_32.Execute(root)} Count {reference.Size.LeafCount}, Added {list.Length}, Duration: {end.Subtract(start).TotalMilliseconds}");
			start = DateTime.Now;
			flat.Leafs.AddRange(list);
			root = flat.Root;
			end = DateTime.Now;
			_outputWriter.WriteLine($"Flat: Root: {MURMUR3_32.Execute(root)} Count {flat.Size.LeafCount}, Added {list.Length}, Duration: {end.Subtract(start).TotalMilliseconds}");
		}

	}
}
