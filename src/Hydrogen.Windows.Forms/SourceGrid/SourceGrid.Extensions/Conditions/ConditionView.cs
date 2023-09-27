// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using SourceGrid.Cells.Views;
using SourceGrid.Cells;

namespace SourceGrid.Conditions;

public class ConditionView : ICondition {
	public ConditionView(IView view) {
		mView = view;
	}


	public delegate bool EvaluateFunctionDelegate(DataGridColumn column, int gridRow, object itemRow);


	public EvaluateFunctionDelegate EvaluateFunction;

	private IView mView;

	public IView View {
		get { return mView; }
	}

	#region ICondition Members

	public bool Evaluate(DataGridColumn column, int gridRow, object itemRow) {
		if (EvaluateFunction == null)
			return false;

		return EvaluateFunction(column, gridRow, itemRow);
	}

	public ICellVirtual ApplyCondition(ICellVirtual cell) {
		SourceGrid.Cells.ICellVirtual copied = cell.Copy();
		copied.View = View;

		return copied;
	}

	#endregion

}
