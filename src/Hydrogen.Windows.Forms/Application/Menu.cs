// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Drawing;

namespace Hydrogen.Windows.Forms;

public class Menu : IMenu {
	string _text;
	List<IMenuItem> _items;
	IApplicationBlock _parent;
	Image _image32x32;
	bool _showInMenuStrip;


	public Menu()
		: this(string.Empty) {
	}

	public Menu(string title)
		: this(title, null) {
	}


	public Menu(string title, IMenuItem[] items)
		: this(title, null, items) {

	}

	public Menu(string title, Image image32x32, IMenuItem[] items)
		: this(title, false, image32x32, items) {
	}

	public Menu(string title, bool showInMenuStrip, Image image32x32, IMenuItem[] items) {
		_parent = null;
		_text = title;
		_image32x32 = image32x32;
		_items = new List<IMenuItem>();
		_showInMenuStrip = showInMenuStrip;
		if (items != null) {
			foreach (IMenuItem item in items) {
				AddItem(item);
			}
		}
	}

	public bool ShowInMenuStrip {
		get { return _showInMenuStrip; }
		set { _showInMenuStrip = value; }
	}

	public virtual IApplicationBlock Parent {
		get { return _parent; }
		set { _parent = value; }
	}

	public virtual string Text {
		get { return _text; }
	}

	public virtual void AddItem(IMenuItem item) {
		if (item is ScreenMenuItem) {
			if (item.Parent == null) {
				item.Parent = this;
			}
		}
		_items.Add(item);
	}

	public virtual bool ContainsItem(IMenuItem item) {
		return _items.Contains(item);
	}

	public virtual void RemoveItem(IMenuItem item) {
		_items.Remove(item);
	}

	public virtual IMenuItem[] Items {
		get { return _items.ToArray(); }
	}

	public Image Image32x32 {
		get { return _image32x32; }
		set { _image32x32 = value; }
	}

	public virtual void Dispose() {
		_parent = null;
		foreach (IMenuItem menuItem in _items) {
			menuItem.Dispose();
		}
		if (_image32x32 != null) {
			_image32x32.Dispose();
		}
	}

}
