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
/// <summary>
/// Bit flags that configure how a logger records messages and exception details.
/// </summary>
public enum LogOptions {
	/// <summary>
	/// Emit debug messages.
	/// </summary>
	DebugEnabled = 1 << 0,
	/// <summary>
	/// Emit informational messages.
	/// </summary>
	InfoEnabled = 1 << 1,
	/// <summary>
	/// Emit warnings.
	/// </summary>
	WarningEnabled = 1 << 2,
	/// <summary>
	/// Emit errors.
	/// </summary>
	ErrorEnabled = 1 << 3,
	/// <summary>
	/// Include exception diagnostic details (stack trace and inner exceptions) when logging errors.
	/// </summary>
	ExceptionDetailEnabled = 1 << 4,

	/// <summary>
	/// Enables every message type and detailed exception output.
	/// </summary>
	VerboseProfile = DebugEnabled | InfoEnabled | WarningEnabled | ErrorEnabled | ExceptionDetailEnabled,
	/// <summary>
	/// Emits warnings and errors with detailed exception output.
	/// </summary>
	StandardProfile = WarningEnabled | ErrorEnabled | ExceptionDetailEnabled,
	/// <summary>
	/// Emits user-facing informational, warning and error messages without stack traces.
	/// </summary>
	UserDisplayProfile = InfoEnabled | WarningEnabled | ErrorEnabled,
}
