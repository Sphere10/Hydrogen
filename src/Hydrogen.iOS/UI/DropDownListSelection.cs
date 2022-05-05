//-----------------------------------------------------------------------
// <copyright file="DropDownListSelection.cs" company="Sphere 10 Software">
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

namespace Hydrogen.iOS {
	public class DropDownListSelection<T> {
		private T _selectedItem;

		public DropDownListSelection() {
			IsItemSelected = false;	
		}

		public bool IsItemSelected { get; private set; }

		public T SelectedItem { 
			get {
				if (!IsItemSelected)
					throw new Exception(string.Format("No item was selected"));
				return _selectedItem;
			}
			set {
				_selectedItem = value;
				IsItemSelected = true;
			}
		}
	}
}

