// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class RuntimeTests {


	[Test]
	public void InExceptionContext_True_1() {
		try {
			throw new System.Exception();
		} catch {
			Assert.That(Tools.Runtime.IsInExceptionContext(), Is.True);
		}
	}

	[Test]
	public void InExceptionContext_True_2() {
		try {
			try {
				throw new System.Exception();
			} finally {
				Assert.That(Tools.Runtime.IsInExceptionContext(), Is.True);
			}
		} catch {
		}
	}


	[Test]
	public void InExceptionContext_False_1() {
		Assert.That(Tools.Runtime.IsInExceptionContext(), Is.False);
	}

	[Test]
	public void InExceptionContext_False_2() {
		try {
		} catch {
		} finally {
			Assert.That(Tools.Runtime.IsInExceptionContext(), Is.False);
		}
	}
}
