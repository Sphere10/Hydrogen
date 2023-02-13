//-----------------------------------------------------------------------
// <copyright file="NagMessageControl.cs" company="Sphere 10 Software">
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
using Hydrogen;
using Hydrogen.Application;
using Hydrogen.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace Hydrogen.Windows.Forms;

public partial class ProductExpirationDetailsControl : ApplicationControl {
	public ProductExpirationDetailsControl() : base() {
		InitializeComponent();
	}

	public override string Text {
		get => _expirationNoticeLabel?.Text;
		set => _expirationNoticeLabel.Text = value;
	}

}

