// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;
using Hydrogen.DApp.Presentation.Loader.Plugins;

namespace Hydrogen.DApp.Presentation.Loader.Tests.PluginTests;

public class RoutingPathExtensionTests {
	[TestCase("/myapp/testing?test=1", ExpectedResult = "/myapp")]
	[TestCase("/", ExpectedResult = "/")]
	[TestCase("/myapp", ExpectedResult = "/myapp")]
	[TestCase("/myapp/", ExpectedResult = "/myapp")]
	[TestCase("/myapp/testing/test-page", ExpectedResult = "/myapp")]
	public string AppPathFromRelativePathTests(string input) {
		return input.ToAppPathFromBaseRelativePath();
	}

	[TestCase("/myapp/testing?test=1", ExpectedResult = "/myapp/testing")]
	[TestCase("/myapp/testing/abc?test=1&test2=2", ExpectedResult = "/myapp/testing/abc")]
	[TestCase("/myapp/", ExpectedResult = "/myapp/")]
	[TestCase("/myapp", ExpectedResult = "/myapp")]
	public string TrimQueryFromRelativePath(string input) {
		return input.TrimQueryParameters();
	}
}
