// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester;

public partial class PasswordDialogTestScreen : ApplicationScreen {
	private readonly TextWriter _outputTextWriter;

	public PasswordDialogTestScreen() {
		InitializeComponent();
		_outputTextWriter = new TextBoxWriter(_outputTextBox);
	}

	private void _standardButton_Click(object sender, EventArgs e) {
		string password;
		var result = PasswordDialog.Show(this, "Password", "Change your Administrator password", out password);
		_outputTextWriter.WriteLine("Result = {0}, Password = {1}", result, password);
	}

	private void _customButton_Click(object sender, EventArgs e) {
		string password;
		var result = PasswordDialog.Show(this, "Password", "Change your Administrator password", out password, PolicyValidator);
		_outputTextWriter.WriteLine("Result = {0}, Password = {1}", result, password);
	}


	private IEnumerable<string> PolicyValidator(string password) {
		var digitCount = password.Count(char.IsDigit);
		var lowerCount = password.Count(char.IsLower);
		var higherCount = password.Count(char.IsLower);
		var whitespaceCount = password.Count(char.IsWhiteSpace);

		if (digitCount == 0)
			yield return "Password needs at least 1 digit";

		if (lowerCount == 0)
			yield return "Password needs at least 1 lower case letter";

		if (higherCount == 0)
			yield return "Password needs at least 1 upper case letter";

		if (whitespaceCount > 0)
			yield return "Password cannot have spaces, tabs or other whitespace";

		if (password.Length < 5)
			yield return "Passwords need to contain 5 or more characters";
	}

	private void _logonButton_Click(object sender, EventArgs e) {
		var logonResult = LogonDialog.Show(this, "Logon", "Logon to Cranewatch", Authenticator);
		_outputTextWriter.WriteLine("LogonResult = {0}, UserObject = {1}", logonResult.ResultCode, logonResult.UserObject ?? "(None)");
	}

	private async Task<AuthenticationResult> Authenticator(string username, string password) {
		await Task.Delay(1000);
		if (username == string.Empty) {
			return new AuthenticationResult {
				ResultCode = AuthenticationErrorCode.ServerUnavailable
			};
		} else if (username == password) {
			return new AuthenticationResult {
				ResultCode = AuthenticationErrorCode.Success,
				UserObject = "User"
			};
		} else {
			return new AuthenticationResult {
				ResultCode = AuthenticationErrorCode.InvalidCredentials
			};
		}
	}
}
