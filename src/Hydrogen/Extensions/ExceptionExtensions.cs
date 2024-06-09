// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hydrogen;

public static class ExceptionExtensions {

	/// <summary>
	/// Gets a sentenced-formatted paragraph of this exception and it's nested inner exceptions messages.
	/// </summary>
	/// <param name="exception">Source exception</param>
	/// <returns>User-displayable exception message.</returns>
	/// <remarks></remarks>
	public static string ToDisplayString(this Exception exception) {
		var aggException = exception as AggregateException;
		if (aggException == null) {
			return ToDisplayStringInternal(exception);
		}

		var exceptions = aggException.InnerExceptions.ToArray();
		if (exceptions.Length == 1) {
			return ToDisplayString(exceptions[0]);
		}

		var message = new StringBuilder();
		message.Append("Multiple errors have occured:");
		for (var i = 0; i < exceptions.Length; i++) {
			message.AppendFormat("{0}{0}", Environment.NewLine);
			message.AppendFormat("[{0}]: {1}", i + 1, ToDisplayStringInternal(exceptions[i]));
		}
		return message.ToString();
	}

	private static string ToDisplayStringInternal(this Exception exception) {
		var message = new StringBuilder();
		var currException = exception;
		while (currException != null) {
			var currMsg = currException.Message.Trim();
			if (currMsg.Length > 0) {
				if (message.Length > 0) {
					message.Append(" ");
				}
				message.Append(char.ToUpper(currMsg[0]));
				message.Append(currMsg.Substring(1));
				if (!currMsg.EndsWith(".")) {
					message.Append(".");
				}
			}
			currException = currException.InnerException;
		}
		return message.ToString();
	}


	/// <summary>
	/// <para>Creates a diagnostic detailed message from the Exception.</para>
	/// <para>The result includes the stacktrace, innerexception et cetera, separated by <seealso cref="Environment.NewLine"/>.</para>
	/// </summary>
	/// <param name="currException">The exception to create the string from.</param>
	/// <param name="additionalMessage">Additional message to place at the top of the string, maybe be empty or null.</param>
	/// <returns></returns>
	public static string ToDiagnosticString(this Exception exception) {
		var aggException = exception as AggregateException;
		if (aggException == null) {
			return ToDiagnosticStringInternal(exception);
		}

		var exceptions = aggException.InnerExceptions.ToArray();
		if (exceptions.Length == 1) {
			return ToDiagnosticString(exceptions[0]);
		}

		var message = new StringBuilder();
		message.AppendLine("Multiple exceptions have occured ({0}):", exceptions.Length);
		for (var i = 0; i < exceptions.Length; i++) {
			message.AppendLine();
			message.AppendFormat("Exception {0}:{1}{1}", i + 1, Environment.NewLine);
			message.AppendFormat(ToDiagnosticStringInternal(exceptions[i]));
		}
		return message.ToString();
	}


	private static string ToDiagnosticStringInternal(this Exception exception) {
		var message = new StringBuilder();
		var currException = exception;
		while (currException != null) {
			message.AppendFormat("[{0}: {1}]{2}", currException.GetType().Name, currException.Message, Environment.NewLine);

			message.AppendLine("Data:");
			foreach (DictionaryEntry i in currException.Data) {
				message.AppendFormat("\t{0}:{1}", i.Key, i.Value);
				message.AppendLine();
			}

			if (currException.StackTrace != null) {
				message.AppendLine("StackTrace:");
				message.AppendLine(currException.StackTrace.ToString());
			}

			if (currException.Source != null) {
				message.AppendLine("Source:");
				message.AppendLine(currException.Source);
			}

			if (currException.TargetSite != null) {
				message.AppendLine("TargetSite:");
				message.AppendLine(currException.TargetSite.ToString());
			}

			message.Append(Environment.NewLine);
			message.Append(Environment.NewLine);
			currException = currException.InnerException;
		}
		return message.ToString();
	}
	/// <summary>
	/// Gets this and it's nested exceptions as a flat list.
	/// </summary>
	/// <param name="exception">The exception.</param>
	/// <returns>Flattened exceptions.</returns>
	public static IEnumerable<Exception> FlattenedExceptionList(this Exception exception) {
		List<Exception> retval = new List<Exception>();
		do {
			retval.Add(exception);
		} while (null != (exception = exception.InnerException));
		return retval;
	}


	/// <summary>
	/// Determines whether the specified exception has exception.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="exception">The exception.</param>
	/// <returns><c>true</c> if the specified exception has exception; otherwise, <c>false</c>.</returns>
	/// <remarks></remarks>
	public static bool HasException<T>(this Exception exception) where T : Exception {
		foreach (var e in exception.FlattenedExceptionList()) {
			if (e is T) {
				return true;
			}
		}
		return false;
	}

	public static Exception InnerMostException(this Exception exception) {
		if (exception.InnerException == null)
			return exception;

		return exception.InnerException.InnerMostException();
	}
}
