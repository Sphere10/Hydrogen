// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Windows.Forms;

namespace Hydrogen;

public static class ComboBoxExtensions {

	/// <summary>
	/// Selects the item in the combo box which matches the given text.
	/// </summary>
	/// <param name="combo">Combo box to select from.</param>
	/// <param name="itemText">Text to match item against.</param>
	/// <remarks>Only works if DataSource is a collection of strings.</remarks>
	/// <returns>Whether or not such an item was selected.</returns>
	public static bool TrySelectByText(this ComboBox combo, string itemText) {
		for (int i = 0; i < combo.Items.Count; i++) {
			if (combo.GetItemText(combo.Items[i]) == itemText) {
				combo.SelectedIndex = i;
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Selects the item in the combo box which matches the given text or sets the text property if it doesn't exist.
	/// </summary>
	/// <remarks>Only works if DataSource is a collection of strings.</remarks>
	/// <param name="combo">Combo box to select from.</param>
	/// <param name="itemText">Text to match item against.</param>
	public static void TrySelectOrSet(this ComboBox combo, string itemText) {
		if (!combo.TrySelectByText(itemText)) {
			combo.Items.Add(itemText);
			TrySelectByText(combo, itemText);
		}
	}
}
