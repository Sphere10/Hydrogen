//-----------------------------------------------------------------------
// <copyright file="UrlToolTests.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Sphere10.Framework.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class MiscellaneousTests {

		[Test]
		public void ToStringSafe() {
			Assert.AreEqual("<null>", ((object)null).ToStringSafe());
		}

		[Test]
		public void InterpolateNullAssumption() {
			Assert.AreEqual(string.Empty, $"{null}");
		}

		[Test]
		public void MemoryStreamSetLengthClearsOldBytes() {
			var rng = new Random(31337);
			for (var i=0; i < 1000; i++ ) {
				using var stream = new MemoryStream();
				var data = rng.NextBytes(i);
				stream.Write(data);
				for (var j = i; j >= 0; j--) {
					var bytes = stream.ToArray();
					stream.Position = rng.Next(0, (int)stream.Length);
					stream.SetLength(j);
					stream.SetLength(i);
					// j-i bytes should be 0
					Assert.That(stream.ToArray().AsSpan(^(i - j)).ToArray().All(b => b == 0));
					// reset stream
					stream.Position = 0;
					stream.Write(data);

				}
			}
		}

		[Test]
		public void StandardBehaviour_ListGetRangeNotSupportOverflow() {
			var list = new List<int>();
			list.AddRange(new[] { 1, 2, 3 });
			Assert.That(() => list.GetRange(1, 3), Throws.InstanceOf<ArgumentException>());
		}

		[Test]
		public void StandardBehaviour_ListRemoveRangeNotSupportOverflow() {
			var list = new List<int>();
			list.AddRange(new[] { 1, 2, 3 });
			Assert.That(() => list.RemoveRange(1, 3), Throws.InstanceOf<ArgumentException>());
		}

		[Test]
		public void StandardBehaviour_ListInsertRangeThrowsOnNull() {
			var list = new List<int>();
			Assert.That(() => list.InsertRange(0, null), Throws.InstanceOf<ArgumentException>());
		}
	}
}
