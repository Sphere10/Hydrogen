using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Text;
using Sphere10.Framework;
using Sphere10.Framework.Maths;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

	public abstract class MerkleListTestsBase {

		protected abstract IDisposable CreateMerkleList(CHF chf, out IMerkleList<string> merkleList);

		[Test]
		public void TestSimple_1([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
			using (CreateMerkleList(chf, out var merkleList)) {
				merkleList.AddRange(new[] { "Alpha", "Beta", "Gamma" });
				merkleList.Remove("Beta");
				merkleList.RemoveAt(0);
				merkleList.Insert(0, "Beta");
				merkleList.Insert(0, "AlphaX");
				merkleList[0] = "Alpha";
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "Alpha", "Beta", "Gamma" }, chf)));
			}
		}


		[Test]
		public void TestSimple_2([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
			var memStream = new MemoryStream();
			var clusteredList = new StreamMappedMerkleList<string>(memStream, 256, chf);

			clusteredList.Add("beta");
			clusteredList.Insert(0, "alpha");
			clusteredList.Insert(2, "gammaa");
			clusteredList.Add("delta");
			clusteredList.Update(2, "gamma");
			clusteredList.Add("epsilon");
			Assert.That(clusteredList.ToArray(), Is.EqualTo(new[] { "alpha", "beta", "gamma", "delta", "epsilon" }));
			Assert.That(clusteredList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "alpha", "beta", "gamma", "delta", "epsilon" }, chf)));
		}

		[Test]
		public void TestSimple_3([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
			using (CreateMerkleList(chf, out var merkleList)) {
				merkleList.AddRange(new[] { "Alpha", "Beta", "Gamma" });
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "Alpha", "Beta", "Gamma" }, chf)));

				merkleList.Remove("Beta");
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "Alpha", "Gamma" }, chf)));
				
				merkleList.RemoveAt(0);
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "Gamma" }, chf)));

				merkleList.Insert(0, "Beta");
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "Beta", "Gamma" }, chf)));

				merkleList.Insert(0, "AlphaX");
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "AlphaX", "Beta", "Gamma" }, chf)));

				merkleList[0] = "Alpha";
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "Alpha", "Beta", "Gamma" }, chf)));

			}
		}


		[Test]
		public void NoNullItems([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
			using (CreateMerkleList(chf, out var merkleList)) {
				Assert.That(() => merkleList.Add(null), Throws.ArgumentNullException);
				Assert.That(merkleList.MerkleTree.Root, Is.Null);

				Assert.That(() => merkleList.Insert(0, null), Throws.ArgumentNullException);
				Assert.That(merkleList.MerkleTree.Root, Is.Null);

				merkleList.Add("alpha");
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "alpha" }, chf)));

				Assert.That(() => merkleList.Update(0, null), Throws.ArgumentNullException);
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "alpha" }, chf)));

				merkleList.Clear();
				Assert.That(merkleList.MerkleTree.Root, Is.Null);
			}
		}

		[Test]
		public void TestSimple_4([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
			using (CreateMerkleList(chf, out var merkleList)) {
				merkleList.AddRange(new[] { "Alpha", "Beta", "Gamma" });
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "Alpha", "Beta", "Gamma" }, chf)));

				merkleList.Remove("Beta");
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "Alpha", "Gamma" }, chf)));

				merkleList.RemoveAt(0);
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "Gamma" }, chf)));

				merkleList.Insert(0, "Beta");
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "Beta", "Gamma" }, chf)));

				merkleList.Insert(0, "AlphaX");
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "AlphaX", "Beta", "Gamma" }, chf)));

				merkleList[0] = "Alpha";
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(new[] { "Alpha", "Beta", "Gamma" }, chf)));

			}
		}

		[Test]
		public void Insert_Empty([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
			using (CreateMerkleList(chf, out var merkleList)) {
				Assert.That(merkleList.MerkleTree.Root, Is.Null);
				merkleList.InsertRange(0, Enumerable.Empty<string>());
				Assert.That(merkleList.MerkleTree.Root, Is.Null);
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
					() => Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(expectedList, chf)))
				);
			}
		}


	}
}
