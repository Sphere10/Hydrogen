// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen.Data;

namespace Hydrogen.Windows.Forms;

public partial class ExecuteScriptForm : Form {
	public ExecuteScriptForm(IDAC dac, ISQLBuilder script) {
		InitializeComponent();
		_scriptTextBox.Text = script.ToString();
		_scriptTextBox.FocusAtEnd();
		_databaseConnectionStringLabel.Text = dac.ConnectionString;
		DAC = dac;
		Script = script;
		DialogResult = DialogResult.Cancel;
	}

	public IDAC DAC { get; private set; }

	public ISQLBuilder Script { get; private set; }


	private async void _executeScriptButton_Click(object sender, EventArgs e) {
		try {
			using (LoadingCircle.EnterAnimationScope(this, disableControls: true)) {
				await Task.Run(() => DAC.ExecuteBatch(Script));
			}
			this.DialogResult = DialogResult.OK;

			Close();
		} catch (Exception error) {
			ExceptionDialog.Show("Error", error);
		} finally {
		}
	}

	private void _copyToClipboardButton_Click(object sender, EventArgs e) {
		Clipboard.SetText(_scriptTextBox.Text);
	}


}
