// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;

namespace Hydrogen.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
    public class StringTests {


        [Test]
        public void TrimStart_1() {
            Assert.AreEqual("", "alpha".TrimStart("alpha"));
        }

        [Test]
        public void TrimStart_2() {
            Assert.AreEqual("1", "alpha1".TrimStart("alpha"));
        }

        [Test]
        public void TrimStart_3() {
            Assert.AreEqual("1alpha2", "1alpha2".TrimStart("alpha"));
        }


        [Test]
        public void TrimStart_4() {
            Assert.AreEqual("", "aLphA".TrimStart("alpha", false));
        }

        [Test]
        public void TrimStart_5() {
            Assert.AreEqual("1", "AlpHa1".TrimStart("alpha", false));
        }

        [Test]
        public void TrimStart_6() {
            Assert.AreEqual("1aLPha2", "1aLPha2".TrimStart("alpha", false));
        }


        [Test]
        public void TrimEnd_1() {
            Assert.AreEqual("", "alpha".TrimEnd("alpha"));
        }

        [Test]
        public void TrimEnd_2() {
            Assert.AreEqual("1", "1alpha".TrimEnd("alpha"));
        }

        [Test]
        public void TrimEnd_3() {
            Assert.AreEqual("1aLpha2", "1aLpha2".TrimEnd("alpha"));
        }

        [Test]
        public void TrimEnd_4() {
            Assert.AreEqual("", "alpHa".TrimEnd("alpha", false));
        }

        [Test]
        public void TrimEnd_5() {
            Assert.AreEqual("1", "1AlphA".TrimEnd("alpha", false));
        }

        [Test]
        public void TrimEnd_6() {
            Assert.AreEqual("1alpHa2", "1alpHa2".TrimEnd("alpha", false));
        }
    }

}
