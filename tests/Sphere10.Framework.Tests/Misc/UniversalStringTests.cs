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
	}
}
