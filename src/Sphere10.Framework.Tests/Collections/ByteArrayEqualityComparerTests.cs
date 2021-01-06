//-----------------------------------------------------------------------
// <copyright file="ByteArrayEqualityComparerTests.cs" company="Sphere 10 Software">
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
using Sphere10.Framework;


namespace Sphere10.Framework.UnitTests {

    [TestFixture]
    public class ByteArrayEqualityComparerTests {

        [Test]
        public void TestNull() {
            Assert.AreEqual(true, ByteArrayEqualityComparer.Instance.Equals(null, null));
        }

        [Test]
        public void TestEmpty() {
            Assert.AreEqual(true, ByteArrayEqualityComparer.Instance.Equals(new byte[0], new byte[0]));
        }

        [Test]
        public void TestSame() {
            Assert.AreEqual(true, ByteArrayEqualityComparer.Instance.Equals(new byte[] {1,2 }, new byte[] { 1,2 }));
        }

        [Test]
        public void TestDiff() {
            Assert.AreEqual(false, ByteArrayEqualityComparer.Instance.Equals(new byte[] { 1, 2 }, new byte[] { 2, 1 }));
        }

        [Test]
        public void TestDiffLonger_1() {
            Assert.AreEqual(false, ByteArrayEqualityComparer.Instance.Equals(new byte[] { 1, 2,3 }, new byte[] { 2, 1 }));
        }

        [Test]
        public void TestDiffLonger_2() {
            Assert.AreEqual(false, ByteArrayEqualityComparer.Instance.Equals(new byte[] { 1, 2 }, new byte[] { 2, 1, 3 }));
        }

    }

}
