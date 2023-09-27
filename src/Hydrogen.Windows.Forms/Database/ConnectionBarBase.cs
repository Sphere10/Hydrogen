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
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen.Data;

namespace Hydrogen.Windows.Forms;

public partial class ConnectionBarBase : UserControlEx, IDatabaseConnectionProvider {
	public ConnectionBarBase() {
		InitializeComponent();
		ArtificialKeysFile = null;
	}

	public string ArtificialKeysFile { get; set; }

	public IDAC GetDAC() {
		var dac = GetDACInternal();
		if (ArtificialKeysFile != null)
			dac.ArtificialKeys = ArtificialKeys.LoadFromFile(ArtificialKeysFile);
		return dac;
	}

	protected virtual IDAC GetDACInternal() {
		throw new NotImplementedException();
	}

	public virtual async Task<Result> TestConnection() {
		var result = Result.Default;
		var dac = GetDAC();
		try {
			await Task.Run(() => {
				using (var scope = dac.BeginScope(openConnection: true)) {
				}
			});
		} catch (Exception error) {
			result.AddError(error.ToDisplayString());
		}
		return result;
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public virtual string ConnectionString {
		get { throw new NotImplementedException(); }
		set { throw new NotImplementedException(); }
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public virtual string DatabaseName {
		get { throw new NotImplementedException(); }
	}

	public void SelectArtificialKeysFile() {
		var dialog = new OpenFileDialog();
		if (dialog.ShowDialog(this) == DialogResult.OK) {
			if (string.IsNullOrEmpty(dialog.FileName))
				return;

			try {
				var fileContents = File.ReadAllText(dialog.FileName);
				var artificialKeys = Tools.Xml.ReadFromString<ArtificialKeys>(fileContents);
				// valid file
				ArtificialKeysFile = dialog.FileName;
				this._artificialKeysMenuItem.Text = "Remove Artificial Keys";
			} catch (Exception error) {
				var summaryError = new SoftwareException(error, "The selected file '{0}' is not a valid Artificial Keys file.".FormatWith(dialog.FileName));
				ExceptionDialog.Show(this, "Invalid Artificial Keys", summaryError);
				ArtificialKeysFile = null;
			}
		}
	}

	private void _artificialKeysMenuItem_Click(object sender, EventArgs e) {
		try {
			if (ArtificialKeysFile != null) {
				ArtificialKeysFile = null;
				_artificialKeysMenuItem.Text = "Artificial Keys";
			} else SelectArtificialKeysFile();
		} catch (Exception error) {
			ExceptionDialog.Show(this, "Error", error);
		}
	}

	private void OptionsButton_Click(object sender, EventArgs e) {
		try {
			DatabaseOptionsContextMenu.Show(OptionsButton, new Point(0, OptionsButton.Height));
		} catch (Exception error) {
			ExceptionDialog.Show(this, "Error", error);
		}
	}
}
