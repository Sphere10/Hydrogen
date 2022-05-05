//-----------------------------------------------------------------------
// <copyright file="DecoratorHighlight.cs" company="Sphere 10 Software">
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
using System.Text;

namespace SourceGrid.Decorators
{
    public class DecoratorHighlight : DecoratorBase
    {
        private CellRange mRange = CellRange.Empty;
        /// <summary>
        /// Gets or sets the range to draw
        /// </summary>
        public CellRange Range
        {
            get { return mRange; }
            set { mRange = value; }
        }


        public override bool IntersectWith(CellRange range)
        {
            return Range.IntersectsWith(range);
        }

        public override void Draw(RangePaintEventArgs e)
        {
        }
    }
}
