//-----------------------------------------------------------------------
// <copyright file="TextBoxExtensions.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hydrogen {

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
}
