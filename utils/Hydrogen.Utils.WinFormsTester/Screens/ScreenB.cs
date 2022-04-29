//-----------------------------------------------------------------------
// <copyright file="ScreenB.cs" company="Sphere 10 Software">
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
using System.Text;
using System.Windows.Forms;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester {
    public partial class ScreenB : ApplicationScreen {
        public ScreenB() {
            InitializeComponent();

	        listMerger1.LeftHeader = "Left Stuff";
            listMerger1.LeftItems = new object[] { "L1", "L2", "L3" };
            listMerger1.RightHeader = "Right Stuff";
            listMerger1.RightItems = new object[] { "R1", "R2", "R3" };
        }
    }
}

