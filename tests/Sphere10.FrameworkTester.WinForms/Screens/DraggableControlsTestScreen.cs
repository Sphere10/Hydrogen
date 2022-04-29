//-----------------------------------------------------------------------
// <copyright file="DraggableControlsTestForm.cs" company="Sphere 10 Software">
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
using Sphere10.Framework;
using Sphere10.Framework.Windows.Forms;

namespace Sphere10.FrameworkTester.WinForms {
	public partial class DraggableControlsTestScreen : ApplicationScreen {
		public DraggableControlsTestScreen() {
			InitializeComponent();
			_pictureBox1.Draggable(true);
			_pictureBox2.Draggable(true);
			_pictureBox3.Draggable(true);
		}

        private void _pictureBox1_Click(object sender, EventArgs e) {
        }
	}
}
