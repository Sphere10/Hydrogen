// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

[ToolboxBitmap(typeof(TextBox))]
public class TextBoxEx : TextBox {
	private PlaceHolderTextExtender _placeHolderTextExtender;
	private System.ComponentModel.IContainer components;

	public TextBoxEx() {
		InitializeComponent();
	}

	[Category("Appearance")]
	[DefaultValue("")]
	public string PlaceHolderText {
		get { return _placeHolderTextExtender.GetPlaceHolderText(this); }
		set { _placeHolderTextExtender.SetPlaceHolderText(this, value); }
	}


	private void InitializeComponent() {
		this.components = new System.ComponentModel.Container();
		this._placeHolderTextExtender = new Hydrogen.Windows.Forms.PlaceHolderTextExtender(this.components);
		this.SuspendLayout();
		// 
		// TextBoxEx
		// 
		this._placeHolderTextExtender.SetPlaceHolderText(this, "");
		this.ResumeLayout(false);
	}
}
