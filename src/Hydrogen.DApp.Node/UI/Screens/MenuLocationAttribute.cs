// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.DApp.Node.UI;

public class MenuLocationAttribute : Attribute {

	public MenuLocationAttribute(AppMenu menu, string name, int preferredIndex = -1) {
		Menu = menu;
		Name = name;
		PreferredIndex = preferredIndex;
	}

	public AppMenu Menu { get; private set; }

	public string Name { get; private set; }

	public int? PreferredIndex { get; private set; }
}
