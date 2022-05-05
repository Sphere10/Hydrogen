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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using NUnit.Framework.Constraints;
using Hydrogen;
using Hydrogen.NUnit;

namespace Hydrogen.Tests {


    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
    public class ExtendedListTests {

        [Test]
        [Pairwise]
        public void IntegrationTests(
            [Values(0, 3,  111)] int startCapacity,
            [Values(19, 7, 13)] int growCapacity,
            [Values(10, 793, 2000)] int maxCapacity) {
            var list = new ExtendedList<int>(startCapacity, growCapacity, maxCapacity);
            AssertEx.ListIntegrationTest(list, list.MaxCapacity, (rng, i) => rng.NextInts(i));
        }
    }
}
