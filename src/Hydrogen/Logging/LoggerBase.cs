//-----------------------------------------------------------------------
// <copyright file="RollingFileLogger.cs" company="Sphere 10 Software">
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


namespace Hydrogen;

public abstract class LoggerBase : ILogger {

	public LogOptions Options { get; set; } = Tools.Runtime.IsDebugBuild ? LogOptions.VerboseProfile : LogOptions.StandardProfile;

	/// <summary>
	/// Logs a debug message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="formatOptions">The format options (if any)</param>
	/// <remarks></remarks>
	public void Debug(string message) {
		if (message != null && Options.HasFlag(LogOptions.DebugEnabled))
			Log(LogLevel.Debug, message);
	}

	/// <summary>
	/// Logs an information message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="formatOptions">The format options (if any)</param>
	/// <remarks></remarks>
	public void Info(string message) {
		if (message != null && Options.HasFlag(LogOptions.InfoEnabled))
			Log(LogLevel.Info, message);
	}

	/// <summary>
	/// Logs a warning message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="formatOptions">The format options (if any)</param>
	/// <remarks></remarks>
	public void Warning(string message) {
		if (message != null && Options.HasFlag(LogOptions.WarningEnabled))
			Log(LogLevel.Warning, message);
	}

	/// <summary>
	/// Logs an error message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="formatOptions">The format options (if any)</param>
	/// <remarks></remarks>
	public void Error(string message) {
		if (message != null && Options.HasFlag(LogOptions.ErrorEnabled))
			Log(LogLevel.Error, message);
	}

	/// <summary>
	/// Logs an exception.
	/// </summary>
	/// <param name="exception">The exception.</param>
	public void Exception(Exception exception) {
		if (LoggerHelper.TryHydrateErrorMessage(exception, Options, out var message))
			Log(LogLevel.Error, message);
	}

	/// <summary>
	/// Implemented by sub-class.
	/// </summary>
	/// <param name="logLevel">The logging level.</param>
	/// <param name="message">The message.</param>
	protected abstract void Log(LogLevel logLevel, string message);
}


