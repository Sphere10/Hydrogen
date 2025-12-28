// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// Forwards log messages to a collection of child loggers, optionally in parallel.
/// </summary>
public class MulticastLogger : ILogger {

	private readonly SynchronizedList<ILogger> _loggers;

	/// <summary>
	/// Creates an empty multicast logger.
	/// </summary>
	public MulticastLogger()
		: this(new List<ILogger>()) {
	}

	/// <summary>
	/// Creates a multicast logger with an initial set of loggers.
	/// </summary>
	/// <param name="loggers">Loggers that will receive forwarded entries.</param>
	public MulticastLogger(params ILogger[] loggers)
		: this(new List<ILogger>(loggers)) {
	}

	/// <summary>
	/// Creates a multicast logger with an initial set of loggers.
	/// </summary>
	/// <param name="loggers">Loggers that will receive forwarded entries.</param>
	public MulticastLogger(IEnumerable<ILogger> loggers)
		: this(new List<ILogger>(loggers)) {
	}

	/// <summary>
	/// Creates a multicast logger with a pre-populated list instance.
	/// </summary>
	/// <param name="loggers">Loggers that will receive forwarded entries.</param>
	public MulticastLogger(IList<ILogger> loggers) {
		_loggers = new SynchronizedList<ILogger>();
		loggers.ForEach(_loggers.Add);
	}

	/// <summary>
	/// When set, uses <see cref="Parallel.ForEach{TSource}(System.Collections.Generic.IEnumerable{TSource}, System.Action{TSource})"/> to dispatch logs concurrently.
	/// </summary>
	public bool MultiThreaded { get; init; } = false;

	/// <summary>
	/// Applies log options to every child logger.
	/// </summary>
	public LogOptions Options {
		get => throw new NotSupportedException("Options can only be set in MultiCastLogger");
		set {
			using (_loggers.EnterWriteScope()) {
				_loggers.ForEach(l => l.Options = value);
			}
		}
	}

	/// <summary>
	/// Registers a new logger to receive forwarded messages.
	/// </summary>
	public void Add(ILogger logger) {
		_loggers.Add(logger);
	}

	/// <summary>
	/// Removes a registered logger.
	/// </summary>
	/// <returns><c>true</c> if the logger was found and removed; otherwise <c>false</c>.</returns>
	public bool Remove(ILogger logger) {
		return _loggers.Remove(logger);
	}

	/// <summary>
	/// Removes all registered loggers.
	/// </summary>
	public void Clear() {
		_loggers.Clear();
	}

	public void Debug(string message) {
		using (_loggers.EnterReadScope()) {
			if (MultiThreaded)
				Parallel.ForEach(_loggers, (logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Debug(message)));
			else
				_loggers.ForEach((logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Debug(message)));
		}
	}

	public void Info(string message) {
		using (_loggers.EnterReadScope()) {
			if (MultiThreaded)
				Parallel.ForEach(_loggers, (logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Info(message)));
			else
				_loggers.ForEach((logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Info(message)));

		}
	}

	public void Warning(string message) {
		using (_loggers.EnterReadScope()) {
			if (MultiThreaded)
				Parallel.ForEach(_loggers, (logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Warning(message)));
			else
				_loggers.ForEach((logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Warning(message)));
		}
	}

	public void Error(string message) {
		using (_loggers.EnterReadScope()) {
			if (MultiThreaded)
				Parallel.ForEach(_loggers, (logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Error(message)));
			else
				_loggers.ForEach((logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Error(message)));
		}
	}

	public void Exception(Exception exception, string message = null) {
		using (_loggers.EnterReadScope()) {
			if (MultiThreaded)
				Parallel.ForEach(_loggers, (logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Exception(exception, message)));
			else
				_loggers.ForEach((logger) => Tools.Exceptions.ExecuteIgnoringException(() => logger.Exception(exception, message)));
		}
	}

}
