//-----------------------------------------------------------------------
// <copyright file="Header.cs" company="Sphere 10 Software">
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
using SourceGrid.Cells.Models;

namespace SourceGrid.Extensions.PingGrids.Cells
{
    /// <summary>
    /// A cell used for the top/left cell when using DataGridRowHeader.
    /// </summary>
    public class Header : SourceGrid.Cells.Virtual.Header
    {
        public Header()
        {
            Model.AddModel(new NullValueModel());
        }
    }
}
