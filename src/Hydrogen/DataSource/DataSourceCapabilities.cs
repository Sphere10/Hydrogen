// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

[Flags]
public enum DataSourceCapabilities {
	CanCreate = 1 << 0,
	CanRead = 1 << 1,
	CanUpdate = 1 << 2 | CanRead,
	CanDelete = 1 << 3,
	CanSearch = 1 << 4 | CanRead,
	CanSort = 1 << 5 | CanRead,
	CanPage = 1 << 6 | CanRead,
	Default = CanCreate | CanRead | CanUpdate | CanDelete | CanSearch | CanSort | CanPage,
	ComboBoxDefault = CanRead | CanSearch | CanSort
}
