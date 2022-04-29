//-----------------------------------------------------------------------
// <copyright file="EnumeratedItemDescription.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework {
	[Flags]
    public enum EnumeratedItemDescription {
        First = 1,
        Last = 2,
        Interior = 4,
        Even = 8,
        Odd = 16,
    }
}
