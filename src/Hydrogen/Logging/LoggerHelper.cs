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
