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
/// Provides application-wide logging functionality. Use it anywhere in the code.
/// </summary>
public static class SystemLog {

	static readonly MulticastLogger _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:System.Object"/> class.
	/// </summary>
	/// <remarks></remarks>
	static SystemLog() {
		_logger = new MulticastLogger();
	}

	/// <summary>
	/// The internal logger that broadcasts to registered sinks.
	/// </summary>
	public static ILogger Logger => _logger;

	/// <summary>
	/// Logs a debug message.
	/// </summary>
	/// <param name="message">The message.</param>
	public static void Debug(string message) {
		_logger.Debug(message);
	}

	/// <summary>
	/// Logs a debug message with component and method prefixes.
	/// </summary>
	public static void Debug(string componentName, string methodName, string message) {
		_logger.Debug(componentName, methodName, message);
	}


	/// <summary>
	/// Logs an information message.
	/// </summary>
	/// <param name="message">The message.</param>
	public static void Info(string message) {
		_logger.Info(message);
	}

	/// <summary>
	/// Logs an informational message with component and method prefixes.
	/// </summary>
	public static void Info(string componentName, string methodName, string message) {
		_logger.Info(componentName, methodName, message);
	}

	/// <summary>
	/// Logs a warning message.
	/// </summary>
	/// <param name="message">The message.</param>
	public static void Warning(string message) {
		_logger.Warning(message);
	}

	/// <summary>
	/// Logs a warning message with component and method prefixes.
	/// </summary>
	public static void Warning(string componentName, string methodName, string message) {
		_logger.Warning(componentName, methodName, message);
	}

	/// <summary>
	/// Logs an error message.
	/// </summary>
	/// <param name="message">The message.</param>
	public static void Error(string message) {
		_logger.Error(message);
	}

	/// <summary>
	/// Logs an error message with component and method prefixes.
	/// </summary>
	public static void Error(string componentName, string methodName, string message) {
		_logger.Error(componentName, methodName, message);
	}


	/// <summary>
	/// Logs an exception using the configured log options.
	/// </summary>
	public static void Exception(Exception exception, string message = null) => _logger.Exception(exception, message);

	/// <summary>
	/// Logs an exception with component and method prefixes, honoring the logger's exception detail settings.
	/// </summary>
	public static void Exception(string componentName, string methodName, Exception exception) {
		if (LoggerHelper.TryHydrateErrorMessage(exception, _logger.Options, out var message))
			_logger.Error(componentName, methodName, message);
	}

	/// <summary>
	/// Registers a logger which will receive all debug, info, warning and error messages.
	/// </summary>
	/// <param name="logger">The logger.</param>
	public static void RegisterLogger(ILogger logger) {

		#region Validate parameters

		if (logger == null) {
			throw new ArgumentNullException("logger");
		}

		#endregion

		_logger.Add(logger);
	}

	/// <summary>
	/// Deregisters a previously registered logger.
	/// </summary>
	/// <param name="logger">The logger.</param>
	public static void DeregisterLogger(ILogger logger) {
		_logger.Remove(logger);
	}

	/// <summary>
	/// Deregisters a previously registered logger.
	/// </summary>
	/// <param name="logger">The logger.</param>
	public static void DeregisterAllLoggers() {
		_logger.Clear();
	}
}
