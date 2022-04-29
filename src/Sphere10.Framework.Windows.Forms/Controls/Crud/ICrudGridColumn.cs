//-----------------------------------------------------------------------
// <copyright file="ICrudGridColumn.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Windows.Forms {
	public interface ICrudGridColumn {
		string ColumnName { get; set; }
		Type DataType { get; set; }
		CrudCellDisplayType DisplayType { get; }
		string SortName { get;  }
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
}
