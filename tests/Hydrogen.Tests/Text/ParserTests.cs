﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using System;
using System.Net;

namespace Hydrogen.Tests;

[TestFixture]
public class ParserTests {

	[Test]
	public void Parse() {
		Assert.AreEqual(1, Tools.Parser.Parse<byte>("1"));
		Assert.AreEqual(1, Tools.Parser.Parse<int>("1"));
		Assert.AreEqual(1, Tools.Parser.Parse<long>("1"));
		Assert.AreEqual(1.0, Tools.Parser.Parse<float>("1"));
		var guid = Guid.NewGuid();
		Assert.AreEqual(guid, Tools.Parser.Parse<Guid>(guid.ToString()));
		Assert.AreEqual(guid, Tools.Parser.Parse<Guid>(guid.ToStrictAlphaString()));
	}

	[Test]
	public void Throws() {
		Assert.Throws<FormatException>(() => Tools.Parser.Parse<int?>("s"));
		Assert.Throws<FormatException>(() => Tools.Parser.Parse<int?>("null"));
	}

	[Test]
	public void Nullable() {
		Assert.IsNull(Tools.Parser.Parse<int?>(""));
		Assert.IsNull(Tools.Parser.Parse<int?>(null));
	}

	[Test]
	public void SafeParse() {
		//Assert.IsNull(Tools.Parser.SafeParseInt32("bad"));
		Assert.IsNull(Tools.Parser.SafeParse<int?>("bad"));
		Assert.AreEqual(default(int), Tools.Parser.SafeParse<int>("bad"));
	}


	[Test]
	public void IPAddr() {
		Assert.AreEqual(IPAddress.Parse("127.0.0.1"), Tools.Parser.Parse<IPAddress>("127.0.0.1"));
		Assert.Throws<FormatException>(() => Tools.Parser.Parse<IPAddress>("asdfhaskdfj"));
	}
}
