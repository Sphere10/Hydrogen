//-----------------------------------------------------------------------
// <copyright file="ListBoxExtensions.cs" company="Sphere 10 Software">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hydrogen {

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
}
