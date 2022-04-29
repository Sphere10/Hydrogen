//-----------------------------------------------------------------------
// <copyright file="NSComboBoxExtensions.cs" company="Sphere 10 Software">
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
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Collections.Generic;


namespace Hydrogen {
	public static class NSComboBoxExtensions
	{
		public static void SelectByItem<T>(this NSComboBox comboBox, T obj, Func<T,T,bool> comparer = null) {
			if (comparer == null) {
				comparer = (x,y) => new GenericComparer().Compare(x,y) == 0;
			}
			int? index = null;
			if (obj == null) {
				// unselect if selecting null
				index = -1;
			} else if (comboBox.DataSource is ComboBoxDataSourceEx) {
				var dataSourceEx = ((ComboBoxDataSourceEx)comboBox.DataSource);
				var data = dataSourceEx.Data;
				index = data.IndexOf(obj, (x,y) => comparer((T)x, (T)y));
				if (dataSourceEx.IncludeEmptyItem) {
					index++;
				}
			}
			if (index.HasValue) {
				if (index.Value != -1) {
					comboBox.SelectItem(index.Value);
				} else {
					if (comboBox.SelectedIndex != -1) {
						comboBox.DeselectItem(comboBox.SelectedIndex);
					}
					comboBox.StringValue = string.Empty;
				}
			}
		}

		public static T GetSelectedItem<T>(this NSComboBox comboBox)  {
			T retval = default(T);
			if (comboBox.SelectedIndex >= 0) {
				if (comboBox.DataSource is ComboBoxDataSourceEx) {
					var dataSourceEx = ((ComboBoxDataSourceEx)comboBox.DataSource);
					var data = dataSourceEx.Data;
					if (dataSourceEx.IncludeEmptyItem && comboBox.SelectedIndex == 0) {
						retval = default(T);
					} else {
						retval = (T)data.ElementAt(comboBox.SelectedIndex - (dataSourceEx.IncludeEmptyItem ? 1 : 0));
					}
				} 
			}
			return retval;
		}

		public static void SetDataSourceEx<T>(this NSComboBox comboBox, IEnumerable<T> dataSource, Func<T, string> getValue, bool includeEmptyRow = true) {
			comboBox.UsesDataSource = true;
			comboBox.DataSource = ComboBoxDataSourceEx.FromData( dataSource, getValue, includeEmptyRow);
			comboBox.ReloadData();
			comboBox.StringValue = string.Empty;
		}

		public static void ClearDataSource(this NSComboBox comboBox) {
			comboBox.UsesDataSource = true;
			comboBox.StringValue = string.Empty;
			comboBox.DataSource = CocoaTool.GenerateComboBoxDataSourceFromEnum<EmptyEnum>();
			comboBox.ReloadData();
		}

		public enum EmptyEnum {
		}
	}
}

