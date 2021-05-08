//-----------------------------------------------------------------------
// <copyright file="MultiCastLogger.cs" company="Sphere 10 Software">
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
using System.Threading.Tasks;

namespace Sphere10.Framework {

	public class MulticastLogger : ILogger {
		private readonly bool _multiThreaded;
		private readonly SynchronizedList<ILogger> _loggers;

		public MulticastLogger(bool multiThreaded = false)
			: this(new List<ILogger>()) {
			_multiThreaded = multiThreaded;
			Options = LogOptions.DebugBuildDefaults;
		}

		public MulticastLogger(params ILogger[] loggers)
			: this(new List<ILogger>(loggers)) {
		}

		public MulticastLogger(IEnumerable<ILogger> loggers)
			: this(new List<ILogger>(loggers)) {
		}

		public MulticastLogger(IList<ILogger> loggers) {
			_loggers = new SynchronizedList<ILogger>();
			loggers.ForEach(_loggers.Add);
		}

		public LogOptions Options {
			get => throw new NotSupportedException("Options can only be set in MultiCastLogger");
			set {
				using (_loggers.EnterWriteScope()) {
					_loggers.ForEach(l => l.Options = value);
				}
			}
		}

		public void Add(ILogger logger) {
			_loggers.Add(logger);
		}

		public bool Remove(ILogger logger) {
			return _loggers.Remove(logger);
		}

		public void Clear() {
			_loggers.Clear();
		}

		public void Debug(string message) {
			using (_loggers.EnterReadScope()) {
				if (_multiThreaded)
					Parallel.ForEach(_loggers, (logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Debug(message)));
				else
					_loggers.ForEach((logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Debug(message)));
			}
		}

		public void Info(string message) {
			using (_loggers.EnterReadScope()) {
				if (_multiThreaded)
					Parallel.ForEach(_loggers, (logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Info(message)));
				else
					_loggers.ForEach((logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Debug(message)));

			}
		}

		public void Warning(string message) {
			using (_loggers.EnterReadScope()) {
				if (_multiThreaded)
					Parallel.ForEach(_loggers, (logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Warning(message)));
				else
					_loggers.ForEach((logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Debug(message)));
			}
		}

		public void Error(string message) {
			using (_loggers.EnterReadScope()) {
				if (_multiThreaded)
					Parallel.ForEach(_loggers, (logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Error(message)));
				else
					_loggers.ForEach((logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Debug(message)));
			}
		}

	}
}
