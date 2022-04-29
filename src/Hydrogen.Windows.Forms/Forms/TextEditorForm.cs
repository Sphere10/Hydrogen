//-----------------------------------------------------------------------
// <copyright file="TextEditorForm.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms {
	public partial class TextEditorForm : Form {
		public TextEditorForm() {
			InitializeComponent();
		}


		public TextEditorForm(string text) {
			InitializeComponent();
			_textBox.Text = text;
		}

	}
}
