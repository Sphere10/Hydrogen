//-----------------------------------------------------------------------
// <copyright file="SchedulerTest.cs" company="Sphere 10 Software">
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
using System.Threading.Tasks;
using NUnit.Framework;
using System.Threading;
using System.Linq;
using System.IO;
using Hydrogen;
using Hydrogen.Data;


namespace Hydrogen.Tests {

	[TestFixture]
	[NonParallelizable]
	public class HydrogenJsonSerializerTests {



		[Test]
		public void NullSerializesNormal() {
			var serializer = new HydrogenJsonSerializer();
			Assert.That(serializer.Serialize<string>(null), Is.EqualTo("null"));
		}

	}
}
