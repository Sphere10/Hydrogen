//-----------------------------------------------------------------------
// <copyright file="TextBoxWriter.cs" company="Sphere 10 Software">
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
using Hydrogen;

namespace Hydrogen.Windows.Forms;
public class TextBoxWriter : SyncTextWriter {
	private readonly TextBox _textBox;

	public TextBoxWriter(TextBox textBox) {
		_textBox = textBox;
	}

	protected override void InternalWrite(string value) {
		_textBox.InvokeEx(() => {
			if (!_textBox.IsDisposed)
				_textBox.AppendText(value);
		});
	}
}

