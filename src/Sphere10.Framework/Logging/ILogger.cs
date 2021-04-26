//-----------------------------------------------------------------------
// <copyright file="ILogger.cs" company="Sphere 10 Software">
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Sphere10.Framework{

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

	}

	public static class ILoggerExtensions {

		public static void LogException(this ILogger logger, Exception exception) {
			logger.Error(exception.ToDiagnosticString());
		}

		public static void LogException(this ILogger logger, string componentName, string methodName, Exception exception) {
			logger.Error($"{ComponentPrefix(componentName, methodName)} {exception.ToDiagnosticString()}");
		}


		public static void Debug(this ILogger logger, string componentName, string methodName, string message) {
			logger.Debug($"{ComponentPrefix(componentName, methodName)} {message}");
		}

		public static  void Info(this ILogger logger, string componentName, string methodName, string message) {
			logger.Info($"{ComponentPrefix(componentName, methodName)} {message}");
		}

		public static void Warning(this ILogger logger, string componentName, string methodName, string message) {
			logger.Warning($"{ComponentPrefix(componentName, methodName)} {message}");
		}

		public static void Error(this ILogger logger, string componentName, string methodName, string message) {
			logger.Error($"{ComponentPrefix(componentName, methodName)} {message}");
		}

		public static void Result(this ILogger logger, string componentName, string methodName, Result result) {
			if (result.HasInformation) {
				foreach(var info in result.InformationMessages) 
					logger.Info(componentName, methodName, info);

				foreach (var error in result.ErrorMessages)
					logger.Info(componentName, methodName, error);
			}
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
}
