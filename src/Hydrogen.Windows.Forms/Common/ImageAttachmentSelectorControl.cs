// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public partial class ImageAttachmentSelectorControl : UserControl {

	public ImageAttachmentSelectorControl() {
		InitializeComponent();
	}

	[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool HasSelectedImage {
		get { return !_noneRadioButton.Checked; }
	}

	public bool TryGetSelectedImage(out Image selectedImage) {
		bool retval = false;
		selectedImage = null;
		if (HasSelectedImage) {
			switch (AttachmentSource) {
				case ImageAttachmentSource.Clipboard:
					if (Clipboard.ContainsImage()) {
						selectedImage = Clipboard.GetImage();
						retval = true;
					}
					break;
				case ImageAttachmentSource.File:
					try {
						selectedImage = Image.FromFile(Filename);
						retval = true;
					} catch {
					}
					break;
				default:
					break;
			}
		}
		return retval;
	}

	[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string Filename {
		get { return _fileSelectorControl.Path; }
	}

	[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public ImageAttachmentSource AttachmentSource {
		get {
			ImageAttachmentSource retval = ImageAttachmentSource.None;
			if (_filenameRadioButton.Checked) {
				ImageAttachmentSource source = ImageAttachmentSource.File;
			} else if (_clipboardRadioButton.Checked) {
				ImageAttachmentSource source = ImageAttachmentSource.Clipboard;
			} else if (_noneRadioButton.Checked) {
				ImageAttachmentSource source = ImageAttachmentSource.None;
			}
			return retval;
		}
		set {
			switch (value) {
				case ImageAttachmentSource.Clipboard:
					_clipboardRadioButton.Checked = true;
					break;
				case ImageAttachmentSource.File:
					_filenameRadioButton.Checked = true;
					break;
				case ImageAttachmentSource.None:
					_noneRadioButton.Checked = true;
					break;
			}
		}
	}


	private void UpdateImageBox() {
		Image img = null;
		if (TryGetSelectedImage(out img)) {
			_pictureBox.Image = img;
		}
	}

	private void _filenameRadioButton_CheckedChanged(object sender, EventArgs e) {
		_fileSelectorControl.Enabled = _filenameRadioButton.Checked;
		if (_filenameRadioButton.Checked) {
			UpdateImageBox();
		}
	}

	private void _clipboardRadioButton_CheckedChanged(object sender, EventArgs e) {
		if (_clipboardRadioButton.Checked) {
			UpdateImageBox();
		}
	}

	private void _noneRadioButton_CheckedChanged(object sender, EventArgs e) {
		if (_noneRadioButton.Checked) {
			UpdateImageBox();
		}
	}

	#region Inner classes/enums

	public enum ImageAttachmentSource {
		File,
		Clipboard,
		None
	}

	#endregion

}
