// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using System;
//using NUnit.Framework;
//using System.IO;
//using System.Linq;
//using System.Text;
//using Hydrogen;
//using Hydrogen.Maths;
//using Hydrogen.NUnit;

//namespace Hydrogen.Tests {

//	[TestFixture]
//	public class SubRootTreeTests {

//		[Test]
//		public void NullRoot() {
//			var tree = new SubRootTree(CHF.SHA2_256);
//			ClassicAssert.AreEqual(null, tree.Root);
//		}

//		[Test]
//		public void Integration_Empty() {
//			var standard = new MerkleTree(CHF.SHA2_256);
//			var perfect = new SubRootTree(CHF.SHA2_256);
//			ClassicAssert.AreEqual(standard.Size, perfect.Size);
//			ClassicAssert.AreEqual(standard.Root, perfect.Root);
//		}

//		[Test]
//		public void Integration_Single() {
//			var standard = new MerkleTree(CHF.SHA2_256);
//			var perfect = new SubRootTree(CHF.SHA2_256);
//			var data = new Random(3133).NextBytes(32);
//			standard.Leafs.Add(data);
//			perfect.Leafs.Add(data);
//			ClassicAssert.AreEqual(standard.Size, perfect.Size);
//			ClassicAssert.AreEqual(standard.Root, perfect.Root);
//		}

//		[Test]
//		public void Integration_Double() {
//			var standard = new MerkleTree(CHF.SHA2_256);
//			var perfect = new SubRootTree(CHF.SHA2_256);
//			var rng = new Random(3133);
//			var data1 = rng.NextBytes(32);
//			var data2 = rng.NextBytes(32);
//			standard.Leafs.AddRange(new[] { data1, data2 });
//			perfect.Leafs.AddRange(new[] { data1, data2 });
//			ClassicAssert.AreEqual(standard.Size, perfect.Size);
//			ClassicAssert.AreEqual(standard.Root, perfect.Root);
//		}

//		[Test]
//		public void Integration_Many([Values(0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,1000)] int n) {
//			var standard = new MerkleTree(CHF.SHA2_256);
//			var perfect = new SubRootTree(CHF.SHA2_256);
//			var rng = new Random(31337*(n+1));
//			for (var i = 0; i < n; i++) {
//				var data = rng.NextBytes(32);
//				standard.Leafs.Add(data);
//				perfect.Leafs.Add(data);
//				ClassicAssert.AreEqual(standard.Size, perfect.Size);
//				ClassicAssert.AreEqual(standard.Root, perfect.Root);
//			}
//		}

//		[Test]
//		public void Integration_10kLeafs([Values(10000)] int n) {
//			var standard = new MerkleTree(CHF.SHA2_256);
//			var perfect = new SubRootTree(CHF.SHA2_256);
//			var rng = new Random(31337 * (n + 1));
//			for (var i = 0; i < n; i++) {
//				var data = rng.NextBytes(32);
//				standard.Leafs.Add(data);
//				perfect.Leafs.Add(data);
//			}
//			ClassicAssert.AreEqual(standard.Size, perfect.Size);
//			ClassicAssert.AreEqual(standard.Root, perfect.Root);

//		}

//	}
//}


