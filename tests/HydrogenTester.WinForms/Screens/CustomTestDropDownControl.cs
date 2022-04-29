//-----------------------------------------------------------------------
// <copyright file="CustomTestDropDownControl.cs" company="Sphere 10 Software">
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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sphere10.FrameworkTester.WinForms {
	public partial class UserControl1 : UserControl {
		public UserControl1() {
			InitializeComponent();
		}

		private void UserControl1_SizeChanged(object sender, EventArgs e) {
			// Calculate size of checked list box.
			checkedListBox1.Size = new Size(DisplayRectangle.Width - checkedListBox1.Left - 5,
											DisplayRectangle.Height - checkedListBox1.Top - button1.Height - 10);
		}
	}
}
