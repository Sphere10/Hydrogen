//-----------------------------------------------------------------------
// <copyright file="ComboBoxDataSourceEx.cs" company="Sphere 10 Software">
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
using MonoMac.AppKit;
using System.Collections.Generic;
using MonoMac.Foundation;


namespace Hydrogen {


	public class ComboBoxDataSourceEx : NSComboBoxDataSource {


		// Called when created from unmanaged code
		public ComboBoxDataSourceEx(IntPtr handle) : base (handle) {
			var x = 1;
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ComboBoxDataSourceEx(NSCoder coder) : base (coder) {
		}


		private ComboBoxDataSourceEx(IEnumerable<object> data, Func<object, string> getValue, bool includeEmptyItem = true) : base() {
			IncludeEmptyItem = includeEmptyItem;
			Data = data;
			GetValueFunc = getValue;
		}

		public bool IncludeEmptyItem { get; set; }

		public static ComboBoxDataSourceEx FromData<T>(IEnumerable<T> data, Func<T, string> getValue, bool includeEmptyRows = true) {
			return new ComboBoxDataSourceEx(data,(o) => getValue((T)o), includeEmptyRows);

		}

		public sealed override string CompletedString(NSComboBox comboBox, string uncompletedString) {
			var retval = uncompletedString;
			var match = Data.FirstOrDefault(datum => GetValueFunc(datum).ToUpper().StartsWith(uncompletedString.ToUpper()));
			if (match != null) {
				retval = GetValueFunc(match);
			}
			return retval;
		}
		
		public sealed override int IndexOfItem(NSComboBox comboBox, string value) {
			return !string.IsNullOrWhiteSpace(value) ? Data.IndexOf(v => GetValueFunc(v) == value) + (IncludeEmptyItem ? 1 : 0)  : -1;
		}	

		public sealed override int ItemCount(NSComboBox comboBox) {
			return Data.Count() + (IncludeEmptyItem ? 1 : 0);
		}

		public sealed override NSObject ObjectValueForItem(NSComboBox comboBox, int index) {
			try {
				if (0 <= index && index <= (Data.Count() + (IncludeEmptyItem ? 1 : 0))) {
					if (IncludeEmptyItem && index == 0) {
						return string.Empty.ToNSString();
					}
					return GetValueFunc(Data.ElementAt(index - (IncludeEmptyItem ? 1 : 0) )).ToNSString();
				} 
				return "Internal error".ToNSString();
			} catch (Exception error) {
				SystemLog.Error(error.ToDiagnosticString());
				return null;
			}

		}

		public Func<object, string> GetValueFunc { get; protected set; }
		public IEnumerable<object> Data { get; protected set; }

	}
}

