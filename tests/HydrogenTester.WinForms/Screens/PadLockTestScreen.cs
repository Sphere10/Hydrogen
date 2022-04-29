//-----------------------------------------------------------------------
// <copyright file="PadLockTestForm.cs" company="Sphere 10 Software">
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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sphere10.Framework.Windows.Forms;


namespace Sphere10.FrameworkTester.WinForms {
	public partial class PadLockTestScreen : ApplicationScreen {
		private readonly TextWriter _outputTextWriter;
		public PadLockTestScreen() {
			InitializeComponent();
			_outputTextWriter = new TextBoxWriter(_outputTextBox);
		}



		private void padLockButton1_PadLockStateChanged(PadLockButton arg1, PadLockButton.PadLockState arg2) {
			_outputTextWriter.WriteLine("{0}: {1}", arg1.Name, arg2);
		}


	}
}
