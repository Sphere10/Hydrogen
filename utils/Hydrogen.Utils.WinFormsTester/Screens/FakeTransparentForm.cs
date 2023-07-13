// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Hydrogen.Utils.WinFormsTester;

public partial class FakeTransparentForm : Form {
	public FakeTransparentForm() {
		SnappingBackground = true;
		this.Opacity = 0.996;
		InitializeComponent();
		FakeCopyBackground();
	}

	public bool SnappingBackground { get; set; }
	protected override void OnActivated(EventArgs e) {
		base.OnActivated(e);
		SnappingBackground = false;
	}
	protected override void OnResize(EventArgs e) {
		base.OnResize(e);
		SnapBackground();
	}

	protected override void OnMove(EventArgs e) {
		base.OnMove(e);
		SnapBackground();
	}

	private void SnapBackground() {
		if (!SnappingBackground) {
			SnappingBackground = true;
			Opacity = 0;
			FakeCopyBackground();
			Opacity = 0.996;
			SnappingBackground = false;
		}
	}

	private void FakeCopyBackground() {
		Rectangle rc = new Rectangle(this.PointToScreen(Point.Empty), this.Size);
		using (Bitmap bitmap = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb))
		using (Graphics g = Graphics.FromImage(bitmap)) {
			g.CopyFromScreen(rc.Left, rc.Top, 0, 0, rc.Size);
			if (BackgroundImage != null) {
				BackgroundImage.Dispose();
			}
			this.BackgroundImage = (Bitmap)bitmap.Clone();
			//foregroundControl.BackBitmap = bitmap;
		}
		this.Invalidate(true);
	}

	//	bool inDraw = false;
	protected override void OnPaintBackground(PaintEventArgs e) {
		base.OnPaintBackground(e);
		//	if (!inDraw) {
		//		try {
		//		inDraw = true;



		//using (Graphics myGraphics = this.CreateGraphics()) {
		//    Size s = this.Size;
		//    var memoryImage = new Bitmap(s.Width, s.Height, myGraphics);
		//    using (Graphics memoryGraphics = Graphics.FromImage(memoryImage)) {
		//        var loc = new Point(ClientRectangle.Location.X, ClientRectangle.Location.Y);
		//        memoryGraphics.CopyFromScreen(PointToScreen(loc), loc, s);
		//        //this.BackgroundImage = memoryImage;
		//        e.Graphics.DrawImage(memoryImage, new Point(0, 0));
		//    }
		//}
		//	Invalidate();
//				} finally {
		//				inDraw = false;
		//		}
		//}
	}

	private void _toggleBorderButton_Click(object sender, EventArgs e) {
		var style = this.FormBorderStyle;
		switch (style) {
			case FormBorderStyle.Fixed3D:
				this.FormBorderStyle = FormBorderStyle.FixedDialog;
				break;
			case FormBorderStyle.FixedDialog:
				this.FormBorderStyle = FormBorderStyle.FixedSingle;
				break;
			case FormBorderStyle.FixedSingle:
				this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
				break;
			case FormBorderStyle.FixedToolWindow:
				this.FormBorderStyle = FormBorderStyle.None;
				break;
			case FormBorderStyle.None:
				this.FormBorderStyle = FormBorderStyle.Sizable;
				break;
			case FormBorderStyle.Sizable:
				this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
				break;
			case FormBorderStyle.SizableToolWindow:
				this.FormBorderStyle = FormBorderStyle.Fixed3D;
				break;

		}

	}
}
