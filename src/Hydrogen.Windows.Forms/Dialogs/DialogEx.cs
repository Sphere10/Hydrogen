// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public partial class DialogEx : FormEx {

	public const int MaxTextLength = 5000;
	
	public DialogEx()
		: this(SystemIconType.None, string.Empty, "OK", true) {
	}

	protected DialogEx(SystemIconType iconType, string title, string text, bool enableApplyAll, params string[] buttonNames) {
		if (title == null)
			throw new ArgumentNullException("title");
		if (text == null)
			throw new ArgumentNullException("text");
		if (buttonNames.Length == 0)
			buttonNames = new[] { "OK" };

		if (buttonNames.Length > 5)
			throw new ArgumentOutOfRangeException("buttonNames", "Cannot have more than 5 buttons");

		// clip the text if its too large
		if (text.Length > MaxTextLength)
			text = text.Substring(0, MaxTextLength);

		InitializeComponent();

		// set color
		//this._textLabelPanel.BackColor = Color.Cyan;
		//this._textLabel.BackColor = Color.Blue;

		_alwaysCheckBox.Visible = enableApplyAll;

		this.Text = title;


		// determine size of form based on label/text
		if (!_textLabel.CanTextFit(text)) {
			var oldSize = _textLabel.Size;
			var newSize = _textLabel.FitSize(text, oldSize.Width);

			// we resize the form, not the label since the label is anchored to the form
			var heightDiff = newSize.Height - oldSize.Height;
			_textLabel.Height += (int)Math.Round(heightDiff * AutoScaleFactor.Height);
			_textLabelPanel.VerticalScroll.Visible = true;
		}
		_textLabel.Text = text;

		for (var i = 0; i < buttonNames.Length; i++) {
			switch (i) {
				case 0:
					button1.Visible = true;
					button1.Text = buttonNames[i];
					break;
				case 1:
					button2.Visible = true;
					button2.Text = buttonNames[i];
					break;
				case 2:
					button3.Visible = true;
					button3.Text = buttonNames[i];
					break;
				case 3:
					button4.Visible = true;
					button4.Text = buttonNames[i];
					break;
			}
		}

		for (var i = buttonNames.Length; i < 4; i++) {
			switch (i) {
				case 0:
					button1.Visible = false;
					break;
				case 1:
					button2.Visible = false;
					break;
				case 2:
					button3.Visible = false;
					break;
				case 3:
					button4.Visible = false;
					break;
			}
		}

		// choose icon
		if (iconType == SystemIconType.None) {
			_pictureBoxEx.Visible = false;
			var oldLocation = _textLabelPanel.Location;
			_textLabelPanel.Location = _pictureBoxEx.Location;
			var widthGain = oldLocation.X - _textLabelPanel.Location.X;
			_textLabelPanel.Width += widthGain;
		}
		_pictureBoxEx.SystemIcon = iconType;
	}

	public bool AlwaysFlag { get; private set; }

	public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icons)
		=> Show(SystemIconType.None, text, caption, buttons, icons);

	public static DialogResult Show(SystemIconType iconType, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icons)
		=> Show(null, iconType, text, caption, buttons, icons);

	public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icons)
		=> Show(owner, SystemIconType.Information, text, caption, buttons, icons);

	public static DialogResult Show(IWin32Window owner, SystemIconType iconType, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icons) {
		//MessageBox.Show(owner, text, caption, buttons, icons);
		return ToDialogResult(buttons, Show(owner, iconType, caption, text, ToButtonNames(buttons)));
	}

	public static DialogExResult Show(SystemIconType iconType, string title, string text, params string[] buttonNames) {
		var dialog = new DialogEx(iconType, title, text, false, buttonNames);
		dialog.ShowDialog();
		return dialog.DialogResult;
	}

	public static DialogExResult Show(IWin32Window owner, SystemIconType iconType, string title, string text, params string[] buttonNames) {
		var dialog = new DialogEx(iconType, title, text, false, buttonNames);
		dialog.StartPosition = FormStartPosition.CenterParent;
		dialog.ShowDialog(owner);
		return dialog.DialogResult;
	}

	public new DialogExResult DialogResult { get; private set; }

	protected virtual void OnProcessButton(DialogExResult button) {
		CloseWithResult(button);
	}

	protected void CloseWithResult(DialogExResult result) {
		DialogResult = result;
		Close();
	}

	private void button1_Click(object sender, EventArgs e) {
		try {
			OnProcessButton(DialogExResult.Button1);
		} catch (Exception error) {
			//
		}
	}

	private void button2_Click(object sender, EventArgs e) {
		try {
			OnProcessButton(DialogExResult.Button2);
		} catch (Exception error) {
			//
		}

	}

	private void button3_Click(object sender, EventArgs e) {
		try {
			OnProcessButton(DialogExResult.Button3);
		} catch (Exception error) {
			//
		}

	}

	private void button4_Click(object sender, EventArgs e) {
		try {
			OnProcessButton(DialogExResult.Button4);
		} catch (Exception error) {
			//
		}

	}

	public static string[] ToButtonNames(MessageBoxButtons buttons) {
		return buttons switch {
			MessageBoxButtons.OK => new[] {
				"OK"
			},
			MessageBoxButtons.OKCancel => new[] {
				"OK", "Cancel"
			}.ReverseArray(),
			MessageBoxButtons.AbortRetryIgnore => new[] {
				"Abort", "Retry", "Ignore"
			}.ReverseArray(),
			MessageBoxButtons.YesNoCancel => new[] {
				"Yes", "No", "Cancel"
			}.ReverseArray(),
			MessageBoxButtons.YesNo => new[] {
				"Yes", "No"
			}.ReverseArray(),
			MessageBoxButtons.RetryCancel => new[] {
				"Retry", "Cancel"
			}.ReverseArray(),
			_ => throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null)
		};
	}

	public static DialogResult ToDialogResult(MessageBoxButtons buttons, DialogExResult result) {
		switch (result) {
			case DialogExResult.Button1:
				switch (buttons) {
					case MessageBoxButtons.OK:
						return System.Windows.Forms.DialogResult.OK;
					case MessageBoxButtons.OKCancel:
						return System.Windows.Forms.DialogResult.Cancel;
					case MessageBoxButtons.AbortRetryIgnore:
						return System.Windows.Forms.DialogResult.Ignore;
					case MessageBoxButtons.YesNoCancel:
						return System.Windows.Forms.DialogResult.Cancel;
					case MessageBoxButtons.YesNo:
						return System.Windows.Forms.DialogResult.No;
					case MessageBoxButtons.RetryCancel:
						return System.Windows.Forms.DialogResult.Cancel;
					default:
						throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null);
				}
			case DialogExResult.Button2:
				switch (buttons) {
					case MessageBoxButtons.OKCancel:
						return System.Windows.Forms.DialogResult.OK;
					case MessageBoxButtons.AbortRetryIgnore:
						return System.Windows.Forms.DialogResult.Retry;
					case MessageBoxButtons.YesNoCancel:
						return System.Windows.Forms.DialogResult.No;
					case MessageBoxButtons.YesNo:
						return System.Windows.Forms.DialogResult.Yes;
					case MessageBoxButtons.RetryCancel:
						return System.Windows.Forms.DialogResult.Retry;
					default:
						throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null);
				}
			case DialogExResult.Button3:
				switch (buttons) {
					case MessageBoxButtons.AbortRetryIgnore:
						return System.Windows.Forms.DialogResult.Abort;
					case MessageBoxButtons.YesNoCancel:
						return System.Windows.Forms.DialogResult.Yes;
					default:
						throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null);
				}
			default:
				throw new ArgumentOutOfRangeException(nameof(result), result, null);
		}
	}

	private void _alwaysCheckBox_CheckedChanged(object sender, EventArgs e) {
		AlwaysFlag = _alwaysCheckBox.Checked;
	}
}