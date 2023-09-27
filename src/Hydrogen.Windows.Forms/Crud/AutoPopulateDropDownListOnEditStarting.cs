// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Linq;
using SourceGrid;
using SourceGrid.Cells.Editors;

namespace Hydrogen.Windows.Forms;

internal class AutoPopulateDropDownListOnEditStarting : SourceGrid.Cells.Controllers.ControllerBase {
	private readonly DropDownList _dropDownList;
	private readonly ICrudGridColumn _columnBinding;
	private readonly object _entity;

	public AutoPopulateDropDownListOnEditStarting(DropDownList ddl, ICrudGridColumn column, object entity) {
		_dropDownList = ddl;
		_columnBinding = column;
		_entity = entity;
	}

	public override void OnEditStarting(CellContext sender, System.ComponentModel.CancelEventArgs e) {
		base.OnEditStarting(sender, e);
		var dropDownValues = _columnBinding.GetDropDownItems(_entity).ToList();
		if (_columnBinding.DropDownItemsIncludeNullItem) {
			dropDownValues.Add(_dropDownList.NullString);
			_dropDownList.AllowNull = true;
			//_dropDownList.NullDisplayString = _columnBinding.DropDownItemsNullDisplayString;
		}
		_dropDownList.StandardValues = dropDownValues;
	}


}
