// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Drawing;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms.WinForms;

/// <summary>
/// Summary description for TransparentPanel.
/// </summary>
public class TransparentPanel : Panel {

	public event PaintEventHandler OnBackgroundPaint;

	public TransparentPanel() {
	}

	//This method makes sure that the background is what is directly behind the	control
	//and not what is behind the form...
	override protected CreateParams CreateParams {
		get {
			CreateParams cp = base.CreateParams;
			cp.ExStyle |= 0x20;
			return cp;
		}
	}

	protected void InvalidateEx() {
		if (Parent == null)
			return;
		Rectangle rc = new Rectangle(this.Location, this.Size);
		Parent.Invalidate(rc, true);

	}


	override protected void OnPaintBackground(PaintEventArgs e) {
		if (OnBackgroundPaint != null) {
			OnBackgroundPaint(this, e);
		}
	}

}
