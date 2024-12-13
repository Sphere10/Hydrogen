// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using Hydrogen.ObjectSpaces;
using NUnit.Framework;
using Hydrogen;

namespace Hydrogen.Tests.ObjectSpaces;

[TestFixture]
public class MemoryMappedTests {

	#region Load

	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.MemoryMappedTestCases))]
	public void LoadEmptyDoesntThrow(TestTraits testTraits) {
		using var stream = new MemoryStream();
		var activationArgs = new Dictionary<string, object> { ["stream"] = stream };

		using (var objectSpace = TestsHelper.CreateObjectSpace(testTraits, activationArgs)) {
			objectSpace.Flush();
		}
		Assert.That(() => { using var _ = TestsHelper.CreateObjectSpace(testTraits, activationArgs); }, Throws.Nothing);
	}

	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.MemoryMappedTestCases))]
	public void LoadDoesntThrow(TestTraits testTraits) {
		var stream = new MemoryStream();
		var activationArgs = new Dictionary<string, object> { ["stream"] = stream };

		using (var objectSpace =  TestsHelper.CreateObjectSpace(testTraits, activationArgs)) {
			var savedAccount = TestsHelper.CreateAccount();
			objectSpace.Save(savedAccount);
			objectSpace.Flush();
		}
		Assert.That(() => { using var _ = TestsHelper.CreateObjectSpace(testTraits, activationArgs); }, Throws.Nothing);
	}

	#endregion

}
