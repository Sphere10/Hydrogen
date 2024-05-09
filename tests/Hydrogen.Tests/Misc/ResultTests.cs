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
public class ResultTests {


	[Test]
	public void ValueTypeCast_Bool() {
		ClassicAssert.IsTrue((bool)Result<bool>.From(true));
		ClassicAssert.IsFalse((bool)Result<bool>.From(false));
	}

	[Test]
	public void ValueTypeCast_Result_Bool() {
		ClassicAssert.AreEqual(Result<bool>.From(true), (Result<bool>)true);
		ClassicAssert.AreEqual(Result<bool>.From(false), (Result<bool>)false);
	}

	[Test]
	public void JsonSerialize_1() {
		Result expected = Result.Default;

		var json = Tools.Json.WriteToString(expected);
		var actual = Tools.Json.ReadFromString<Result<int>>(json);

		Assert.That(actual, Is.EqualTo(expected));
	}

	[Test]
	public void JsonSerialize_2() {
		Result expected = Result.Error("Hello", "World!");

		var json = Tools.Json.WriteToString(expected);
		var actual = Tools.Json.ReadFromString<Result<int>>(json);

		Assert.That(actual, Is.EqualTo(expected));
	}

	[Test]
	public void JsonSerialize_3() {
		Result<int> expected = Result<int>.Error("Hello", "World!");
		expected.Value = 11;

		var json = Tools.Json.WriteToString(expected);
		var actual = Tools.Json.ReadFromString<Result<int>>(json);

		Assert.That(actual, Is.EqualTo(expected));
	}


	[Test]
	public void JsonSerialize_4() {
		Result<int> inner = Result<int>.Error("Hello", "World!");
		inner.Value = 11;
		Result<Result<int>> expected = Result<Result<int>>.From(inner);
		expected.AddInfo("Info");

		var json = Tools.Json.WriteToString(expected);
		var actual = Tools.Json.ReadFromString<Result<Result<int>>>(json);

		Assert.That(actual, Is.EqualTo(expected));
	}
}
