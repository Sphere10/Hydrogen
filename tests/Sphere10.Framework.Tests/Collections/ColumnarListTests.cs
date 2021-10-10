//-----------------------------------------------------------------------
// <copyright file="LargebinaryFileTests.cs" company="Sphere 10 Software">
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
using System.Linq;
using NUnit.Framework;
using System.IO;
using Sphere10.Framework.NUnit;
using System.Collections;

namespace Sphere10.Framework.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class ColumnarListTests {


		[Test]
		public void Basic() {
			const int seed = 31337;
			var rng = new Random(seed);

			var columnarList = new ColumnarList(new[] {
				new ExtendedList<object>(),  // col 1
				new ExtendedList<object>(),  // col 2
				new ExtendedList<object>(),  // col 3
			});

			var item1 = new object[] { 1, 2, 3 };
			var item2 = new object[] { 4, 5, 6 };
			var item3 = new object[] { 7, 8, 9 };
			var items = new object[][] {
				item1,
				item2,
				item3
			};

			columnarList.AddRange(items);

			Assert.AreEqual(3, columnarList.Count);
			Assert.AreEqual(item1, columnarList[0]);
			Assert.AreEqual(item2, columnarList[1]);
			Assert.AreEqual(item3, columnarList[2]);
		}

		[Test]
		[Pairwise]
		public void IntegrationTests(
			[Values(1, 4, 11)] int columns,
			[Values(0, 3, 111)] int startCapacity,
			[Values(19, 7, 13)] int growCapacity,
			[Values(10, 793, 2000)] int maxCapacity) {
			var list = new ColumnarList(Tools.Collection.Generate(() => new ExtendedList<object>()).Take(columns).ToArray());
			AssertEx.ListIntegrationTest(
				list,
				maxCapacity,
				(rng, i) => rng.NextObjects(columns, i),
				comparer: new ActionEqualityComparer<object[]>((x, y) => ((IStructuralEquatable)x).Equals(y, EqualityComparer<string>.Default))
			);
		}

	}
}
