//-----------------------------------------------------------------------
// <copyright file="CrudCapabilities.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;

namespace Sphere10.Framework {
	[Flags]
	public enum DataSourceCapabilities {
		CanCreate	= 1 << 0,
		CanRead		= 1 << 1,
		CanUpdate	= 1 << 2 | CanRead,
		CanDelete	= 1 << 3,
		CanSearch	= 1 << 4 | CanRead,
		CanSort		= 1 << 5 | CanRead,
		CanPage		= 1 << 6 | CanRead,
		Default = CanCreate | CanRead | CanUpdate | CanDelete | CanSearch | CanSort | CanPage,
		ComboBoxDefault = CanRead | CanSearch | CanSort
	}
}
