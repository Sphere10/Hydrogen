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
		protected const CHF DefaultCHF = CHF.SHA2_256;

		private byte[] ComputeRoot<TItem>(params TItem[] items) {
			return Tools.MerkleTree.ComputeMerkleRoot(items, DefaultCHF);
		}

		protected abstract IDisposable CreateMerkleList(out IMerkleList<string> merkleList);

		[Test]
		public void CRUD_1() {
			using (CreateMerkleList(out var merkleList)) {
				merkleList.AddRange(new[] { "Alpha", "Beta", "Gamma" });
				merkleList.Remove("Beta");
				merkleList.RemoveAt(0);
				merkleList.Insert(0, "Beta");
				merkleList.Insert(0, "AlphaX");
				merkleList[0] = "Alpha";
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(ComputeRoot("Alpha", "Beta", "Gamma")));
			}
		}


		[Test]
		public void CRUD_2() {
			using (CreateMerkleList(out var merkleList)) {
				merkleList.AddRange(new[] { "Alpha", "Beta", "Gamma" });
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(ComputeRoot("Alpha", "Beta", "Gamma")));

				merkleList.Remove("Beta");
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(ComputeRoot("Alpha", "Gamma")));
				
				merkleList.RemoveAt(0);
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(ComputeRoot("Gamma")));

				merkleList.Insert(0, "Beta");
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(ComputeRoot("Beta", "Gamma")));

				merkleList.Insert(0, "AlphaX");
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(ComputeRoot("AlphaX", "Beta", "Gamma")));

				merkleList[0] = "Alpha";
				Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(ComputeRoot("Alpha", "Beta", "Gamma")));

			}
		}


		[Test]
		public void Insert_Empty() {
			using (CreateMerkleList(out var merkleList)) {
				Assert.That(merkleList.MerkleTree.Root, Is.Null);
				merkleList.InsertRange(0, Enumerable.Empty<string>());
				Assert.That(merkleList.MerkleTree.Root, Is.Null);
			}
		}


		[Test]
		public void IntegrationTests([Values(100)] int iterations) {
			var rng = new Random(31337);
			using (CreateMerkleList(out var merkleList)) {
				var expectedList = new ExtendedList<string>();
				AssertEx.ListIntegrationTest(
					merkleList,
					100,
					(rng, i) => Tools.Collection.GenerateArray<string>(i, _ => rng.NextString(100)),
					false,
					iterations,
					expectedList,
					() => Assert.That(merkleList.MerkleTree.Root, Is.EqualTo(Tools.MerkleTree.ComputeMerkleRoot(expectedList)))
				);
			}
		}


	}
}
