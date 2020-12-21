//-----------------------------------------------------------------------
// <copyright file="SystemLog.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework{


	/// <summary>
	/// Provides application-wide logging functionality. Use it anywhere in the code.
	/// </summary>
	/// <remarks></remarks>
	public static class SystemLog {

		static readonly MulticastLogger _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
		/// </summary>
		/// <remarks></remarks>
		static SystemLog() {
			_logger = new MulticastLogger(false);
		}

		/// <summary>
		/// The internal logger.
		/// </summary>
		public static ILogger Logger => _logger;

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Debug(string message) {
			_logger.Debug(message);
		}

		public static void Debug2(string componentName, string methodName, string message) {
			_logger.Debug2(componentName, methodName, message);
		}


		/// <summary>
		/// Logs an information message.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Info(string message) {
			_logger.Info(message);
		}

		public static void Info2(string componentName, string methodName, string message) {
			_logger.Info2(componentName, methodName, message);
		}

		/// <summary>
		/// Logs a warning message.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Warning(string message) {
			_logger.Warning(message);
		}

		public static void Warning2(string componentName, string methodName, string message) {
			_logger.Warning2(componentName, methodName, message);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Error(string message) {
			_logger.Error(message);
		}

		public static void Error2(string componentName, string methodName, string message) {
			_logger.Error2(componentName, methodName, message);
		}

		public static void Exception(Exception exception) {
			_logger.LogException(exception);
		}

		public static void Exception(string componentName, string methodName, Exception exception) {
			_logger.LogException(componentName, methodName, exception);
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
}
