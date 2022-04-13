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
	public class MerkleListAdapterTests : MerkleListTestsBase {

		protected override IDisposable CreateMerkleList(CHF chf, out IMerkleList<string> merkleList) {
			merkleList = new MerkleListAdapter<string>(chf);
			return Disposables.None;
		}

		[Test]
		public void IntegrationTests_Heavy([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, [Values(100)] int iterations) {
			var rng = new Random(31337);
			using (CreateMerkleList(chf, out var merkleList)) {
				var expectedList = new ExtendedList<string>();
				AssertEx.ListIntegrationTest(
					merkleList,
					1000,
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
