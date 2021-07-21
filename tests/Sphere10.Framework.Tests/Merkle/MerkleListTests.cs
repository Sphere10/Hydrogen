using System;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Text;
using Sphere10.Framework;
using Sphere10.Framework.Maths;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class MerkleListTests {

		[Test]
		public void CRUD() {
			var expected = new MerkleList<string>(new StringSerializer(), CHF.SHA2_256) { 
				"Alpha",
				"Beta",
				"Gamma"
			};


			var test = new MerkleList<string>(new StringSerializer(), CHF.SHA2_256) {
				"Alpha",
				"Beta",
				"Gamma"
			};
			var root = test.MerkleTree.Root;
			test.Remove("Beta");
			test.RemoveAt(0);
			test.Insert(0, "Beta");
			test.Insert(0, "AlphaX");
			test[0] = "Alpha";

			Assert.AreEqual(expected.MerkleTree.Root, test.MerkleTree.Root);

		}
	}
}
