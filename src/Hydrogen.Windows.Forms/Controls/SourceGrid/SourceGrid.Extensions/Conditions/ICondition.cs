//-----------------------------------------------------------------------
// <copyright file="ICondition.cs" company="Sphere 10 Software">
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
    public interface ICondition
    {
        bool Evaluate(DataGridColumn column, int gridRow, object itemRow);

        ICellVirtual ApplyCondition(ICellVirtual cell);
    }
}
