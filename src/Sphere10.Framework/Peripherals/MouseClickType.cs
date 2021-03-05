//-----------------------------------------------------------------------
// <copyright file="MouseClickType.cs" company="Sphere 10 Software">
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
using System.Reflection;

namespace Sphere10.Framework
{
    /// <summary>
    /// Summary description for MouseClickType.
    /// </summary>
    [Obfuscation(Exclude = true)]
    public enum MouseClickType
    {
		None = 0,
        Single,
        Double,
        Tripple
    }
}
