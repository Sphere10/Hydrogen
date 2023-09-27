// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Drawing;

namespace Hydrogen.Windows.Forms;

public interface ICrudGridColumn {
	string ColumnName { get; set; }
	Type DataType { get; set; }
	CrudCellDisplayType DisplayType { get; }
	string SortName { get; }

	bool CellHasValue(object row);

	object GetCellValue(object row);

	void SetCellValue(object row, object @value);

	bool ExpandsToFit { get; }
	bool CanEditCell { get; }

	IEnumerable<object> GetDropDownItems(object row);

	string DropDownItemDisplayMember { get; }
	bool DropDownItemsIncludeNullItem { get; }
	string DropDownItemsNullDisplayString { get; }

	void ButtonPressed(object row);

	Size GetButtonSize(object row);

	string GetButtonCaption(object row);

	Image GetButtonImage(object row);

	string GetDateTimeFormat(object row);
}


public enum CrudCellDisplayType {
	Text,
	Boolean,
	Currency,
	Numeric,
	DateTime,
	Date,
	Time,
	DropDownList,
	Button,
	EditCommand,
	DeleteCommand,
	Empty
}
