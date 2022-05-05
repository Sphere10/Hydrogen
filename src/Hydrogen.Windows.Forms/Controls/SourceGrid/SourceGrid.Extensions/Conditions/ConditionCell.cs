//-----------------------------------------------------------------------
// <copyright file="ConditionCell.cs" company="Sphere 10 Software">
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

using SourceGrid.Cells;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceGrid.Conditions
{
    public class ConditionCell : ICondition
    {
        public ConditionCell(ICellVirtual cell)
        {
            mCell = cell;
        }

        public delegate bool EvaluateFunctionDelegate(DataGridColumn column, int gridRow, object itemRow);

        public EvaluateFunctionDelegate EvaluateFunction;

        private ICellVirtual mCell;
        public ICellVirtual Cell
        {
            get { return mCell; }
        }

        #region ICondition Members
        public bool Evaluate(DataGridColumn column, int gridRow, object itemRow)
        {
            if (EvaluateFunction == null)
                return false;

            return EvaluateFunction(column, gridRow, itemRow);
        }

        public ICellVirtual ApplyCondition(ICellVirtual cell)
        {
            return Cell;
        }
        #endregion
    }
}
