//-----------------------------------------------------------------------
// <copyright file="ComboBoxItem.cs" company="Sphere 10 Software">
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
using System.Collections;

namespace Hydrogen {
	[Register("ComboBoxItem")]
	public class ComboBoxItem : NSObject, IEqualityComparer {

		public ComboBoxItem(int key, string value)  {
			Key = key;
			Value = value;
		}

		public int Key { get; set; }
		
		public string Value { get; set; }
		
		public override string ToString() {
			return Value;
		}

		public override string Description {
			get {
				return base.Description;
			}
		}

		public override NSObject Copy() {
			return new ComboBoxItem(Key, Value);
		}

		public override NSObject MutableCopy() {
			return new ComboBoxItem(Key, Value);
		}

		public int GetHashCode(object obj) {
			return obj.GetHashCode();
		}
		
		public override int GetHashCode() {
	
				return string.Format("{0}{1}", Key, Value).GetHashCode();
		}
		
		public new bool Equals(object x, object y) {
			if (x is ComboBoxItem && y is ComboBoxItem) {
				var xItem = x as ComboBoxItem;
				var yItem = y as ComboBoxItem;
				return xItem.Key == yItem.Key && xItem.Value == yItem.Value;
			}
			return false;
		}
		
		public override bool Equals(object obj) {
			if (obj is ComboBoxItem) {
				return Equals(this, obj as ComboBoxItem);
			}
			return base.Equals(obj);
		}
	}
}

