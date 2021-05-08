//-----------------------------------------------------------------------
// <copyright file="TextWriterLogger.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework {

	public class ActionLogger : ILogger {

		private readonly Action<string> _debugAction;
		private readonly Action<string> _infoAction;
		private readonly Action<string> _warningAction;
		private readonly Action<string> _errorAction;


		/// <summary>
		/// Can enable/disable logging levels
		/// </summary>
		public LogOptions Options { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="action"></param>
	    public ActionLogger(Action<string> action) 
			: this(action, action, action, action) {
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="debugAction"></param>
		/// <param name="infoAction"></param>
		/// <param name="warningAction"></param>
		/// <param name="errorAction"></param>
		public ActionLogger(Action<string> debugAction, Action<string> infoAction, Action<string> warningAction, Action<string> errorAction) {
			_debugAction = debugAction;
			_infoAction = infoAction;
			_warningAction = warningAction;
			_errorAction = errorAction;
			this.Options = LogOptions.DebugBuildDefaults;
		}


		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="formatOptions">The format options (if any)</param>
		/// <remarks></remarks>
		public void Debug(string message) {
			if (Options.HasFlag(LogOptions.DebugEnabled)) {
                LogMessage(_debugAction, message);
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
                LogMessage(_infoAction, message);
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
                LogMessage(_warningAction, message);
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
                LogMessage(_errorAction, message);
		    }
		}

	    protected virtual void LogMessage(Action<string> action, string message) {
	        try {
	            action(message);
	        } catch {
	            // errors do not propagate outside logging framework
	        }
	    }

	}
}
