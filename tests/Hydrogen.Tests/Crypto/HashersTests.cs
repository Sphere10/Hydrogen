using System;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Sphere10.Framework;
using Sphere10.Framework.Maths;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class HashersTests {

		[Test]
		public void Aggregate_Empty() {
			Assert.Throws<ArgumentException>( () => Hashers.Aggregate(CHF.SHA2_256, Enumerable.Empty<byte[]>()));
		}

		[Test]
		public void Aggregate([Values(1, 2, 3, 100)] int n) {
			var rng = new Random(31337*(n+1));
			var items = Tools.Collection.Generate(() => rng.NextBytes(32)).Take(n).ToArray();

			var expected = items[0];
			foreach (var item in items.Skip(1))
				expected = Hashers.JoinHash(CHF.SHA2_256, expected, item);
			
			Assert.AreEqual(expected, Hashers.Aggregate(CHF.SHA2_256, items));
		}

	}
}
