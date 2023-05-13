//-----------------------------------------------------------------------
// <copyright file="LogOptions.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
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
    public enum LogOptions {
        DebugEnabled        = 1 << 0,
        InfoEnabled         = 1 << 1,
        WarningEnabled      = 1 << 2,
        ErrorEnabled        = 1 << 3,
        ExceptionDetailEnabled  = 1 << 4,
        
		VerboseProfile = DebugEnabled | InfoEnabled | WarningEnabled | ErrorEnabled | ExceptionDetailEnabled,
        StandardProfile = WarningEnabled | ErrorEnabled | ExceptionDetailEnabled,
        UserDisplayProfile = InfoEnabled | WarningEnabled | ErrorEnabled,
    }
}
