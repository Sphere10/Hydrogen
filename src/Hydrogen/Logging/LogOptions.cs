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
public enum LogOptions {
	DebugEnabled = 1 << 0,
	InfoEnabled = 1 << 1,
	WarningEnabled = 1 << 2,
	ErrorEnabled = 1 << 3,
	ExceptionDetailEnabled = 1 << 4,

	VerboseProfile = DebugEnabled | InfoEnabled | WarningEnabled | ErrorEnabled | ExceptionDetailEnabled,
	StandardProfile = WarningEnabled | ErrorEnabled | ExceptionDetailEnabled,
	UserDisplayProfile = InfoEnabled | WarningEnabled | ErrorEnabled,
}
