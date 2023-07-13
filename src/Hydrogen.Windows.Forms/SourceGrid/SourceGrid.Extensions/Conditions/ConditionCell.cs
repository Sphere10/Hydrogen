// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using SourceGrid.Cells;

namespace SourceGrid.Conditions;

public class ConditionCell : ICondition {
	public ConditionCell(ICellVirtual cell) {
		mCell = cell;
	}


	public delegate bool EvaluateFunctionDelegate(DataGridColumn column, int gridRow, object itemRow);


	public EvaluateFunctionDelegate EvaluateFunction;

	private ICellVirtual mCell;

	public ICellVirtual Cell {
		get { return mCell; }
	}

	#region ICondition Members

	public bool Evaluate(DataGridColumn column, int gridRow, object itemRow) {
		if (EvaluateFunction == null)
			return false;

		return EvaluateFunction(column, gridRow, itemRow);
	}

	public ICellVirtual ApplyCondition(ICellVirtual cell) {
		return Cell;
	}

	#endregion

}
