// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Terminal.Gui;

namespace Hydrogen.DApp.Node.UI;

public class TextViewWriter : SyncTextWriter {
	private readonly TextView _textBox;

	public TextViewWriter(TextView textBox) {
		_textBox = textBox;
	}

	protected override void InternalWrite(string value) {
		_textBox.Text += value;
		_textBox.SetNeedsDisplay();
	}
}
