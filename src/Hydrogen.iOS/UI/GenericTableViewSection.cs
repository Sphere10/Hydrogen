//-----------------------------------------------------------------------
// <copyright file="GenericTableViewSection.cs" company="Sphere 10 Software">
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
using Hydrogen;
using System.Linq;

namespace Hydrogen.iOS {

	public class GenericTableViewSection<T> {
		private bool _collapsed;

		public event EventHandlerEx<GenericTableViewSection<T>, bool, bool> CollapsedChanged;

		public GenericTableViewSection() : this(Enumerable.Empty<T>()) {
		}

		public GenericTableViewSection(string title, IEnumerable<T> items) : this(items, title) {
		}

		public GenericTableViewSection(IEnumerable<T> items, string title = null, string footer = null) : this(new ObservableList<T>(items), title, footer) {
			Items = new ObservableList<T>(items);
			Title = title;
			Footer = footer;
			_collapsed = false;
		}

		public GenericTableViewSection(ObservableList<T> items, string title = null, string footer = null) {
			Items = items;
			Title = title;
			Footer = footer;
			_collapsed = false;
		}

		public string Title;

		public string Footer;

		public ObservableList<T> Items;

		public bool Collapsed {
			get {
				return _collapsed;
			}
			set {
				var oldValue = _collapsed;
				_collapsed = value;
				if (_collapsed != value)
					CollapsedChanged(this, oldValue, _collapsed);
			}
		}

		protected void NotifyCollapsedChanged(bool oldValue, bool newValue) {
			if (CollapsedChanged != null)
				CollapsedChanged(this, oldValue, newValue);
		}
	}


}

