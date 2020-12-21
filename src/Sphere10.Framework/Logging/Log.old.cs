////-----------------------------------------------------------------------
//// <copyright file="Log.cs" company="Sphere 10 Software">
////
//// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
////
//// Distributed under the MIT software license, see the accompanying file
//// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
////
//// <author>Herman Schoenfeld</author>
//// <date>2018</date>
//// </copyright>
////-----------------------------------------------------------------------

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;

//namespace Sphere10.Framework {

//	/// <summary>
//	/// Provides application-wide logging functionality. Use it anywhere in the code.
//	/// </summary>
//	/// <remarks></remarks>
//	public static class Log {

//		static readonly List<ILogger> _loggers;

//		/// <summary>
//		/// Initializes a new instance of the <see cref="T:System.Object"/> class.
//		/// </summary>
//		/// <remarks></remarks>
//		static Log() {
//			_loggers = new List<ILogger>();
//		}

//		/// <summary>
//		/// Logs a debug message.
//		/// </summary>
//		/// <param name="message">The message.</param>
//		public static void Debug(string message, params object[] formatOptions) {
//			_loggers.ForEach(logger => logger.Debug(message));
//		}

//		/// <summary>
//		/// Logs an information message.
//		/// </summary>
//		/// <param name="message">The message.</param>
//		/// <param name="formatOptions">The format options (if any)</param>
//		public static void Info(string message, params object[] formatOptions) {
//			_loggers.ForEach(logger => logger.Info(message));
//		}

//		/// <summary>
//		/// Logs a warning message.
//		/// </summary>
//		/// <param name="message">The message.</param>
//		/// <param name="formatOptions">The format options (if any)</param>
//		public static void Warning(string message, params object[] formatOptions) {
//			_loggers.ForEach(logger => logger.Warning(message));
//		}

//		/// <summary>
//		/// Logs an error message.
//		/// </summary>
//		/// <param name="message">The message.</param>
//		/// <param name="formatOptions">The format options (if any)</param>
//		public static void Error(string message, params object[] formatOptions) {
//			_loggers.ForEach(logger => logger.Error(message));
//		}

//		/// <summary>
//		/// Registers a logger which will receive all debug, info, warning and error messages.
//		/// </summary>
//		/// <param name="logger">The logger.</param>
//		public static void RegisterLogger(ILogger logger) {
//			#region Validate parameters
//			if (logger == null) {
//				throw new ArgumentNullException("logger");
//			}
//			#endregion
//			_loggers.Add(logger);
//		}

//		/// <summary>
//		/// Deregisters a previously registered logger.
//		/// </summary>
//		/// <param name="logger">The logger.</param>
//		public static void DeregisterLogger(ILogger logger) {
//			_loggers.Remove(logger);
//		}
//	}
//}
