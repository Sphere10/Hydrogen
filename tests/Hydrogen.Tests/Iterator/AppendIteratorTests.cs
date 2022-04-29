//-----------------------------------------------------------------------
// <copyright file="AppendIteratorTests.cs" company="Sphere 10 Software">
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using NUnit.Framework.Constraints;
using Hydrogen;

namespace Hydrogen.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
    public class AppendIteratorTests {

        [Test]
        public void TestUnionAntiPattern() {
            var data = new[] {"one"};
            var union = data.Union("one");
            var result = union.ToArray();
            Assert.AreEqual(1, result.Length);
            Assert.AreEqual("one", result[0]);
        }

        [Test]
        public void TestConcat() {
            var data = new[] { "one" };
            var union = data.Concat("one");
            var result = union.ToArray();
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("one", result[0]);
            Assert.AreEqual("one", result[0]);
        }


    }

}
