//-----------------------------------------------------------------------
// <copyright file="SizingMask.cs" company="Sphere 10 Software">
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

namespace Hydrogen {

	[Flags]
	public enum SizingMask : int  {
		None				= -1,
		CocoaAutoresizing	= 1 << 0,
		UserResizingMask	= 1 << 1,
		Fit					= 1 << 2,
		Compact				= 1 << 3 | UserResizingMask,
		Expand				= 1 << 4 | UserResizingMask,
		FitCompact			= UserResizingMask | Fit | Compact,
		FitExpanding		= UserResizingMask | Fit | Expand
	}
}

