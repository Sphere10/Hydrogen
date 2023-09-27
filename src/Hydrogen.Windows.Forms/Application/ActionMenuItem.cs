// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;

namespace Hydrogen.Windows.Forms;

public class ActionMenuItem : MenuItem, ILinkMenuItem {
	private Action _selectAction;

	public ActionMenuItem(Action onClick)
		: this(string.Empty, onClick) {
	}

	public ActionMenuItem(string text, Action select) {
		Guard.ArgumentNotNull(select, nameof(select));
		Text = text;
		_selectAction = select;
	}

	public ActionMenuItem(string text, Image image16x16, Action OnClick)
		: this(text, image16x16, true, true, false) {
	}

	public ActionMenuItem(
		string text,
		Image image16x16,
		bool showOnExplorerBar = true,
		bool showOnToolBar = true,
		bool executeOnLoad = false
	) : base(image16x16, showOnExplorerBar, showOnToolBar, executeOnLoad) {
		Text = text;
	}


	public virtual string Text { get; set; }

	public virtual void OnSelect() {
		_selectAction();
	}

	public override void Dispose() {
		base.Dispose();
	}

}
