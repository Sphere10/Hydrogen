//-----------------------------------------------------------------------
// <copyright file="ActionComboBoxDataSource.cs" company="Sphere 10 Software">
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
using MonoMac.Foundation;
using MonoMac.AppKit;
using System.Collections.Generic;

namespace Hydrogen {
	
	// Inversion of implementation
	// Object inheritance
	// Plugification
	// Plug pattern
	
	public sealed class ActionComboBoxDataSource : NSComboBoxDataSource{
		
		public ActionComboBoxDataSource(
			Func<NSComboBox, string, string> completedString = null,
			Func<NSComboBox, string, int> indexOfItem = null,
			Func<NSComboBox, int> itemCount = null,
			Func<NSComboBox, int, NSObject>  objectValueForItem = null) {

			CompletedStringFunc = completedString;
			IndexOfItemFunc = indexOfItem;
			ItemCountFunc = itemCount;
			ObjectValueForItemFunc = objectValueForItem;

		}
		
		
		public Func<NSComboBox, string, string> CompletedStringFunc { get; private set; }
		
		public Func<NSComboBox, string, int> IndexOfItemFunc { get; private set; }
		
		public Func<NSComboBox, int> ItemCountFunc { get; private set; }
		
		public Func<NSComboBox, int, NSObject> ObjectValueForItemFunc { get; private set; }
		
		
		public override string CompletedString(NSComboBox comboBox, string uncompletedString) {
			if (CompletedStringFunc != null) {
				return CompletedStringFunc(comboBox, uncompletedString);
			}
			return uncompletedString;
		}

		
		public override int IndexOfItem(NSComboBox comboBox, string value) {
			if (IndexOfItemFunc != null) {
				return IndexOfItemFunc(comboBox, value);
			}		
			return default(int);
		}
		
		
		public override int ItemCount(NSComboBox comboBox) {
			if (ItemCountFunc != null) {
				return ItemCountFunc(comboBox);
			}
			return default(int);
		}
		
		public override NSObject ObjectValueForItem(NSComboBox comboBox, int index) {
			if (ObjectValueForItemFunc != null) {
				return ObjectValueForItemFunc(comboBox, index);
			}		
			return default(NSObject);
		}

	}
}

