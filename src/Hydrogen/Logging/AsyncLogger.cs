// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// Wraps another logger and performs all logging calls on a dedicated worker thread.
/// </summary>
public class AsyncLogger : LoggerDecorator {

	private readonly SerialThreadPool _serialThreadPool;

	/// <summary>
	/// Creates an asynchronous logger that forwards messages to <paramref name="decoratedLogger"/>.
	/// </summary>
	/// <param name="decoratedLogger">Underlying logger that performs the actual write.</param>
	/// <param name="errorHandler">Optional callback invoked if the worker thread encounters an exception.</param>
	public AsyncLogger(ILogger decoratedLogger, Action<Exception> errorHandler = null) : base(decoratedLogger) {
		_serialThreadPool = new SerialThreadPool(errorHandler);
	}

	public override void Debug(string message) {
		_serialThreadPool.QueueUserWorkItem(() => base.Debug(message));
	}

	public override void Info(string message) {
		_serialThreadPool.QueueUserWorkItem(() => base.Info(message));
	}

	public override void Warning(string message) {
		_serialThreadPool.QueueUserWorkItem(() => base.Warning(message));
	}

	public override void Error(string message) {
		_serialThreadPool.QueueUserWorkItem(() => base.Error(message));
	}

	public override void Exception(Exception exception, string message) {
		_serialThreadPool.QueueUserWorkItem(() => base.Exception(exception, message));
	}
};
