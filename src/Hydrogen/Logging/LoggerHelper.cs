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
	public static bool TryHydrateErrorMessage(Exception exception, LogOptions logOptions, out string message) {
		if ((logOptions & LogOptions.ErrorEnabled) == 0) {
			message = null;
			return false;
		}
		message = (logOptions & LogOptions.ExceptionDetailEnabled) != 0 ? exception.ToDiagnosticString() : exception.ToDisplayString();
		return true;
	}

}
