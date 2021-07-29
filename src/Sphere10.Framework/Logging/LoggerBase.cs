//-----------------------------------------------------------------------
// <copyright file="RollingFileLogger.cs" company="Sphere 10 Software">
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

using System.Configuration;
using System.IO;

namespace Sphere10.Framework {

    public abstract class LoggerBase : ILogger {
        public LogOptions Options {
            get;
            set;
        }

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatOptions">The format options (if any)</param>
		/// <remarks></remarks>
		public void Debug(string message) {
			if (Options.HasFlag(LogOptions.DebugEnabled)) {
				Log(LogLevel.Debug, message);
			}
		}

		/// <summary>
		/// Logs an information message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatOptions">The format options (if any)</param>
		/// <remarks></remarks>
		public void Info(string message) {
			if (Options.HasFlag(LogOptions.InfoEnabled)) {
				Log(LogLevel.Info, message);
			}
		}

		/// <summary>
		/// Logs a warning message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatOptions">The format options (if any)</param>
		/// <remarks></remarks>
		public void Warning(string message) {
			if (Options.HasFlag(LogOptions.WarningEnabled)) {
				Log(LogLevel.Warning, message);
			}
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatOptions">The format options (if any)</param>
		/// <remarks></remarks>
		public void Error(string message) {
			if (Options.HasFlag(LogOptions.ErrorEnabled)) {
				Log(LogLevel.Error, message);
			}
		}

		protected abstract void Log(LogLevel logLevel, string message);
	}
}
