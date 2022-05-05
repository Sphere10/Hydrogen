//-----------------------------------------------------------------------
// <copyright file="MouseButton.cs" company="Sphere 10 Software">
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

namespace Hydrogen {
	[Obfuscation(Exclude = true)]
    [Flags]
    public enum MouseButton
    {
		None		= 0,
		Left,
		Right,
		Middle,
        XButton1,
        XButton2
    }

}
