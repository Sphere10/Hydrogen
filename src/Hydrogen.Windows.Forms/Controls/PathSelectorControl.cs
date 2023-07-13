// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

[DefaultEvent("PathChanged")]
public partial class PathSelectorControl : UserControlEx {
	public event EventHandlerEx PathChanged;
	private string _textOnEnter;

	public PathSelectorControl() {
		InitializeComponent();
	}

	[Browsable(true)]
	[DefaultValue(PathSelectionMode.OpenFile)]
	public PathSelectionMode Mode { get; set; }


	[Browsable(true)]
	[DefaultValue(false)]
	public bool ForcePathExists { get; set; }

	[Browsable(true)]
	[DefaultValue(true)]
	public bool Editable {
		get { return !_filenameTextBox.ReadOnly; }
		set { _filenameTextBox.ReadOnly = !value; }
	}

	public string Path {
		get { return _filenameTextBox.Text; }
		set {
			_filenameTextBox.Text = value;
			if (!this.DesignMode) {
				NotifyPathChanged();
			}
		}
	}


	[Browsable(true)]
	public string PlaceHolderText {
		get { return _filenameTextBox != null ? _filenameTextBox.PlaceHolderText : null; }
		set {
			if (_filenameTextBox != null)
				_filenameTextBox.PlaceHolderText = value;
		}
	}

	protected virtual void OnPathChanged() {
	}

	private void NotifyPathChanged() {
		OnPathChanged();
		PathChanged?.Invoke();
	}

	private void ShowOpenDialog() {
		var dlg = new OpenFileDialog();
		dlg.CheckFileExists = ForcePathExists;
		dlg.CheckPathExists = ForcePathExists;
		dlg.Multiselect = false;
		if (dlg.ShowDialog(this) == DialogResult.OK) {
			Path = dlg.FileName;
		}

	}

	private void ShowSaveDialog() {
		var dlg = new SaveFileDialog();
		dlg.CheckPathExists = ForcePathExists;
		dlg.CheckFileExists = ForcePathExists;
		if (dlg.ShowDialog(this) == DialogResult.OK) {
			Path = dlg.FileName;
		}
	}

	private void ShowOpenSaveDialog() {
		var dlg = new OpenFileDialog();
		dlg.Title = "Select File";
		dlg.CheckFileExists = ForcePathExists;
		dlg.CheckPathExists = ForcePathExists;
		dlg.Multiselect = false;
		if (dlg.ShowDialog(this) == DialogResult.OK) {
			Path = dlg.FileName;
		}
	}

	private void ShowSelectFolderDialog() {
		var dlg = new FolderBrowserDialog();
		if (dlg.ShowDialog(this) == DialogResult.OK) {
			Path = dlg.SelectedPath;
		}

	}

	#region Event Handlers

	private void _filenameTextBox_Enter(object sender, EventArgs e) {
		try {
			if (_textOnEnter == null)
				_textOnEnter = this._filenameTextBox.Text;
		} catch (Exception error) {
			SystemLog.Exception(error);
			ExceptionDialog.Show(error);
		}
	}

	private void _filenameTextBox_Validating(object sender, CancelEventArgs e) {
		try {
			var textOnLeave = _filenameTextBox.Text;
			if (!string.IsNullOrEmpty(textOnLeave) && _textOnEnter != textOnLeave && ForcePathExists) {
				if (Mode.IsIn(PathSelectionMode.Folder)) {
					e.Cancel = !Directory.Exists(textOnLeave);
				} else {
					e.Cancel = !File.Exists(textOnLeave);
				}
			}
			if (e.Cancel)
				return;

			if (!string.IsNullOrEmpty(textOnLeave) && _textOnEnter != textOnLeave) {
				NotifyPathChanged();
			}
			_textOnEnter = null;
		} catch (Exception error) {
			SystemLog.Exception(error);
			ExceptionDialog.Show(error);
		}
	}

	private void _filenameTextBox_Validated(object sender, EventArgs e) {
		try {
		} catch (Exception error) {
			SystemLog.Exception(error);
			ExceptionDialog.Show(error);
		}
	}

	private void _fileSelectorButton_Click(object sender, EventArgs e) {
		try {
			switch (Mode) {
				case PathSelectionMode.OpenFile:
					ShowOpenDialog();
					break;
				case PathSelectionMode.SaveFile:
					ShowSaveDialog();
					break;
				case PathSelectionMode.File:
					ShowOpenSaveDialog();
					break;
				case PathSelectionMode.Folder:
					ShowSelectFolderDialog();
					break;
			}
		} catch (Exception error) {
			SystemLog.Exception(error);
			ExceptionDialog.Show(error);
		}
	}

	protected override void OnEnabledChanged(EventArgs e) {
		try {
			base.OnEnabledChanged(e);
			_filenameTextBox.Enabled =
				_fileSelectorButton.Enabled = this.Enabled;
		} catch (Exception error) {
			SystemLog.Exception(error);
			ExceptionDialog.Show(error);
		}
	}

	#endregion


	public new class StateEventProvider : ControlStateEventProviderBase<PathSelectorControl> {
		protected override void RegisterStateChangedListener(PathSelectorControl control, EventHandlerEx eventHandler) {
			control.PathChanged += eventHandler;
		}

		protected override void DeregisterStateChangedListener(PathSelectorControl control, EventHandlerEx eventHandler) {
			control.PathChanged -= eventHandler;
		}
	}
}
