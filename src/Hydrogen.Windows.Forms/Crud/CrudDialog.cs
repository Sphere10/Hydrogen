// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public partial class CrudDialog : Form {
	private Action _delayedInitializationAction;

	public CrudDialog() {
		InitializeComponent();
		_delayedInitializationAction = null;
	}

	protected override async void OnLoad(EventArgs e) {
		base.OnLoad(e);
		await _crudGrid.RefreshGrid();
	}

	public static void Show<TEntity>(IWin32Window window, string title, IEnumerable<ICrudGridColumn> gridBindings, DataSourceCapabilities capabilities, ICrudDataSource<TEntity> dataSource) {
		Show(window, title, gridBindings, typeof(DefaultCrudEntityEditor), capabilities, dataSource);
	}

	public static void Show<TEntity>(string title, IEnumerable<ICrudGridColumn> gridBindings, DataSourceCapabilities capabilities, ICrudDataSource<TEntity> dataSource) {
		Show(null, title, gridBindings, typeof(DefaultCrudEntityEditor), capabilities, dataSource);
	}

	public static void Show<TEntity>(string title, IEnumerable<ICrudGridColumn> gridBindings, Type entityEditorType, DataSourceCapabilities capabilities, ICrudDataSource<TEntity> dataSource) {
		Show(null, title, gridBindings, entityEditorType, capabilities, dataSource);
	}

	public static void Show<TEntity>(IWin32Window window, string title, IEnumerable<ICrudGridColumn> gridBindings, Type entityEditorType, DataSourceCapabilities capabilities, ICrudDataSource<TEntity> dataSource) {
		var crudDialog = new CrudDialog();
		crudDialog.Text = title;
		crudDialog.SetCrudParameters(gridBindings, entityEditorType, capabilities, dataSource);
		crudDialog.ShowDialog(window);
	}

	public void SetCrudParameters<TEntity>(IEnumerable<ICrudGridColumn> gridBindings, Type entityEditorType, DataSourceCapabilities capabilities, ICrudDataSource<TEntity> dataSource) {
		var initializationAction =
			new Action(() => {
				try {
					if (entityEditorType != null)
						_crudGrid.SetEntityEditor<TEntity>(entityEditorType);
					_crudGrid.Capabilities = capabilities;
					_crudGrid.GridBindings = gridBindings;
					_crudGrid.SetDataSource(dataSource);
				} catch (Exception error) {
					ExceptionDialog.Show(this, error);
				}
			});

		if (!IsHandleCreated)
			_delayedInitializationAction = initializationAction;
		else
			initializationAction();
	}

	private void _okButton_Click(object sender, EventArgs e) {
		try {
			Close();
		} catch (Exception error) {
			ExceptionDialog.Show(this, error);
		}
	}


	protected override void OnHandleCreated(EventArgs e) {
		base.OnHandleCreated(e);
		if (_delayedInitializationAction != null)
			_delayedInitializationAction();
	}

}
