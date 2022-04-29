//-----------------------------------------------------------------------
// <copyright file="LoadingCircleTestForm.cs" company="Sphere 10 Software">
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester {
	public partial class LoadingCircleTestScreen : ApplicationScreen {
		public LoadingCircleTestScreen() {
			InitializeComponent();
		}

		private async void button1_Click(object sender, EventArgs e) {
		    using (LoadingCircle.EnterAnimationScope(_panel)) {
		        await Task.Delay(2000);
		    }
		}

		private async void button2_Click(object sender, EventArgs e) {
			loadingCircle1.StartAnimating();
            await Task.Delay(2000);
            loadingCircle1.StopAnimating();
            
        }
	}
}
