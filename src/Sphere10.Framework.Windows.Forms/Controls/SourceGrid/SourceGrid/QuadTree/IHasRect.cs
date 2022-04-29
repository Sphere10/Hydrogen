//-----------------------------------------------------------------------
// <copyright file="IHasRect.cs" company="Sphere 10 Software">
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

using SourceGrid;
using System;
using System.Drawing;

namespace QuadTreeLib
{
    /// <summary>
    /// An interface that defines and object with a rectangle
    /// </summary>
    public interface IHasRect
    {
        Range Rectangle { get; }
    }
}
