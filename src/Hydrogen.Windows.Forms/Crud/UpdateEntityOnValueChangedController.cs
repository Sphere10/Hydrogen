// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Linq;
using SourceGrid;

namespace Hydrogen.Windows.Forms;

internal class UpdateEntityOnValueChangedController : SourceGrid.Cells.Controllers.ControllerBase {
	private readonly CrudGrid _grid;
	private readonly ICrudDataSource<object> _dataSource;
	private readonly ICrudGridColumn _columnBinding;
	private object _entity;

	public UpdateEntityOnValueChangedController(CrudGrid grid, ICrudDataSource<object> dataSource, ICrudGridColumn column, object entity) {
		_grid = grid;
		_dataSource = dataSource;
		_columnBinding = column;
		_entity = entity;
	}

	public override void OnValueChanged(CellContext sender, EventArgs e) {
		// set the entity's property with the new cell value
		_columnBinding.SetCellValue(_entity, sender.Value);

		// validate the change via the data source
		var errors = _dataSource.Validate(_entity, CrudAction.Update);
		if (errors.Any()) {
			DialogEx.Show(sender.Grid, SystemIconType.Error, "Unable to update", errors.ToParagraphCase(), "OK");
			_dataSource.Refresh(_entity);
		} else {
			_dataSource.Update(_entity);
		}
		_grid.NotifyEntityUpdated(_entity);
	}
}
