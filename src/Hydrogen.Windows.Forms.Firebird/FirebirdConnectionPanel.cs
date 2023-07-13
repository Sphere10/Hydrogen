// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel;
//using FirebirdSql.Data.FirebirdClient;
using FirebirdSql.Data.FirebirdClient;
using Hydrogen.Data;

namespace Hydrogen.Windows.Forms.Firebird;

public partial class FirebirdConnectionPanel : ConnectionPanelBase {
	public FirebirdConnectionPanel() {
		InitializeComponent();
	}

	protected override IDAC GetDACInternal() {
		return new FirebirdDAC(ConnectionString);
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override string ConnectionString {
		get {
			var server = _serverTextBox.Text;
			var database = _databaseTextBox.Text;
			var username = _usernameTextBox.Text;
			var password = _passwordTextBox.Text;
			var port = Tools.Parser.Parse<int?>(_portTextBox.Text);
			var connectionStringBuilder = new FbConnectionStringBuilder();
			connectionStringBuilder.DataSource = server;
			if (port.HasValue)
				connectionStringBuilder.Port = port.Value;
			connectionStringBuilder.Database = database;
			connectionStringBuilder.UserID = username;
			connectionStringBuilder.Password = password ?? string.Empty;
			connectionStringBuilder.ServerType = FbServerType.Default;
			return connectionStringBuilder.ToString();
		}
		set {
			// Blame the stupid FbConnectionStringBuilder implementation for below try/catch
			var connectionStringBuilder = new FbConnectionStringBuilder(value);
			try {
				_serverTextBox.Text = connectionStringBuilder.DataSource;
			} catch {
				_serverTextBox.Text = string.Empty;
			}
			try {
				_portTextBox.Text = connectionStringBuilder.Port.ToString();
			} catch {
				_portTextBox.Text = string.Empty;
			}
			try {
				_databaseTextBox.Text = connectionStringBuilder.Database;
			} catch {
				_databaseTextBox.Text = string.Empty;
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
		get { return Database; }
	}

	public string Server {
		get { return _serverTextBox.Text; }
		set { _serverTextBox.Text = value; }
	}

	public string Database {
		get { return _databaseTextBox.Text; }
		set { _databaseTextBox.Text = value; }
	}

	public string Username {
		get { return _usernameTextBox.Text; }
		set { _usernameTextBox.Text = value; }
	}

	public string Password {
		get { return _passwordTextBox.Text; }
		set { _passwordTextBox.Text = value; }
	}

	public string Port {
		get { return _portTextBox.Text; }
		set { _portTextBox.Text = value; }
	}


}
