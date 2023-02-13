//-----------------------------------------------------------------------
// <copyright file="UrlToolTests.cs" company="Sphere 10 Software">
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

using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ResultTests {


	[Test]
	public void ValueTypeCast_Bool() {
		Assert.IsTrue((bool)Result<bool>.From(true));
		Assert.IsFalse((bool)Result<bool>.From(false));
	}

	[Test]
	public void ValueTypeCast_Result_Bool() {
		Assert.AreEqual(Result<bool>.From(true), (Result<bool>)true);
		Assert.AreEqual(Result<bool>.From(false), (Result<bool>)false);
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

