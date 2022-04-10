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
	public class StreamMappedMerkleListTests : MerkleListTestsBase {

		[Test]
		public void TestAdaptedScopes() {
			Assert.That(true, Is.False);
		}

		protected override IDisposable CreateMerkleList(out IMerkleList<string> merkleList) {
			var memStream = new MemoryStream();
			var clusteredList = new StreamMappedMerkleList<string>(memStream, 256, DefaultCHF);
			merkleList = clusteredList;
			return memStream;
		}

	}
}
