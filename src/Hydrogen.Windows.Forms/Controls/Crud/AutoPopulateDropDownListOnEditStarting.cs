//-----------------------------------------------------------------------
// <copyright file="AutoPopulateDropDownListOnEditStarting.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SourceGrid;
using SourceGrid.Cells.Editors;

namespace Hydrogen.Windows.Forms {
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
}
