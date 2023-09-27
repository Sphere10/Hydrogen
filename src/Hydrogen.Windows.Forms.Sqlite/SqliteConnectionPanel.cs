// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Hydrogen.Data;


namespace Hydrogen.Windows.Forms.Sqlite;

public partial class SqliteConnectionPanel : ConnectionPanelBase, IDatabaseConnectionProvider {
	public SqliteConnectionPanel() {
		InitializeComponent();
		_journalComboBox.EnumType = typeof(SqliteJournalMode);
		_syncComboBox.EnumType = typeof(SqliteSyncMode);

		_journalComboBox.SelectedEnum = SqliteJournalMode.Default;
		_syncComboBox.SelectedEnum = SqliteSyncMode.Normal;

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
		return new SqliteDAC(ConnectionString);
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override string ConnectionString {
		get {
			var connectionStringBuilder = new SQLiteConnectionStringBuilder {
				FailIfMissing = true,
				DataSource = _fileSelectorControl.Path,
				Password = _passwordTextBox.Text,
				JournalMode = SqliteHelper.Convert((SqliteJournalMode)_journalComboBox.SelectedEnum),
				SyncMode = SqliteHelper.Convert((SqliteSyncMode)_syncComboBox.SelectedEnum)
			};
			return connectionStringBuilder.ToString();
		}
		set {
			var connectionStringBuilder = new SQLiteConnectionStringBuilder(value);
			_fileSelectorControl.Path = connectionStringBuilder.DataSource;
			_passwordTextBox.Text = connectionStringBuilder.Password;
			_journalComboBox.SelectedEnum = SqliteHelper.Convert(connectionStringBuilder.JournalMode);
			_syncComboBox.SelectedEnum = SqliteHelper.Convert(connectionStringBuilder.SyncMode);
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


}
