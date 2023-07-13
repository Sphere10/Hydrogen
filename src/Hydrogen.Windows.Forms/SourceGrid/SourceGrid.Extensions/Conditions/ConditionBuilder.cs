// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using SourceGrid.Cells.Views;

namespace SourceGrid.Conditions;

public static class ConditionBuilder {
	public static ICondition AlternateView(
		IView view,
		System.Drawing.Color alternateBackcolor,
		System.Drawing.Color alternateForecolor) {
		SourceGrid.Cells.Views.IView viewAlternate = (SourceGrid.Cells.Views.IView)view.Clone();
		viewAlternate.BackColor = alternateBackcolor;
		viewAlternate.ForeColor = alternateForecolor;

		SourceGrid.Conditions.ConditionView condition =
			new SourceGrid.Conditions.ConditionView(viewAlternate);

		condition.EvaluateFunction = delegate(SourceGrid.DataGridColumn column, int gridRow, object itemRow) { return (gridRow & 1) == 1; };

		return condition;
	}
}
