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

public class CrudGridColumn<TEntity> : ICrudGridColumn {
	public CrudGridColumn() {
		DropDownItemsNullDisplayString = string.Empty;
	}

	public string ColumnName { get; set; }
	public CrudCellDisplayType DisplayType { get; set; }
	public Type DataType { get; set; }
	public string SortName { get; set; }
	public bool ExpandsToFit { get; set; }
	public bool CanEditCell { get; set; }
	public Func<TEntity, bool> PropertyHasValue { get; set; }
	public Func<TEntity, object> PropertyValue { get; set; }
	public Action<TEntity, object> SetPropertyValue { get; set; }
	public Func<TEntity, IEnumerable<object>> DropDownItems { get; set; }
	public string DropDownItemDisplayMember { get; set; }
	public bool DropDownItemsIncludeNullItem { get; set; }
	public string DropDownItemsNullDisplayString { get; set; }
	public Action<TEntity> ButtonAction { get; set; }
	public Func<TEntity, Size> ButtonSize { get; set; }
	public Func<TEntity, string> ButtonCaption { get; set; }
	public Func<TEntity, Image> ButtonImage { get; set; }
	public Func<TEntity, string> DateTimeFormat { get; set; }

	public bool CellHasValue(object row) {
		if (PropertyHasValue == null)
			return true;
		return PropertyHasValue((TEntity)row);
	}

	public object GetCellValue(object row) {
		if (PropertyValue == null)
			return null;

		return PropertyValue((TEntity)row);
	}

	public void SetCellValue(object row, object @value) {
		SetPropertyValue((TEntity)row, @value);
	}

	public IEnumerable<object> GetDropDownItems(object row) {
		return DropDownItems((TEntity)row);
	}

	public void ButtonPressed(object row) {
		ButtonAction((TEntity)row);
	}

	public Size GetButtonSize(object row) {
		if (ButtonSize == null)
			return Size.Empty;

		return ButtonSize((TEntity)row);
	}

	public string GetButtonCaption(object row) {
		if (ButtonCaption == null)
			return string.Empty;

		return ButtonCaption((TEntity)row);
	}

	public Image GetButtonImage(object row) {
		if (ButtonImage == null)
			return null;

		return GetButtonImage(row);
	}

	public string GetDateTimeFormat(object row) {
		if (DateTimeFormat == null) {
			switch (DisplayType) {
				case CrudCellDisplayType.Date:
					return "yyyy-MM-dd";
				case CrudCellDisplayType.Time:
					return "HH:mm:ss.fff";
				case CrudCellDisplayType.DateTime:
				default:
					return "yyyy-MM-dd HH:mm:ss.fff";
			}
		}
		return DateTimeFormat((TEntity)row);
	}


}
