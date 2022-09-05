//-----------------------------------------------------------------------
// <copyright file="LogLevel.cs" company="Sphere 10 Software">
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
using System.Runtime.Serialization;

namespace Hydrogen {

	public enum LogLevel {
		[EnumMember(Value = "none")]
		None,

		[EnumMember(Value = "debug")]
		Debug,

		[EnumMember(Value = "info")]
		Info,

		[EnumMember(Value = "warning")]
		Warning,

		[EnumMember(Value = "error")]
		Error,

	}

	public static class LogLevelExtensions  {
		public static LogOptions ToLogOptions(this LogLevel logLevel) => 
			logLevel switch {
				LogLevel.None => 0,
				LogLevel.Debug => LogOptions.ExceptionDetailEnabled | LogOptions.ErrorEnabled | LogOptions.WarningEnabled | LogOptions.InfoEnabled | LogOptions.DebugEnabled,
				LogLevel.Info => LogOptions.ExceptionDetailEnabled |LogOptions.ErrorEnabled | LogOptions.WarningEnabled | LogOptions.InfoEnabled,
				LogLevel.Warning => LogOptions.ExceptionDetailEnabled | LogOptions.ErrorEnabled | LogOptions.WarningEnabled,
				LogLevel.Error => LogOptions.ExceptionDetailEnabled |  LogOptions.ErrorEnabled,
				_ => throw new NotSupportedException(logLevel.ToString())
			};
	}
}