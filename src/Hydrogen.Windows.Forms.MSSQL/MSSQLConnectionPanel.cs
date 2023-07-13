// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using Hydrogen.Data;

namespace Hydrogen.Windows.Forms.MSSQL;

public partial class MSSQLConnectionPanel : ConnectionPanelBase {
	public MSSQLConnectionPanel() {
		InitializeComponent();
	}

	protected override IDAC GetDACInternal() {
		return new MSSQLDAC(ConnectionString);
	}


	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override string ConnectionString {
		get {
			var server = _serverTextBox.Text;
			var database = _databaseTextBox.Text;
			var username = _usernameTextBox.Text;
			var password = _passwordTextBox.Text;
			var port = Tools.Parser.SafeParse<int?>(_portTextBox.Text);

			var connectionStringBuilder = new SqlConnectionStringBuilder();
			connectionStringBuilder.DataSource = port.HasValue ? string.Format("{0},{1}", server, port.Value) : server;
			connectionStringBuilder.InitialCatalog = database;
			connectionStringBuilder.UserID = username;
			connectionStringBuilder.Password = password ?? string.Empty;
			connectionStringBuilder.IntegratedSecurity = false;
			return connectionStringBuilder.ToString();
		}
		set {
			var connectionStringBuilder = new SqlConnectionStringBuilder(value);
			string server;
			int? port;
			SplitIntoServerPortParts(connectionStringBuilder.DataSource, out server, out port);
			_serverTextBox.Text = server;
			_portTextBox.Text = port.HasValue ? port.ToString() : string.Empty;
			_databaseTextBox.Text = connectionStringBuilder.InitialCatalog;
			_usernameTextBox.Text = connectionStringBuilder.UserID;
			_passwordTextBox.Text = connectionStringBuilder.Password;
		}
	}

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


	private void SplitIntoServerPortParts(string dataSourceText, out string server, out int? port) {
		dataSourceText = dataSourceText.Trim();
		var portText = dataSourceText.Reverse().TakeWhile(char.IsDigit);
		var serverText = dataSourceText.Substring(0, dataSourceText.Length - portText.Count()).Trim();
		if (serverText.EndsWith(",") && portText.Any()) {
			server = serverText.Substring(0, serverText.Length - 1);
			port = int.Parse(new string(portText.Reverse().ToArray()));
		} else {
			server = dataSourceText;
			port = null;
		}
	}

}
