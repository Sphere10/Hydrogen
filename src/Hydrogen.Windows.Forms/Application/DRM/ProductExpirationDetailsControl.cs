// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

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
