// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Threading;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class ValidationIndicatorTestScreen : ApplicationScreen {
	private TextWriter _outputTextWriter;

	public ValidationIndicatorTestScreen() {
		InitializeComponent();
		_outputTextWriter = new TextBoxWriter(_outputTextBox);
	}

	private void _validButton_Click(object sender, EventArgs e) {
		_validationIndicator1.State = ValidationState.Valid;
	}

	private void _errorButton_Click(object sender, EventArgs e) {
		_validationIndicator1.State = ValidationState.Error;
	}

	private void _validatingButton_Click(object sender, EventArgs e) {
		_validationIndicator1.State = ValidationState.Validating;
	}

	private void _disabledButton_Click(object sender, EventArgs e) {
		_validationIndicator1.State =
			ValidationState.Disabled;
	}

	private void _textBox_TextChanged(object sender, EventArgs e) {
		_validationIndicator2.RunValidation();
	}

	private void _validationIndicator2_PerformValidation(ValidationIndicator arg1, ValidationIndicatorEvent arg2) {
		var input = _textBox.Text;
		Thread.Sleep(1000);
		if (input == "Herman") {
			arg2.ValidationResult = true;
			arg2.ValidationMessage = "Success";
		} else {
			arg2.ValidationResult = false;
			arg2.ValidationMessage = string.Format("'{0}' does not match 'Herman'", input);
		}
		_outputTextWriter.WriteLine("{0}: PerformedValidation on input '{1}'", arg1.Name, input);
	}

	private void _validationIndicator1_ValidationStateChanged(ValidationIndicator arg1, ValidationState arg2, ValidationState arg3) {
		_outputTextWriter.WriteLine("{0}: State Changed {1} -> {2}", arg1.Name, arg2, arg3);
	}

	private void _validationIndicator2_ValidationStateChanged(ValidationIndicator arg1, ValidationState arg2, ValidationState arg3) {
		_outputTextWriter.WriteLine("{0}: State Changed {1} -> {2}", arg1.Name, arg2, arg3);
	}

	private void _enableDisableButton_Click(object sender, EventArgs e) {
		_validationIndicator2.Enabled = !_validationIndicator2.Enabled;
	}
}
