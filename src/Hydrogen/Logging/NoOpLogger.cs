//-----------------------------------------------------------------------
// <copyright file="NoOpLogger.cs" company="Sphere 10 Software">
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

namespace Hydrogen {

	/// <summary>
	/// No-operation logger. Does nothing.
	/// </summary>
	/// <remarks></remarks>
	public class NoOpLogger : ILogger {
	    public LogOptions Options { get; set; }
	    /// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatOptions">The format options (if any)</param>
		/// <remarks></remarks>
		public void Debug(string message) {
			// do nothing
		}

		/// <summary>
		/// Logs an information message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatOptions">The format options (if any)</param>
		/// <remarks></remarks>
		public void Info(string message) {
			// do nothing
		}

		/// <summary>
		/// Logs a warning message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatOptions">The format options (if any)</param>
		/// <remarks></remarks>
		public void Warning(string message) {
			// do nothing
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatOptions">The format options (if any)</param>
		/// <remarks></remarks>
		public void Error(string message) {
			// do nothing
		}

	}
}
