// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Hydrogen.DApp.Presentation.Plugins;
using NUnit.Framework.Legacy;
using Assert = NUnit.Framework.Assert;

namespace Hydrogen.DApp.Presentation.Loader.Tests.PluginTests;

public class MenuItemTests {
	[Test]
	public void MergeMenuItemsDuplicateRetainsOrig() {
		var menu1 = new List<MenuItem> {
			new("A",
				"/",
				new List<MenuItem> {
					new("B", "/", new List<MenuItem>())
				})
		};

		var menu2 = new List<MenuItem> {
			new("A",
				"/a",
				new List<MenuItem> {
					new("B", "/", new List<MenuItem>()),
					new("C", "/", new List<MenuItem>())
				}),
			new("AA",
				"/",
				new List<MenuItem> {
					new("B", "/", new List<MenuItem>())
				}),
		};

		List<MenuItem> merged = menu1.Merge(menu2).ToList();

		ClassicAssert.AreEqual(2, merged.Count);
		ClassicAssert.AreEqual(menu1[0].Heading, merged[0].Heading);
		ClassicAssert.AreEqual(menu1[0].Route, merged[0].Route);
		ClassicAssert.AreEqual(2, merged[0].Children.Count);
		ClassicAssert.AreEqual(1, merged[1].Children.Count);
	}

	[Test]
	public void CopyMenuItemsSameButDifRef() {
		var menu1 = new List<MenuItem> {
			new("A",
				"/",
				new List<MenuItem> {
					new("B", "/", new List<MenuItem>())
				})
		};

		var copy = menu1.Copy().ToList();

		ClassicAssert.AreNotSame(copy, menu1);
		ClassicAssert.IsTrue(menu1[0].Heading == copy[0].Heading);
		ClassicAssert.IsTrue(menu1[0].Children[0].Heading == copy[0].Children[0].Heading);
	}
}
