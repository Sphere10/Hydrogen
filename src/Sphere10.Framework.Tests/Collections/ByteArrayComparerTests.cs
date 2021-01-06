//-----------------------------------------------------------------------
// <copyright file="ByteArrayComparerTests.cs" company="Sphere 10 Software">
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
    public class ByteArrayComparerTests {

        [Test]
        public void TestNull() {
            Assert.AreEqual(0, ByteArrayComparer.Instance.Compare(null, null));
        }

        [Test]
        public void TestEmpty() {
            Assert.AreEqual(0, ByteArrayComparer.Instance.Compare(new byte[0], new byte[0]));
        }

        [Test]
        public void TestSame() {
            Assert.AreEqual(0, ByteArrayComparer.Instance.Compare(new byte[] {1,2 }, new byte[] { 1,2 }));
        }

        [Test]
        public void TestSmaller() {
            Assert.AreEqual(-1, ByteArrayComparer.Instance.Compare(new byte[] { 1, 2 }, new byte[] { 2, 1 }));
        }

        [Test]
        public void TestGreater() {
            Assert.AreEqual(1, ByteArrayComparer.Instance.Compare(new byte[] { 2, 1 }, new byte[] { 1, 2 }));
        }
    }

}
