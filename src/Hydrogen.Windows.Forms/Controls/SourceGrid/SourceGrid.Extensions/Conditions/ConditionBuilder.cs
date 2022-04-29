//-----------------------------------------------------------------------
// <copyright file="ConditionBuilder.cs" company="Sphere 10 Software">
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

using SourceGrid.Cells.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceGrid.Conditions
{
    public static class ConditionBuilder
    {
        public static ICondition AlternateView(
                                            IView view,
                                            System.Drawing.Color alternateBackcolor,
                                            System.Drawing.Color alternateForecolor)
        {
            SourceGrid.Cells.Views.IView viewAlternate = (SourceGrid.Cells.Views.IView)view.Clone();
            viewAlternate.BackColor = alternateBackcolor;
            viewAlternate.ForeColor = alternateForecolor;

            SourceGrid.Conditions.ConditionView condition =
                        new SourceGrid.Conditions.ConditionView(viewAlternate);

            condition.EvaluateFunction = delegate(SourceGrid.DataGridColumn column, int gridRow, object itemRow)
                                    {
                                        return (gridRow & 1) == 1;
                                    };

            return condition;
        }
    }
}
