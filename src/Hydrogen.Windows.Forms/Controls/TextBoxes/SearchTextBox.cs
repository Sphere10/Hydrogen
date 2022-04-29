//-----------------------------------------------------------------------
// <copyright file="SearchTextBox.cs" company="Sphere 10 Software">
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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace Sphere10.Framework.Windows.Forms {

	[ToolboxBitmap(typeof(SearchTextBox), "SmallSearchIcon")]
	public class SearchTextBox : TextBoxEx {
		private readonly PictureBox _pictureBox;

	    public SearchTextBox() {
			base.PlaceHolderText = "Search";
			var bitmap = Resources.SmallSearchIcon;
			_pictureBox = new PictureBox { Cursor = Cursors.Default, Image = bitmap, Size = bitmap.Size, BackColor = Color.Transparent };
			Controls.Add(_pictureBox);
			PictureBox.SizeChanged += (o, e) => OnResize(e);
			FitPicture();
		}

		public PictureBox PictureBox {
			get {
				return _pictureBox;
			}
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			FitPicture();
		}

		private void FitPicture() {
			_pictureBox.Size = new Size(_pictureBox.Width, this.ClientSize.Height + 2);
			_pictureBox.Location = new Point(this.ClientSize.Width - _pictureBox.Width, -1);
			// Send EM_SETMARGINS to prevent text from disappearing underneath the button
			SendMessage(this.Handle, 0xd3, (IntPtr)2, (IntPtr)(_pictureBox.Width << 16));
		}

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

	}
}
