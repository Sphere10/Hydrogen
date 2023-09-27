// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel;
using System.IO;
using FirebirdSql.Data.FirebirdClient;
using Hydrogen.Data;


namespace Hydrogen.Windows.Forms.Firebird;

public partial class FirebirdEmbeddedConnectionBar : ConnectionBarBase, IDatabaseConnectionProvider {
	public FirebirdEmbeddedConnectionBar() {
		InitializeComponent();
	}

	public bool HasPassword {
		get { return _passwordTextBox.Text.Trim().Length > 0; }
	}

	public string Password {
		get { return _passwordTextBox.Text; }
	}

	public string Filename {
		get { return _fileSelectorControl.Path; }
	}

	public PathSelectionMode Mode {
		get { return _fileSelectorControl.Mode; }
		set { _fileSelectorControl.Mode = value; }
	}

	protected override IDAC GetDACInternal() {
		return new FirebirdDAC(ConnectionString);
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override string ConnectionString {
		get {
			var connectionStringBuilder = new FbConnectionStringBuilder {
				Database = _fileSelectorControl.Path,
				UserID = _usernameTextBox.Text,
				Password = _passwordTextBox.Text,
				ServerType = FbServerType.Embedded
			};
			return connectionStringBuilder.ToString();
		}
		set {
			var connectionStringBuilder = new FbConnectionStringBuilder(value);
			try {
				_fileSelectorControl.Path = connectionStringBuilder.Database;
			} catch {
				_fileSelectorControl.Path = string.Empty;
			}
			try {
				_usernameTextBox.Text = connectionStringBuilder.UserID;
			} catch {
				_usernameTextBox.Text = string.Empty;
			}
			try {
				_passwordTextBox.Text = connectionStringBuilder.Password;
			} catch {
				_passwordTextBox.Text = string.Empty;
			}
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override string DatabaseName {
		get {
			var path = _fileSelectorControl.Path;
			if (string.IsNullOrEmpty(path))
				return null;

			return Path.GetFileNameWithoutExtension(path);
		}
	}

}
