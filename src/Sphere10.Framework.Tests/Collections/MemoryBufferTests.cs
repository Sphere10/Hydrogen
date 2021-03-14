//-----------------------------------------------------------------------
// <copyright file="LargeCollectionTests.cs" company="Sphere 10 Software">
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
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class MemoryBufferTests {


		[Test]
		public void IntegrationTests(
			[Values(0, 3, 111)] int startCapacity,
			[Values(1, 391)] int growCapacity,
			[Values(71, 2177)] int maxCapacity) {
			var RNG = new Random(startCapacity + growCapacity);
			var list = new MemoryBuffer(startCapacity, growCapacity, maxCapacity);
			var expected = new List<byte>();
			AssertEx.ListIntegrationTest<byte>(list, maxCapacity, (rng, i) => rng.NextBytes(i), mutateFromEndOnly: true);
		}
	}
}
