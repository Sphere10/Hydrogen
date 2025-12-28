// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

internal static class LoggerHelper {
	/// <summary>
	/// Generates a formatted exception message based on the configured options.
	/// </summary>
	/// <param name="exception">Exception to format.</param>
	/// <param name="logOptions">Options controlling inclusion of stack traces.</param>
	/// <param name="message">Formatted output when the method returns <c>true</c>.</param>
	/// <returns><c>true</c> if error logging is enabled and the message was generated; otherwise <c>false</c>.</returns>
	public static bool TryHydrateErrorMessage(Exception exception, LogOptions logOptions, out string message) {
		if ((logOptions & LogOptions.ErrorEnabled) == 0) {
			message = null;
			return false;
		}
		message = (logOptions & LogOptions.ExceptionDetailEnabled) != 0 ? exception.ToDiagnosticString() : exception.ToDisplayString();
		return true;
	}

}
