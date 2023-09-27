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
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen.Data;

namespace Hydrogen.Windows.Forms;

public partial class DatabaseConnectionBar : ConnectionBarBase, IDatabaseConnectionProvider {
	private const string MSSQLConnectionBarTypeName = "Hydrogen.Windows.Forms.MSSQL.MSSQLConnectionBar, Hydrogen.Windows.Forms.MSSQL";
	private const string SqliteConnectionBarTypeName = "Hydrogen.Windows.Forms.Sqlite.SqliteConnectionBar, Hydrogen.Windows.Forms.Sqlite";
	private const string FirebirdConnectionBarTypeName = "Hydrogen.Windows.Forms.Firebird.FirebirdConnectionBar, Hydrogen.Windows.Forms.Firebird";
	private const string FirebirdFileConnectionBarTypeName = "Hydrogen.Windows.Forms.Firebird.FirebirdEmbeddedConnectionBar, Hydrogen.Windows.Forms.Firebird";

	public DatabaseConnectionBar() {
		InitializeComponent();
		_dbmsCombo.EnumType = typeof(DBMSType);
		SelectDefaultBar();
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public DBMSType SelectedDBMSType {
		get => (DBMSType)_dbmsCombo.SelectedEnum;
		set => _dbmsCombo.SelectedEnum = value;
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public DBMSType[] IgnoreDBMS {
		get => _dbmsCombo.IgnoreEnums.Cast<DBMSType>().ToArray();
		set => _dbmsCombo.IgnoreEnums = (value ?? new DBMSType[0]).Cast<object>().ToArray();
	}


	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override string ConnectionString {
		get => CurrentConnectionBar.ConnectionString;
		set => CurrentConnectionBar.ConnectionString = value;
	}


	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public override string DatabaseName => CurrentConnectionBar.DatabaseName;

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public virtual DBReference Database {
		get => new() {
			DBMSType = SelectedDBMSType,
			ConnectionString = ConnectionString
		};
		set {
			this.SelectedDBMSType = value.DBMSType;
			this.ConnectionString = value.ConnectionString;
		}
	}

	protected virtual void SelectDefaultBar() {
		ChangeConnectionBar(MSSQLConnectionBarTypeName);
	}

	protected override IDAC GetDACInternal() {
		return CurrentConnectionBar.GetDAC();
	}

	public override Task<Result> TestConnection() {
		return CurrentConnectionBar.TestConnection();
	}

	protected ConnectionBarBase CurrentConnectionBar { get; set; }

	protected virtual void _dbmsCombo_SelectedIndexChanged(object sender, EventArgs e) {
		var comboItem = (DBMSType)_dbmsCombo.SelectedEnum;
		switch (comboItem) {
			case DBMSType.SQLServer:
				if (CurrentConnectionBar == null || CurrentConnectionBar.GetType().Name != MSSQLConnectionBarTypeName)
					ChangeConnectionBar(MSSQLConnectionBarTypeName);
				break;
			case DBMSType.Sqlite:
				if (CurrentConnectionBar == null || CurrentConnectionBar.GetType().Name != SqliteConnectionBarTypeName)
					ChangeConnectionBar(SqliteConnectionBarTypeName);
				break;
			case DBMSType.Firebird:
				if (CurrentConnectionBar == null || CurrentConnectionBar.GetType().Name != FirebirdConnectionBarTypeName)
					ChangeConnectionBar(FirebirdConnectionBarTypeName);
				break;
			case DBMSType.FirebirdFile:
				if (CurrentConnectionBar == null || CurrentConnectionBar.GetType().Name != FirebirdFileConnectionBarTypeName)
					ChangeConnectionBar(FirebirdFileConnectionBarTypeName);
				break;
		}
	}

	protected void ChangeConnectionBar(string connectionBarTypeName) {
		if (Tools.Runtime.IsDesignMode)
			return;

		var connectionBar = (ConnectionBarBase)Tools.Object.Create(connectionBarTypeName);
		ChangeConnectionBar(connectionBar);
	}

	protected void ChangeConnectionBar(ConnectionBarBase connectionBar) {
		if (CurrentConnectionBar != null)
			_connectionProviderPanel.Controls.Remove(CurrentConnectionBar);
		CurrentConnectionBar = connectionBar;
		CurrentConnectionBar.Location = new Point(0, (_connectionProviderPanel.Height - CurrentConnectionBar.Height).ClipTo(0, int.MaxValue));
		CurrentConnectionBar.Dock = DockStyle.Fill;
		CurrentConnectionBar.DockPadding.All = 0;
		CurrentConnectionBar.Width = _connectionProviderPanel.Width;
		_connectionProviderPanel.Controls.Add(CurrentConnectionBar);
	}
}
