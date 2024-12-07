// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Hamish Rose
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen.DApp.Presentation.Plugins;

/// <summary>
/// Menu item extensions
/// </summary>
public static class MenuItemExtensions {
	/// <summary>
	/// Merges a collection of menu items, producing a new collection that contains both merged. Original
	/// menu collection and items should be unchanged (Pure Function). If the first list contains an item with the same header, then it will be unchanged. Recursively merges
	/// children.
	/// </summary>
	/// <param name="items"></param>
	/// <param name="target"></param>
	/// <exception cref="ArgumentNullException"></exception>
	/// <returns> merged list</returns>
	public static IEnumerable<MenuItem> Merge(this IEnumerable<MenuItem> items, IEnumerable<MenuItem> target) {
		if (items == null) {
			throw new ArgumentNullException(nameof(items));
		}

		List<MenuItem> result = items.Copy().ToList();
		return MergeInner(result, target);

		IEnumerable<MenuItem> MergeInner(List<MenuItem> a, IEnumerable<MenuItem> b) {
			if (a == null)
				throw new ArgumentNullException(nameof(a));
			if (b == null)
				throw new ArgumentNullException(nameof(b));

			foreach (MenuItem menuItem in b) {
				MenuItem? match = a.FirstOrDefault(x => x.Heading == menuItem.Heading);

				if (match is null) {
					a.Add(menuItem);
				} else {
					MergeInner(match.Children, menuItem.Children);
				}
			}

			return a;
		}
	}

	/// <summary>
	/// Creates a deep copy of the menu.
	/// </summary>
	/// <param name="target"> to be copied</param>
	/// <returns> copied</returns>
	public static IEnumerable<MenuItem> Copy(this IEnumerable<MenuItem> target) {
		List<MenuItem> menu = new();

		foreach (MenuItem item in target) {
			menu.Add(new MenuItem(item.Heading, item.Route, item.Children.Copy().ToList(), item.IconPath));
		}

		return menu;
	}
}
