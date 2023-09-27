// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Windows.Forms.AppointmentBook;

[Flags]
public enum CellTraits {
	Empty = 1,
	Filled = 1 << 1,
	Edge = 1 << 2 | Filled,
	Top = 1 << 3 | Edge | Filled,
	Bottom = 1 << 4 | Edge | Filled,
	Interior = 1 << 5 | Filled,
}
