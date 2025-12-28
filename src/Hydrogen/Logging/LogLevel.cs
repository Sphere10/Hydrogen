// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Runtime.Serialization;

namespace Hydrogen;

/// <summary>
/// Describes the severity of a log entry.
/// </summary>
public enum LogLevel {
	/// <summary>
	/// Indicates logging is disabled.
	/// </summary>
	[EnumMember(Value = "none")] None,

	/// <summary>
	/// Diagnostic messages intended for developers.
	/// </summary>
	[EnumMember(Value = "debug")] Debug,

	/// <summary>
	/// Informational messages about normal operation.
	/// </summary>
	[EnumMember(Value = "info")] Info,

	/// <summary>
	/// Non-fatal issues that may require attention.
	/// </summary>
	[EnumMember(Value = "warning")] Warning,

	/// <summary>
	/// Errors that indicate an operation failed.
	/// </summary>
	[EnumMember(Value = "error")] Error,

}


public static class LogLevelExtensions {
	/// <summary>
	/// Converts a log level into the corresponding <see cref="LogOptions"/> bit mask.
	/// </summary>
	public static LogOptions ToLogOptions(this LogLevel logLevel) =>
		logLevel switch {
			LogLevel.None => 0,
			LogLevel.Debug => LogOptions.ExceptionDetailEnabled | LogOptions.ErrorEnabled | LogOptions.WarningEnabled | LogOptions.InfoEnabled | LogOptions.DebugEnabled,
			LogLevel.Info => LogOptions.ExceptionDetailEnabled | LogOptions.ErrorEnabled | LogOptions.WarningEnabled | LogOptions.InfoEnabled,
			LogLevel.Warning => LogOptions.ExceptionDetailEnabled | LogOptions.ErrorEnabled | LogOptions.WarningEnabled,
			LogLevel.Error => LogOptions.ExceptionDetailEnabled | LogOptions.ErrorEnabled,
			_ => throw new NotSupportedException(logLevel.ToString())
		};
}
