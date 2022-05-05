//-----------------------------------------------------------------------
// <copyright file="LogonDialog.cs" company="Sphere 10 Software">
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
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen;
using Hydrogen.Windows.Forms.WinForms;

namespace Hydrogen.Windows.Forms {

	public partial class LogonDialog : Form {
		public const int MaxTextLength = 5000;
		private readonly Func<string, string, Task<AuthenticationResult>> _authenticator;

		public LogonDialog() : this("Logon", "Designer mode text") {
		}

		public LogonDialog(string title, string text, Func<string, string, Task<AuthenticationResult>> authenticator = null) {
			SetStyle(ControlStyles.Selectable, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.ContainerControl, true);

			InitializeComponent();
			Text = title;
			_textLabel.Text = text;
			_authenticator = authenticator;
		}

		public AuthenticationResult LogonResult { get; protected set; }

		#region Methods

	    protected virtual async Task LogonAsync() {
	        const int rtfHeightPerLine = 15;
	        const int maxErrorsWithoutScrollbar = 10;
	        var usernameText = _usernameTextBox.Text;
	        var passwordText = _passwordTextBox.Text;
	        using (_loadingCircle.BeginAnimationScope()) {
	            try {
	                LogonResult = await _authenticator(usernameText, passwordText);
	                if (LogonResult == null) {
	                    throw new Exception("Authenticator returned null");
	                }
	                if (LogonResult.ResultCode == AuthenticationErrorCode.Success) {
	                    Close();

	                } else {
	                    try {
	                        var errors = new List<string> {LogonResult.ResultCode.GetDescription()};
	                        var extraHeight = rtfHeightPerLine*errors.Count().ClipTo(0, maxErrorsWithoutScrollbar) - _errorRichTextBox.Height;
	                        this.Size = new Size(this.Size.Width, (int) (this.Size.Height + extraHeight));
	                        if (errors.Any()) {
	                            _errorRichTextBox.Lines = errors.ToArray();
	                        }
	                    } catch (Exception error) {
	                        ExceptionDialog.Show(this, error);
	                    }
	                }

	            } catch (Exception error) {
	                this.InvokeEx(() => ExceptionDialog.Show(this, error));
	                LogonResult = new AuthenticationResult {
	                    ResultCode = AuthenticationErrorCode.ServerUnavailable,
	                    UserObject = null
	                };
	            }
	        }
	    }

	    #endregion

		#region Static Methods

		public static AuthenticationResult Show(IWin32Window owner, string title, string text, Func<string, string, Task<AuthenticationResult>> authenticator) {
			var dialog = new LogonDialog(title, text, authenticator);
			var startPosition = owner != null ? FormStartPosition.CenterParent : FormStartPosition.CenterScreen;
			dialog.StartPosition = startPosition;
			dialog.ShowDialog(owner);
			return dialog.LogonResult;
		}

		#endregion

		#region Event Handlers

		private void _okButton_Click(object sender, EventArgs e) {
			try {
				LogonAsync();
			} catch (Exception error) {
				ExceptionDialog.Show(this, error);
			}
		}

		private void _cancelButton_Click(object sender, EventArgs e) {
			try {
				LogonResult = new AuthenticationResult {
					ResultCode = AuthenticationErrorCode.Aborted
				};
				Close();
			} catch (Exception error) {
				ExceptionDialog.Show(this, error);
			}
		}

		private void _showPasswordCheckBox_CheckedChanged(object sender, EventArgs e) {
			try {
				_passwordTextBox.PasswordChar = _hidePasswordCheckBox.Checked ? '*' : (char)0;
			} catch (Exception error) {
				ExceptionDialog.Show(this, error);
			}
		}

		#endregion

	}

}
