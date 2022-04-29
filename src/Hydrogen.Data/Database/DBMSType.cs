//-----------------------------------------------------------------------
// <copyright file="DBMSType.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Hydrogen.Data {
	public enum DBMSType {
        [Description("SQL Server")]
		SQLServer			= 1,

        [Description("Sqlite")]
		Sqlite				= 2,

        [Description("Firebird")]
        Firebird			= 3,

        [Description("Firebird Embedded")]
        FirebirdFile		= 4,
	}
}
