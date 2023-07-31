// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// Loggers are used to process debug, information, warning and error messages.
/// </summary>
/// <remarks></remarks>
public interface ILogger {

	LogOptions Options { get; set; }

	/// <summary>
	/// Logs a debug message.
	/// </summary>
	/// <param name="message">The message.</param>
	void Debug(string message);

	/// <summary>
	/// Logs an information message.
	/// </summary>
	/// <param name="message">The message.</param>
	void Info(string message);

	/// <summary>
	/// Logs a warning message.
	/// </summary>
	/// <param name="message">The message.</param>
	void Warning(string message);

	/// <summary>
	/// Logs an error message.
	/// </summary>
	/// <param name="message">The message.</param>
	void Error(string message);

	/// <summary>
	/// Logs an exception message.
	/// </summary>
	/// <param name="exception">The exception.</param>
	/// <param name="message"></param>
	void Exception(Exception exception, string message = null);
}


public static class ILoggerExtensions {

	//public static void LogException(this ILogger logger, Exception exception)  
	//	logger.Error(TryHydrateErrorMessage(exception, logger.Options));

	//public static void LogException(this ILogger logger, string componentName, string methodName, Exception exception) {
	//	logger.Error($"{ComponentPrefix(componentName, methodName)} {exception.ToDiagnosticString()}");
	//}

	public static void Log(this ILogger logger, LogLevel logLevel, string message) {
		switch (logLevel) {
			case LogLevel.None:
				break;
			case LogLevel.Debug:
				logger.Debug(message);
				break;
			case LogLevel.Info:
				logger.Info(message);
				break;
			case LogLevel.Warning:
				logger.Warning(message);
				break;
			case LogLevel.Error:
				logger.Error(message);
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
		}
	}
	public static void Debug(this ILogger logger, string componentName, string methodName, string message) {
		logger.Debug($"{ComponentPrefix(componentName, methodName)} {message}");
	}

	public static void Info(this ILogger logger, string componentName, string methodName, string message) {
		logger.Info($"{ComponentPrefix(componentName, methodName)} {message}");
	}

	public static void Warning(this ILogger logger, string componentName, string methodName, string message) {
		logger.Warning($"{ComponentPrefix(componentName, methodName)} {message}");
	}

	public static void Error(this ILogger logger, string componentName, string methodName, string message) {
		logger.Error($"{ComponentPrefix(componentName, methodName)} {message}");
	}

	public static void Result(this ILogger logger, string componentName, string methodName, Result result) {
		foreach (var code in result.ErrorCodes)
			logger.Log(code.Severity, $"{ComponentPrefix(componentName, methodName)} {code.Payload}");
	}

	public static void Exception(this ILogger logger, Exception exception,  string componentName, string methodName, string message = null) {
		logger.Exception(exception, $"{ComponentPrefix(componentName, methodName)} {message}");
	}

	public static IDisposable LogDuration(this ILogger logger, string messagePrefix) {
		var start = DateTime.Now;
		return new ActionScope(
			() => logger.Debug($"{messagePrefix ?? string.Empty} ({(long)DateTime.Now.Subtract(start).TotalMilliseconds} ms)")
		);
	}


	private static string ComponentPrefix(string componentName, string methodName)
		=> $"[{componentName}::{methodName}]";
}
