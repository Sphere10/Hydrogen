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

using NUnit.Framework;

namespace Hydrogen.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
    public class ResultTests {


        [Test]
        public void ValueTypeCast_Bool() {
            Assert.IsTrue((bool)Result<bool>.From(true));
            Assert.IsFalse((bool)Result<bool>.From(false));
        }

        [Test]
        public void ValueTypeCast_Result_Bool() {
            Assert.AreEqual(Result<bool>.From(true), (Result<bool>)true);
            Assert.AreEqual(Result<bool>.From(false), (Result<bool>)false);
        }
    }
}
