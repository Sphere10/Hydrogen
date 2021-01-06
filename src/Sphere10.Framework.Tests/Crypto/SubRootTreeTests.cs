//using System;
//using NUnit.Framework;
//using System.IO;
//using System.Linq;
//using System.Text;
//using Sphere10.Framework;
//using Sphere10.Framework.Maths;
//using Sphere10.Framework.NUnit;

//namespace Sphere10.Framework.UnitTests {

//	[TestFixture]
//	public class SubRootTreeTests {

//		[Test]
//		public void NullRoot() {
//			var tree = new SubRootTree(CHF.SHA2_256);
//			Assert.AreEqual(null, tree.Root);
//		}

//		[Test]
//		public void Integration_Empty() {
//			var standard = new MerkleTree(CHF.SHA2_256);
//			var perfect = new SubRootTree(CHF.SHA2_256);
//			Assert.AreEqual(standard.Size, perfect.Size);
//			Assert.AreEqual(standard.Root, perfect.Root);
//		}

//		[Test]
//		public void Integration_Single() {
//			var standard = new MerkleTree(CHF.SHA2_256);
//			var perfect = new SubRootTree(CHF.SHA2_256);
//			var data = new Random(3133).NextBytes(32);
//			standard.Leafs.Add(data);
//			perfect.Leafs.Add(data);
//			Assert.AreEqual(standard.Size, perfect.Size);
//			Assert.AreEqual(standard.Root, perfect.Root);
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
//			Assert.AreEqual(standard.Size, perfect.Size);
//			Assert.AreEqual(standard.Root, perfect.Root);
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
//				Assert.AreEqual(standard.Size, perfect.Size);
//				Assert.AreEqual(standard.Root, perfect.Root);
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
//			Assert.AreEqual(standard.Size, perfect.Size);
//			Assert.AreEqual(standard.Root, perfect.Root);

//		}

//	}
//}
