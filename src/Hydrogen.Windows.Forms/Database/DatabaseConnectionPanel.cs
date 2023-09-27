// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hydrogen.Data;

namespace Hydrogen.Windows.Forms;

public partial class DatabaseConnectionPanel : ConnectionPanelBase, IDatabaseConnectionProvider {
	private const string MSSQLConnectionPanelTypeName = "Hydrogen.Windows.Forms.MSSQL.MSSQLConnectionPanel, Hydrogen.Windows.Forms.MSSQL";
	private const string SqliteConnectionPanelTypeName = "Hydrogen.Windows.Forms.Sqlite.SqliteConnectionPanel, Hydrogen.Windows.Forms.Sqlite";
	private const string FirebirdConnectionPanelTypeName = "Hydrogen.Windows.Forms.Firebird.FirebirdConnectionPanel, Hydrogen.Windows.Forms.Firebird";
	private const string FirebirdFileConnectionPanelTypeName = "Hydrogen.Windows.Forms.Firebird.FirebirdEmbeddedConnectionPanel, Hydrogen.Windows.Forms.Firebird";

	public event EventHandlerEx<DatabaseConnectionPanel, DBMSType> DBMSTypeChanged;

	public DatabaseConnectionPanel() {
		InitializeComponent();
		_dbmsCombo.EnumType = typeof(DBMSType);
		SelectDefaultPanel();
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

	//[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	//[Browsable(false)]
	public override string ConnectionString {
		get => CurrentConnectionPanel.ConnectionString;
		set => CurrentConnectionPanel.ConnectionString = value;
	}

	public virtual DBReference Database => new DBReference {
		DBMSType = SelectedDBMSType,
		ConnectionString = ConnectionString
	};

	public override string DatabaseName => CurrentConnectionPanel.DatabaseName;

	protected virtual void OnDBMSTypeChanged() {
	}

	protected virtual void SelectDefaultPanel() {
		ChangeConnectionPanel(MSSQLConnectionPanelTypeName);
	}

	protected override IDAC GetDACInternal() {
		return CurrentConnectionPanel.GetDAC();
	}

	public override Task<Result> TestConnection() {
		return CurrentConnectionPanel.TestConnection();
	}

	protected ConnectionPanelBase CurrentConnectionPanel { get; set; }

	protected virtual void _dbmsCombo_SelectedIndexChanged(object sender, EventArgs e) {
		var comboItem = (DBMSType)_dbmsCombo.SelectedEnum;
		switch (comboItem) {
			case DBMSType.SQLServer:
				if (CurrentConnectionPanel == null || CurrentConnectionPanel.GetType().Name != MSSQLConnectionPanelTypeName)
					ChangeConnectionPanel(MSSQLConnectionPanelTypeName);
				break;
			case DBMSType.Sqlite:
				if (CurrentConnectionPanel == null || CurrentConnectionPanel.GetType().Name != SqliteConnectionPanelTypeName)
					ChangeConnectionPanel(SqliteConnectionPanelTypeName);
				break;
			case DBMSType.Firebird:
				if (CurrentConnectionPanel == null || CurrentConnectionPanel.GetType().Name != FirebirdConnectionPanelTypeName)
					ChangeConnectionPanel(FirebirdConnectionPanelTypeName);
				break;
			case DBMSType.FirebirdFile:
				if (CurrentConnectionPanel == null || CurrentConnectionPanel.GetType().Name != FirebirdFileConnectionPanelTypeName)
					ChangeConnectionPanel(FirebirdFileConnectionPanelTypeName);
				break;
		}
	}

	protected void ChangeConnectionPanel(string connectionPanelTypeName) {
		if (Tools.Runtime.IsDesignMode)
			return;

		var connectionBar = (ConnectionPanelBase)Tools.Object.Create(connectionPanelTypeName);
		ChangeConnectionPanel(connectionBar);
	}

	protected void ChangeConnectionPanel(ConnectionPanelBase connectionPanel) {
		if (CurrentConnectionPanel != null)
			_connectionProviderPanel.Controls.Remove(CurrentConnectionPanel);
		CurrentConnectionPanel = connectionPanel;
		CurrentConnectionPanel.Dock = DockStyle.Fill;
		CurrentConnectionPanel.DockPadding.All = 0;
		CurrentConnectionPanel.Margin = Padding.Empty;
		_connectionProviderPanel.Controls.Add(CurrentConnectionPanel);
		RaiseDBMSTypeChangedEvent();
	}

	private void RaiseDBMSTypeChangedEvent() {
		OnDBMSTypeChanged();
		DBMSTypeChanged?.Invoke(this, SelectedDBMSType);
	}
}
