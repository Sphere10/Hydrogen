//-----------------------------------------------------------------------
// <copyright file="ApproxEqual.cs" company="Sphere 10 Software">
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
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using System.IO;

namespace Hydrogen.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
    public class ApproxEqual {

        [Test]
        public void Exact() {
            var date = DateTime.Now;
            var test = date;
            Assert.IsTrue(date.ApproxEqual(test));
        }

        [Test]
        public void LessThanButWithinTolerance() {
            var date = DateTime.Now;
            var test = date.Subtract(TimeSpan.FromMilliseconds(100));
            Assert.IsTrue(date.ApproxEqual(test,TimeSpan.FromMilliseconds(250)));
        }

        [Test]
        public void LessThanButAtMaxTolerance() {
            var date = DateTime.Now;
            var test = date.Subtract(TimeSpan.FromMilliseconds(250));
            Assert.IsTrue(date.ApproxEqual(test, TimeSpan.FromMilliseconds(250)));
        }

        [Test]
        public void LessThanAndBeyondTolerance() {
            var date = DateTime.Now;
            var test = date.Subtract(TimeSpan.FromMilliseconds(251));
            Assert.IsFalse(date.ApproxEqual(test, TimeSpan.FromMilliseconds(250)));
        }
        [Test]
        public void GreaterThanButWithinTolerance() {
            var date = DateTime.Now;
            var test = date.Add(TimeSpan.FromMilliseconds(100));
            Assert.IsTrue(date.ApproxEqual(test, TimeSpan.FromMilliseconds(250)));
        }

        [Test]
        public void GreaterThanButAtMaxTolerance() {
            var date = DateTime.Now;
            var test = date.Add(TimeSpan.FromMilliseconds(250));
            Assert.IsTrue(date.ApproxEqual(test, TimeSpan.FromMilliseconds(250)));
        }

        [Test]
        public void GreaterThanAndBeyondTolerance() {
            var date = DateTime.Now;
            var test = date.Add(TimeSpan.FromMilliseconds(251));
            Assert.IsFalse(date.ApproxEqual(test, TimeSpan.FromMilliseconds(250)));
        }
    }
}
