//-----------------------------------------------------------------------
// <copyright file="CellTraits.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Windows.Forms.AppointmentBook {
	[Flags]
	public enum CellTraits {
		Empty			= 1,
		Filled			= 1 << 1,
		Edge			= 1 << 2 | Filled,
		Top				= 1 << 3 | Edge | Filled,
		Bottom			= 1 << 4 | Edge | Filled,
		Interior		= 1 << 5 | Filled,
	}
}
