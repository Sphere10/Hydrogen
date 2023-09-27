// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Linq;
using System.Windows.Forms;

namespace Hydrogen;

public static class ListBoxExtensions {


	/// <summary>
	/// Removes the selected items from a listbox.
	/// </summary>
	/// <param name="listBox">The listbox to clear selected items from.</param>
	public static void RemoveSelectedItems(this ListBox listBox) {
		object[] selectedItems = listBox.SelectedItems.Cast<object>().ToArray();
		foreach (object obj in selectedItems) {
			listBox.Items.Remove(obj);
		}
	}
}
