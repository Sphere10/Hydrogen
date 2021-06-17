//-----------------------------------------------------------------------
// <copyright file="MultiKeyDictionaryTest.cs" company="Sphere 10 Software">
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
using System.Text;
using NUnit.Framework;
using System.IO;
using Sphere10.Framework;


namespace Sphere10.Framework.Tests {

    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
    public class MultiKeyDictionaryTest {

        [Test]
        public void TestSimple() {
            var dict = new EnumerableKeyDictionary<string, int>();
            dict.Add(11, "1", "1");
            dict.Add(12, "1", "2");
            dict.Add(21, "2", "1");
            dict.Add(22, "2", "2");

            Assert.True(dict.ContainsKey("1", "1"));
            Assert.True(dict.ContainsKey("1", "2"));
            Assert.True(dict.ContainsKey("2", "1"));
            Assert.True(dict.ContainsKey("2", "2"));

            Assert.AreEqual(11, dict["1", "1"]);
            Assert.AreEqual(12, dict["1", "2"]);
            Assert.AreEqual(21, dict["2", "1"]);
            Assert.AreEqual(22, dict["2", "2"]);

        }

        [Test]
        public void LookupObjectKey_SameNumericTypeCode() {
            var dict = new EnumerableKeyDictionary<object, string>();
            dict.Add("magic", (int)1);
            Assert.True(dict.ContainsKey((int)1));
        }


        [Test]
        public void LookupObjectKey_VaryingNumericTypeCode_1() {
            var dict = new EnumerableKeyDictionary<object, string>();
            dict.Add("magic", (int)1);
            Assert.True(dict.ContainsKey((uint)1));
        }

        [Test]
        public void LookupObjectKey_VaryingNumericTypeCode_2() {
            var dict = new EnumerableKeyDictionary<object, string>();
            dict.Add("magic", (sbyte)1);
            Assert.True(dict.ContainsKey((ulong)1));
        }

        [Test]
        public void LookupObjectKey_VaryingNumericTypeCode_3() {
            var dict = new EnumerableKeyDictionary<object, string>();
            dict.Add("magic", (int)1);
            Assert.True(dict.ContainsKey((long)1));
        }

        [Test]
        public void LookupObjectKey_VaryingNumericTypeCode_4() {
            var dict = new EnumerableKeyDictionary<object, string>();
            dict.Add("magic", (float)1);
            Assert.True(dict.ContainsKey((double)1));
        }
    
    }

}
