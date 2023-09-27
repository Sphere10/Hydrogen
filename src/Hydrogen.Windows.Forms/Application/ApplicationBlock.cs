// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Drawing;

namespace Hydrogen.Windows.Forms;

public class ApplicationBlock : IApplicationBlock {
	private string _name;
	private List<IMenu> _menus;
	private Image _image32x32;
	private Image _image8x8;
	private string _helpFile;
	private bool _showInMenuStrip;
	private bool _showInToolStrip;


	public ApplicationBlock()
		: this(string.Empty, null, null, null) {
	}

	public ApplicationBlock(string title, Image image32x32, Image image8x8, string helpFile)
		: this(title, image32x32, image8x8, helpFile, null) {
	}

	public ApplicationBlock(string title, Image image32x32, Image image8x8, string helpFile, Menu[] menus)
		: this(title, false, false, image32x32, image8x8, helpFile, menus) {
	}

	public ApplicationBlock(string title, bool showInToolStrip, bool showInMenuStrip, Image image32x32, Image image8x8, string helpFile, Menu[] menus) {
		_showInMenuStrip = showInMenuStrip;
		_showInToolStrip = showInToolStrip;
		_helpFile = helpFile;
		_name = title;
		_image32x32 = image32x32;
		_image8x8 = image8x8;
		_menus = new List<IMenu>();
		if (menus != null) {
			foreach (Menu menu in menus) {
				AddMenu(menu);
			}
		}
		Position = 0;
	}

	public bool ShowInToolStrip {
		get { return _showInToolStrip; }
		set { _showInToolStrip = value; }
	}

	public virtual bool ShowInMenuStrip {
		get { return _showInMenuStrip; }
		set { _showInMenuStrip = value; }
	}

	public int Position { get; }

	public virtual string Name {
		get { return _name; }
		set { _name = value; }
	}

	public virtual Image Image32x32 {
		get { return _image32x32; }
		set { _image32x32 = value; }
	}

	public Image Image8x8 {
		get { return _image8x8; }
		set { _image8x8 = value; }
	}

	public virtual IMenu[] Menus {
		get { return _menus.ToArray(); }
	}

	public string HelpFileCHM {
		get { return _helpFile; }
		set { _helpFile = value; }
	}

	public Type DefaultScreen { get; init; }

	public virtual void AddMenu(IMenu menu) {
		menu.Parent = this;
		_menus.Add(menu);
	}

	public virtual bool ContainsMenu(IMenu menu) {
		return _menus.Contains(menu);
	}

	public virtual void RemoveMenu(IMenu menu) {
		_menus.Remove(menu);
	}

	public virtual void Dispose() {
		foreach (IMenu menu in _menus) {
			menu.Dispose();
		}
		if (_image8x8 != null) {
			_image8x8.Dispose();
		}
		if (_image32x32 != null) {
			_image32x32.Dispose();
		}
	}

}
