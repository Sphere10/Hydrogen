// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;

namespace Hydrogen;

public static class TextBoxExtensions {

	public static void FocusAtEnd(this TextBox textBox) {
		textBox.Select(textBox.Text.Length, 0);
		textBox.Focus();
	}

	public static void AppendText(this TextBox textBox, string text) {
		textBox.Text += text;
		textBox.FocusAtEnd();
	}

	public static void AppendLine(this TextBox textBox, string text) {
		textBox.AppendText(text + Environment.NewLine);
	}
}
