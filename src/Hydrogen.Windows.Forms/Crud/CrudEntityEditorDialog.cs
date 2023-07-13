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
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public partial class CrudEntityEditorDialog : Form {
	private ICrudDataSource<object> _dataSource;
	private ICrudEntityEditor<object> _crudEntityEditor;
	private DataSourceCapabilities _capabilities;
	private bool _isNewEntity;
	private object _entity;

	public CrudEntityEditorDialog() {
		InitializeComponent();
		UserAction = null;
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public CrudAction? UserAction { get; set; }

	public void SetEntityEditor(ICrudDataSource<object> dataSource, ICrudEntityEditor<object> entityEditor, DataSourceCapabilities capabilities, object entity, bool isNewEntity) {
		_isNewEntity = isNewEntity;
		_entity = entity;
		_dataSource = dataSource;
		_crudEntityEditor = entityEditor;
		_capabilities = capabilities;
		var editorControl = entityEditor.AsControl();
		var targetPanelWidth = editorControl.Width;
		var targetPanelHeight = editorControl.Height;
		var currentPanelWidth = _entityEditorControlPanel.Width;
		var currentPanelHeight = _entityEditorControlPanel.Height;
		this.Location = new Point(this.Location.X - (targetPanelWidth - currentPanelWidth) / 2, this.Location.Y - (targetPanelHeight - currentPanelHeight) / 2);
		this.Size = new Size(this.Width + (targetPanelWidth - currentPanelWidth), this.Height + (targetPanelHeight - currentPanelHeight));
		editorControl.Dock = DockStyle.Fill;
		var canSave = (isNewEntity && _capabilities.HasFlag(DataSourceCapabilities.CanCreate)) || (!isNewEntity && _capabilities.HasFlag(DataSourceCapabilities.CanUpdate));
		_entityEditorControlPanel.Controls.Add(editorControl);
		_deleteButton.Visible = _capabilities.HasFlag(DataSourceCapabilities.CanDelete) && !isNewEntity;
		_saveButton.Visible = canSave;
		if (!canSave) {
			_cancelButton.Text = "OK";
			_cancelButton.Location = _saveButton.Location;
		}
		entityEditor.SetEntity(capabilities, entity, isNewEntity);
	}

	public void CancelChanges() {
		_crudEntityEditor.UndoChanges();
		UserAction = null;
		RequiresGridRefresh = false;
	}

	public bool SaveChanges() {
		if (_isNewEntity) {
			if (Validate(CrudAction.Create)) {
				_crudEntityEditor.AcceptChanges();
				_dataSource.Create(_crudEntityEditor.GetEntityWithChanges());
				UserAction = CrudAction.Create;
				RequiresGridRefresh = true;
				return true;
			}
		} else {
			if (Validate(CrudAction.Update)) {
				_crudEntityEditor.AcceptChanges();
				_dataSource.Update(_crudEntityEditor.GetEntityWithChanges());
				UserAction = CrudAction.Update;
				RequiresGridRefresh = true;
				return true;
			}
		}

		RequiresGridRefresh = false;
		return false;
	}

	public bool DeleteEntity() {
		if (DialogEx.Show(this, SystemIconType.Question, "Confirm Delete", "Are you sure you want to delete this record?", "&No", "&Yes") == DialogExResult.Button2) {
			if (HasChanges)
				CancelChanges();
			if (Validate(CrudAction.Delete)) {
				_dataSource.Delete(_entity);
				UserAction = CrudAction.Delete;
				RequiresGridRefresh = true;
				return true;
			}
		}
		return false;
	}

	public bool Validate(CrudAction action) {
		const int rtfHeightPerLine = 20;
		const int maxErrorsWithoutScrollbar = 5;
		var errors = _dataSource.Validate(action != CrudAction.Delete ? _crudEntityEditor.GetEntityWithChanges() : _entity, action);
		var extraHeight = rtfHeightPerLine * errors.Count().ClipTo(0, maxErrorsWithoutScrollbar) - _tableLayoutPanel.RowStyles[1].Height;
		this.Size = new Size(this.Size.Width, (int)(this.Size.Height + extraHeight));
		_tableLayoutPanel.RowStyles[1].Height = (_tableLayoutPanel.RowStyles[1].Height + extraHeight).ClipTo(0.0f, float.MaxValue);
		if (errors.Any()) {
			_errorRichTextBox.Lines = errors.ToArray();
			return false;
		}
		return true;
	}

	public bool HasChanges {
		get { return _crudEntityEditor.HasChanges; }
	}

	public bool RequiresGridRefresh { get; protected set; }

	#region Event Handler

	private void EntityEditorDialog_FormClosing(object sender, FormClosingEventArgs e) {
		try {
			if (HasChanges) {
				switch (DialogEx.Show(SystemIconType.Question, "Close", "Are you sure you want to cancel your changes?", "&No", "&Yes")) {
					case DialogExResult.Button1:
						e.Cancel = true;
						break;
					case DialogExResult.Button2:
						CancelChanges();
						e.Cancel = false;
						break;
				}
			}
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}

	private void _deleteButton_Click(object sender, EventArgs e) {
		try {
			if (DeleteEntity())
				Close();
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}

	}

	private void _saveButton_Click(object sender, EventArgs e) {
		try {
			if (SaveChanges())
				Close();
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}

	}

	private void _cancelButton_Click(object sender, EventArgs e) {
		try {
			CancelChanges();
			Close();
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}

	}

	#endregion

}
