// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Terminal.Gui;

namespace Hydrogen.DApp.Node.UI;

public static class Dialogs {


	public static bool SelectEnum<T>(string title, string description, T currentSelection, out T selection) where T : struct, Enum {
		var enumValues = Enum.GetValues<T>();
		var currIndex = Array.IndexOf(enumValues, currentSelection);
		Guard.Argument(currIndex >= 0, nameof(currentSelection), "Not a value of enumeration");
		var datasource = new ListDataSource<T>(enumValues, x => Enum.GetName(x), x => Tools.Enums.GetDescription(x));
		var dlg = new ListDialog<T>(title, description, datasource, currIndex);
		Terminal.Gui.Application.Run(dlg);
		if (!dlg.Cancelled) {
			selection = dlg.SelectedValue;
			return true;
		}
		selection = default;
		return false;
	}

	public static void Error(string title, string message) {
		MessageBox.ErrorQuery(title, message);
	}

	public static void Exception(Exception error) {
		Error("Error", error.ToDisplayString());
	}

}
