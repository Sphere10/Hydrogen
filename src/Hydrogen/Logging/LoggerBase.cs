// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;


namespace Hydrogen;

/// <summary>
/// Base implementation of <see cref="ILogger"/> that filters messages using <see cref="Options"/>.
/// </summary>
public abstract class LoggerBase : ILogger {

	/// <summary>
	/// Gets or sets the flags that control which messages are emitted.
	/// Defaults to <see cref="LogOptions.VerboseProfile"/> in debug builds and <see cref="LogOptions.StandardProfile"/> otherwise.
	/// </summary>
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
	/// <param name="message">Additional context to append to the exception details.</param>
	public void Exception(Exception exception, string message = null) {
		if (LoggerHelper.TryHydrateErrorMessage(exception, Options, out var exceptionMessage))
			Log(LogLevel.Error, !string.IsNullOrWhiteSpace(message) ? $"{message}. {exceptionMessage}" : exceptionMessage);
	}

	/// <summary>
	/// Implemented by sub-class.
	/// </summary>
	/// <param name="logLevel">The logging level.</param>
	/// <param name="message">The message.</param>
	protected abstract void Log(LogLevel logLevel, string message);
}
