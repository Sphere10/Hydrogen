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

namespace Sphere10.Framework.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class ArrayToolTests {

		[Test]
		public void Transpose_Null() {
			Assert.Throws<ArgumentNullException>(() => Tools.Array.Transpose<object>(null));
		}

		[Test]
		public void Transpose_PartiallyEmpty() {
			var input = new object[][] {
				new object[0],
				new object[0],
				new object[0],
			};
			Assert.Throws<ArgumentException>(() => Tools.Array.Transpose(input));
		}

		[Test]
		public void Transpose_Empty() {
			var empty = new object[0][];
			Assert.AreEqual(empty, Tools.Array.Transpose(empty)); 
        }

		[Test]
		public void Transpose_Case1() {
			var input = new object[][] {
				new object[] { 1, 2, 3 },
				new object[] { 4, 5, 6 },
			};
			var expected = new object[][] {
				new object[] { 1, 4 },
				new object[] { 2, 5 },
				new object[] { 3, 6 },
			};
			Assert.AreEqual(expected, Tools.Array.Transpose(input));
		}

		[Test]
		public void Transpose_Case2() {
			var expected = new object[][] {
				new object[] { 1, 2, 3 },
				new object[] { 4, 5, 6 },
			};
			var input = new object[][] {
				new object[] { 1, 4 },
				new object[] { 2, 5 },
				new object[] { 3, 6 },
			};
			Assert.AreEqual(expected, Tools.Array.Transpose(input));
		}

		[Test]
		public void Transpose_Complex([Values(100)] int iterations) {
			const int seed = 31337;
			var rng = new Random(seed);
			for (var i = 0; i < iterations; i++) {
				var input = rng.NextObjects(rng.Next(1,10), rng.Next(1,10));
				var output = Tools.Array.Transpose(Tools.Array.Transpose(input));
				Assert.AreEqual(input, output);
            }
		}
	}
}
